/* ******************************************************************************
	Maurizio Battisti
	19/07/2023
	Crea una copia della corsa co tutto 
****************************************************************************** */
CREATE PROCEDURE [dbo].[up_Run_CreateCopy]
	@RunId					uniqueidentifier,
	@NewRunId				uniqueidentifier = NULL,
	@ContractRowNumber		int = NULL,
	@LineNumber				int = NULL,
	@RunNumber				varchar(MAX) = NULL,
	@RunName				varchar(MAX) = NULL,
	@Path					varchar(MAX) = NULL,
	@StartTime				Time = NULL,
	@InvertNodes			bit = 0,
	@Inc_AdditionalDays		bit = 1,
	@Inc_ElastibusDays		bit = 1,
	@Inc_Suspensions		bit = 1,
	@Inc_Replacements		bit = 1,
	@Inc_Tabs				bit = 1,
	@Inc_Forfaits			bit = 1,
	@Inc_InternalNotes		bit = 1
AS
BEGIN
	SET DATEFIRST 1; -- this sets Monday to the first day of the week for the current connection.

	IF @NewRunId IS NULL SET @NewRunId = newID();

	DECLARE @Tbl_Map AS TABLE 
	(
		NaoType		varchar(10) NOT NULL,
		ParentId	uniqueidentifier NULL,
		OldI_d		uniqueidentifier NOT NULL,
		New_Id		uniqueidentifier NOT NULL DEFAULT(NEWID())
	);

	DECLARE @Tbl_Nodes AS TABLE (
		RunNodeId		uniqueidentifier,
		RunVariationId	uniqueidentifier,
		[Hour]			time,
		Number			int
	);

	BEGIN TRAN;

	-- carica i nuovi dati o li assegna identici ai vecchi 
	SELECT 
			@ContractRowNumber = COALESCE(@ContractRowNumber, r.ContractRowNumber),
			@LineNumber = COALESCE(@LineNumber, r.LineNumber), 
			@RunNumber = COALESCE(@RunNumber, r.RunNumber),
			@Path = COALESCE(@Path, r.Path),
			@StartTime = COALESCE(@StartTime, r.StartTime),
			@RunName = COALESCE(@RunName, r.RunName)
		FROM dbo.vw_Runs r
		WHERE r.RunId = @RunId;

	-- crea la tabella di mappatura
	BEGIN 
		-- salva gli id delle varianti
		INSERT INTO @Tbl_Map (NaoType, OldI_d, ParentId)
			SELECT  'VAR', v.RunVariationId, v.RunId
				FROM dbo.RunVariations v
				WHERE v.RunId = @RunId;
	
		-- savla i periodi
		INSERT INTO @Tbl_Map (NaoType, OldI_d, ParentId)
			SELECT  'PER', p.RunPeriodId, p.RunId
				FROM dbo.RunPeriods p
				WHERE p.RunId = @RunId;

		-- inserisce i mezzi
		INSERT INTO @Tbl_Map (NaoType, OldI_d, ParentId)
			SELECT  'CAR', rc.RunCarId, rc.RunPeriodId
				FROM dbo.RunCars rc
				INNER JOIN dbo.RunPeriods p
					ON Rc.RunPeriodId = p.RunPeriodId
				WHERE p.RunId = @RunId;

		-- inserisce i costi
		INSERT INTO @Tbl_Map (NaoType, OldI_d, ParentId)
			SELECT  'COST', rcc.RunCarCostId, rcc.RunCarId
				FROM dbo.RunCarCosts rcc
				INNER JOIN dbo.RunCars rc
					ON rc.RunCarId = rcc.RunCarId
				INNER JOIN dbo.RunPeriods p
					ON Rc.RunPeriodId = p.RunPeriodId
				WHERE p.RunId = @RunId;

		-- inserisce le sostituzioni
		INSERT INTO @Tbl_Map (NaoType, OldI_d, ParentId)
			SELECT  'REPL', repl.CarReplacementId, repl.RunPeriodId
				FROM dbo.RunCarReplacements repl
				INNER JOIN dbo.RunPeriods p
					ON repl.RunPeriodId = p.RunPeriodId
				WHERE p.RunId = @RunId;
	END;

	-- inserisce la nuova corsa
	INSERT INTO dbo.Runs 
			(RunId, ContractId, Extra,
			ContractRowNumber,
			StartDate,  EndDate,
			RequestedDays, Note,
			RunName
			)
		SELECT @NewRunId, r.ContractId, r.Extra, 
				@ContractRowNumber, 
				r.StartDate, r.EndDate,
				r.RequestedDays, r.Note, 
				@RunName
			FROM dbo.Runs r
			WHERE r.RunId = @RunId;

	-- inserisce le varianti
	INSERT INTO dbo.RunVariations 
			(RunVariationId, RunId, StartDate,
			LineNumber, RunNumber, [Path], StartTime, EndTime,
			Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday,
			RequestedFrequency, RequestedCapacity,
			Km, Note
			)
		SELECT m.New_Id, @NewRunId, v.StartDate, 
				@LineNumber, @RunNumber, @Path, @StartTime, DATEADD(SECOND, DATEDIFF(SECOND, v.StartTime, v.EndTime), @StartTime),
				v.Monday, v.Tuesday, v.Wednesday, v.Thursday, v.Friday, v.Saturday, v.Sunday,
				v.RequestedFrequency, v.RequestedCapacity, 
				v.Km, v.Note
			FROM dbo.RunVariations v
			INNER JOIN @Tbl_Map m
				ON m.NaoType = 'VAR'
				AND v.RunId = m.ParentId;

	-- inserisce i calendari
	INSERT INTO dbo.RunVariationCalendars
		(RunVariationId, CalendarId, Exclusion,
		Monday, Tuesday, Wednesday,
		Thursday, Friday, Saturday, Sunday
		)
		SELECT  m.New_Id, vc.CalendarId, vc.Exclusion,
				vc.Monday, vc.Tuesday, vc.Wednesday,
				vc.Thursday, vc.Friday, vc.Saturday, vc.Sunday
			FROM  dbo.RunVariationCalendars vc
			INNER JOIN @Tbl_Map m
				ON m.NaoType = 'VAR'
				AND vc.RunVariationId = m.OldI_d;

	-- inserisce i nodi
	BEGIN
		IF @InvertNodes = 0
		BEGIN
			INSERT INTO @Tbl_Nodes
				(RunNodeId, RunVariationId, [Hour], Number)
			SELECT n.RunNodeId,
					n.RunVariationId,
					n.[Hour],
					ROW_NUMBER() OVER (PARTITION BY n.RunVariationId ORDER BY n.hour ASC)
				FROM dbo.RunNodes n
				INNER JOIN @Tbl_Map m
					ON m.NaoType = 'VAR'
					AND n.RunVariationId = m.OldI_d;
		END
		ELSE
		BEGIN
			INSERT INTO @Tbl_Nodes
				(RunNodeId, RunVariationId, [Hour], Number)
			SELECT n.RunNodeId,
					n.RunVariationId,
					n.[Hour],
					ROW_NUMBER() OVER (PARTITION BY n.RunVariationId ORDER BY n.hour DESC)
				FROM dbo.RunNodes n
				INNER JOIN @Tbl_Map m
					ON m.NaoType = 'VAR'
					AND n.RunVariationId = m.OldI_d;
		END;

		WITH CTE_First AS
		(
			SELECT n.*
				FROM @Tbl_Nodes n
				WHERE n.Number = 1
		), CTE_NewNodes AS
		(
			SELECT n.*,
					DATEADD(SECOND, DATEDIFF(SECOND, f.Hour, n.Hour) * (CASE @InvertNodes WHEN 1 THEN -1 ELSE 1 END), @StartTime) AS newHour
				FROM dbo.RunNodes n
				INNER JOIN CTE_First f
					ON n.RunVariationId = f.RunVariationId
		)
		INSERT INTO dbo.RunNodes
			(RunNodeId, RunVariationId,
			ProgrNumber, [Hour], CollectionPointId,
			CoincidenceDescr)
			SELECT NEWID(), m.New_Id,
					ROW_NUMBER() OVER (PARTITION BY n.RunVariationId ORDER BY nn.newHour), 
					nn.newHour, n.CollectionPointId,
					n.CoincidenceDescr
				FROM dbo.RunNodes n
				INNER JOIN CTE_NewNodes nn
					ON n.RunNodeId = nn.RunNodeId
				INNER JOIN @Tbl_Map m
					ON m.NaoType = 'VAR'
					AND m.OldI_d = n.RunVariationId;

		-- ricalcola ora di inizo e fine in base ai nodi
		-- TODO: aggiorna inizioe fine sula variante

	END

	-- inserisce i periodi
	INSERT INTO dbo.RunPeriods 
		(RunId, RunPeriodId,
		StartDate, EndDate,
		Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday,
		Note)
		SELECT @NewRunId, m.New_Id,
				p.StartDate, p.EndDate,
				p.Monday, p.Tuesday, p.Wednesday, p.Thursday, p.Friday, p.Saturday, p.Sunday,
				p.Note
			FROM dbo.RunPeriods  p
			INNER JOIN @Tbl_Map m
				ON m.NaoType = 'PER'
				AND m.OldI_d = p.RunPeriodId;

	-- inserisce i mezzi sui periodi
	INSERT INTO dbo.RunCars
			(RunCarId, RunPeriodId,
			AssociateId, CarId, DriverId,
			CarType, Note)
		SELECT m.New_Id, mp.New_Id,
				rc.AssociateId, rc.CarId, DriverId,
				rc.CarType, rc.Note
			FROM dbo.RunCars rc
			INNER JOIN @Tbl_Map m
				ON m.NaoType = 'CAR'
				AND rc.RunCarId = m.OldI_d
			INNER JOIN @Tbl_Map mp
				ON mp.NaoType = 'PER'
				AND mp.OldI_d = rc.RunPeriodId
			WHERE (@Inc_Replacements = 1
					OR rc.CarType <> 'R');

	-- inserisce i costi sui mezzi
	INSERT INTO dbo.RunCarCosts
			(RunCarCostId, RunCarId,
			StartDAte,
			KmPrice, KmPriceExtra,
			DayPrice, DayIntegration, DayForfait
			)
		SELECT m.New_Id, mrc.New_Id,
				rcc.StartDAte,
				rcc.KmPrice, rcc.KmPriceExtra,
				rcc.DayPrice, rcc.DayIntegration, rcc.DayForfait
			FROM dbo.RunCarCosts rcc
			INNER JOIN @Tbl_Map m
				ON m.NaoType = 'COST'
				AND rcc.RunCarCostId = m.OldI_d
			INNER JOIN dbo.RunCars rc
				ON rc.RunCarId = rcc.RunCarId
			INNER JOIN @Tbl_Map mrc
				ON mrc.NaoType = 'CAR'
				AND mrc.OldI_d = rcc.RunCarId
			WHERE (@Inc_Replacements = 1
					OR rc.CarType <> 'R');

	-- inserisce le sostituzioni dei mezzi
	IF @Inc_Replacements = 1
	BEGIN
		-- inserisce le sostituzioni dei mezzi
		INSERT INTO dbo.RunCarReplacements
				(CarReplacementId, RunPeriodId,
				StartDate, EndDate, Note)
			SELECT m.New_Id, mp.New_Id,
					cr.StartDate, cr.EndDate, cr.Note
				FROM dbo.RunCarReplacements cr
				INNER JOIN @Tbl_Map m
					ON m.NaoType = 'REPL'
					AND cr.CarReplacementId = m.OldI_d
				INNER JOIN @Tbl_Map mp
					ON mp.NaoType = 'PER'
					AND mp.OldI_d = cr.RunPeriodId
					;
		-- inserisce il dettaglio delle sostituzioni
		INSERT INTO dbo.RunCarReplacementDetails
				(CarReplacementId,
				OriginaRunCarId, ReplacedRunCarId)
			SELECT m.New_Id, 
					m_o.New_Id, m_r.New_Id
				FROM dbo.RunCarReplacementDetails cr
				INNER JOIN @Tbl_Map m
					ON m.NaoType = 'REPL'
					AND cr.CarReplacementId = m.OldI_d
				INNER JOIN @Tbl_Map m_o
					ON m_o.NaoType = 'CAR'
					AND cr.OriginaRunCarId = m_o.OldI_d
				INNER JOIN @Tbl_Map m_r
					ON m_r.NaoType = 'CAR'
					AND cr.ReplacedRunCarId = m_r.OldI_d
					;
	END

	-- inserisce le sospensioni
	IF @Inc_Suspensions = 1
	BEGIN
		-- inserisce le sospensioni
		INSERT INTO dbo.RunSuspensions 
				(RunSuspensionId, RunId,
				StartDate, EndDate, 
				SuspensionNote, SuspensionTypeId
				)
			SELECT NEWID(), @NewRunId, 
					s.StartDate, s.EndDate,
					s.SuspensionNote, s.SuspensionTypeId
				FROM dbo.RunSuspensions s
				WHERE s.RunId = @RunId;
	END

	--- inserisce i giorni addizionali
	IF @Inc_AdditionalDays = 1
	BEGIN
		-- inserisce i giorni addizionali
		INSERT INTO dbo.RunAdditionalDays 
				(RunId,  Day, Note)
			SELECT @NewRunId, d.Day, d.Note
				FROM dbo.RunAdditionalDays d
				WHERE d.RunId = @RunId;
	END

	--- inserisce i giorni effettivi elastibus
	IF @Inc_ElastibusDays = 1
	BEGIN
		-- inserisce i giorni elastibus
		INSERT INTO dbo.RunElastibusDays 
				(RunId,  Day, Km, PeopleCount,  Note)
			SELECT @NewRunId, d.Day, d.Km, d.PeopleCount, d.Note
				FROM dbo.RunElastibusDays d
				WHERE d.RunId = @RunId;
	END

	-- inserisce le etichette
	IF @Inc_Tabs = 1
	BEGIN
		-- inserisce le etichette
		INSERT INTO dbo.RunTags 
				(RunId,  TagId)
			SELECT @NewRunId, t.TagId
				FROM dbo.RunTags t
				WHERE t.RunId = @RunId;
	END

	-- inserisce i forfait
	IF @Inc_Forfaits = 1
	BEGIN
		-- inserisce i forfait
		INSERT INTO dbo.MultiRunForfaitDetails 
				(RunId,  ForfaitId)
			SELECT @NewRunId, mfd.ForfaitId
				FROM dbo.MultiRunForfaitDetails mfd
				WHERE mfd.RunId = @RunId;
	END

	-- inserisce ke note interne
	IF @Inc_InternalNotes = 1
	BEGIN
		-- inserisce ke note interne
		INSERT INTO [dbo].[RunInternalNotes]
			(RunId, Note)
			SELECT @NewRunId, n.Note
				FROM [dbo].[RunInternalNotes] n
				WHERE n.RunId = @RunId;
	END
	
	-- aggiunge la corsa allla lista diq eulel che necessitano un ricalcolo
	INSERT INTO dbo.Runs_NeedsDayRecalc
		(RunId)
		VALUES (@NewRunId);

	COMMIT;

	RETURN 0
END
