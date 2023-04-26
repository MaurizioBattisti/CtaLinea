/* *****************************************************************************
    Maurizio Battisti
    16/04/2023
	cambia i mezzi della lista con i corrispondenti a aparitre da una certa data
	utilizzaba per
	- chiusura ditt
	- vendita mezzi
	- ecc
***************************************************************************** */
CREATE PROCEDURE [dbo].[up_ChangeCarsFromDate]
	@RefDate			Date = NULL,
	@CarMapList			Varchar(MAX)
AS
BEGIN
	DECLARE @DayBefore DATE;
	-- se la data è nulla prende la data di oggi
	IF @RefDate IS NULL SET @RefDate = GETDATE();
	SET @DayBefore = DATEADD(d, -1, @RefDate);

	DECLARE @Tbl_CarMap AS TABLE
	(
		Old_CarId		uniqueidentifier PRIMARY KEY,
		New_CarId		uniqueidentifier NOT NULL
	);
	
	DECLARE @Tmp AS TABLE (
		KeyValue	varchar(MAX)
	);
	-- crea la lista temporanea
	INSERT INTO @Tmp (KeyValue)
		SELECT value FROM STRING_SPLIT(@CarMapList, ',');;

	-- inserisce i valori dei mezzi
	INSERT INTO @Tbl_CarMap (Old_CarId, New_CarId)
		SELECT TRIM(SUBSTRING(t.KeyValue, 0, CHARINDEX ('=', t.KeyValue))),
			TRIM(SUBSTRING(t.KeyValue, 1 + CHARINDEX ('=', t.KeyValue), LEN(t.KeyValue)))
			FROM @Tmp t;

	-- Prepara le tabelle di lavoro
	DECLARE @Tbl_RunCars AS TABLE
	(
		RunCarId				uniqueidentifier PRIMARY KEY,
		RunPeriodId				uniqueidentifier,
		New_RunCarId			uniqueidentifier DEFAULT (NEWID()),
		DueToPeriodChange		bit DEFAULT 1
	);
	DECLARE @Tbl_RunPeriods AS TABLE
	(
		RunPeriodId				uniqueidentifier PRIMARY KEY,
		New_RunPeriodId			uniqueidentifier DEFAULT (NEWID())
	);
	DECLARE @Tbl_CarReplacments AS TABLE
	(
		CarReplacementId		uniqueidentifier PRIMARY KEY,
		RunPeriodId				uniqueidentifier,
		New_CarReplacementId	uniqueidentifier DEFAULT (NEWID())
	)
	BEGIN;
	-- primo riempimento della tabella dei Mezzi
	WITH CTE_Periods AS
	(
		SELECT rc.RunPeriodId
			FROM dbo.RunCars rc
			INNER JOIN dbo.RunPeriods rp
				ON rc.RunPeriodId = rp.RunPeriodId
			INNER JOIN dbo.Runs r
				ON rp.RunId = r.RunId
			INNER JOIN Dbo.Contracts c
				ON r.ContractId = c.ContractId
			INNER JOIN @Tbl_CarMap cm
				ON rc.CarId = cm.Old_CarId
			WHERE @RefDate BETWEEN COALESCE (rp.StartDate, r.StartDate,c.StartDate)
								AND COALESCE (rp.EndDate, r.EndDate,c.EndDate) 
	)
	INSERT INTO @Tbl_RunCars 
		(RunCarId, RunPeriodId)
		SELECT DISTINCT rc.RunCarId, rc.RunPeriodId
			FROM dbo.RunCars rc
			INNER JOIN CTE_Periods p
				ON rc.RunPeriodId = p.RunPeriodId
		;
	-- riempie la tabella dei rimpiazzi che hanno un mezzo da sostituire
	INSERT INTO @Tbl_CarReplacments (CarReplacementId, RunPeriodId)
		SELECT crd.CarReplacementId, cr.RunPeriodId
			FROM dbo.RunCarReplacementDetails crd
			INNER JOIN dbo.RunCarReplacements cr
				ON crd.CarReplacementId = cr.CarReplacementId
			INNER JOIN dbo.RunCars rc
				ON crd.ReplacedRunCarId = rc.RunCarId
			INNER JOIN dbo.RunPeriods rp
				ON rc.RunPeriodId = rp.RunPeriodId
			INNER JOIN dbo.Runs r
				ON rp.RunId = r.RunId
			INNER JOIN Dbo.Contracts c
				ON r.ContractId = c.ContractId
			INNER JOIN @Tbl_CarMap cm
				ON rc.CarId = cm.Old_CarId
			WHERE @RefDate BETWEEN COALESCE (cr.StartDate, rp.StartDate, r.StartDate,c.StartDate)
								AND COALESCE (cr.EndDate, rp.EndDate, r.EndDate,c.EndDate) 

	-- aggiunge i mezzi che vengono da rimpiazzi solo se  se devono essere creati nuovi
	INSERT INTO @Tbl_RunCars 
		(RunCarId, RunPeriodId, DueToPeriodChange)
		SELECT crd.ReplacedRunCarId, r.RunPeriodId, 0 AS DueToPeriodChange
			FROM dbo.RunCarReplacementDetails crd
			INNER JOIN @Tbl_CarReplacments r
				ON crd.CarReplacementId = r.CarReplacementId
			LEFT JOIN @Tbl_RunCars ex
				ON ex.RunCarId = crd.ReplacedRunCarId
			WHERE ex.RunCarId IS NULL
		;

	-- riempie la tabella di periodi da inserire
	INSERT INTO @Tbl_RunPeriods (RunPeriodId)
		SELECT DISTINCT c.RunPeriodId
			FROM @Tbl_RunCars c
			WHERE c.DueToPeriodChange = 1;
	END;

	BEGIN TRAN;
	-- esegue la duplicazione dei dati
	BEGIN;
		-- crea i nuovi periodi con la data di inizio impostata correttametne
		INSERT INTO dbo.RunPeriods 
			(RunId, RunPeriodId, 
			StartDate, EndDate,
			Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, 
			Note)
			SELECT rp.RunId, rp2.New_RunPeriodId,
					@RefDate, rp.EndDate,
					rp.Monday, rp.Tuesday, rp.Wednesday, rp.Thursday, rp.Friday, rp.Saturday, rp.Sunday, 
					rp.Note
				FROM dbo.RunPeriods rp
				INNER JOIN @Tbl_RunPeriods rp2
					ON Rp.RunPeriodId = rp2.RunPeriodId
			;
		-- crea i nuovi mezzi nei nei periodi
		INSERT INTO dbo.RunCars 
			(RunCarId, RunPeriodId, AssociateId, CarId, CarType, Note)
			SELECT rc_2.New_RunCarId, COALESCE(rp2.New_RunPeriodId, rc.RunPeriodId),
				rc.AssociateId, rc.CarId, 
				rc.CarType, rc.Note
				FROM dbo.RunCars rc
				INNER JOIN @Tbl_RunCars rc_2
					ON Rc.RunCarId = rc_2.RunCarId
				LEFT JOIN @Tbl_RunPeriods rp2
					ON rc_2.RunPeriodId = rp2.RunPeriodId
			;
		-- duplica i costi dei mezzi
		-- li duplica tutti anch quelli che probabilmente sono vecchi
		-- mettendo una data finta dove è nulla
		INSERT INTO dbo.RunCarCosts
			(RunCarCostId, RunCarId, 
			StartDAte, 
			KmPrice, DayPrice, KmPriceExtra,
			DayForfait, DayIntegration)
			SELECT NEWID(), c.New_RunCarId, 
				COALESCE(cc.StartDAte, '20000101'),
				cc.KmPrice, cc.DayPrice, cc.KmPriceExtra,
				cc.DayForfait, cc.DayIntegration
				FROM dbo.RunCarCosts cc
				INNER JOIN @Tbl_RunCars c
					ON cc.RunCarId =c.RunCarId
			;
		-- imposta la data più alta inferiore alla data di taglio a nullo sui costi appna duplicati
		WITH CTE_Cost AS
		(
			SELECT cc.RunCarId,
					MAX(cc.StartDAte ) AS maxDate
				FROM dbo.RunCarCosts cc
				INNER JOIN @Tbl_RunCars c
					ON cc.RunCarId = c.New_RunCarId
				WHERE cc.StartDAte <= @RefDate
				GROUP BY cc.RunCarId
		)
		UPDATE dbo.RunCarCosts
			SET StartDAte = NULL
			FROM dbo.RunCarCosts cc
			INNER JOIN CTE_Cost cc2
				ON cc.StartDAte = cc2.maxDate
				AND cc.RunCarId = cc2.RunCarId;
		-- elimina tutte le  altre date dei costi
		WITH CTE_CostToDEl AS
		(
			SELECT cc.RunCarCostId
				FROM dbo.RunCarCosts cc
				INNER JOIN @Tbl_RunCars cc2
					ON cc.RunCarCostId = cc2.New_RunCarId
				WHERE cc.StartDAte IS NOT NULL
					AND cc.StartDAte < @RefDate
		)
		DELETE FROM dbo.RunCarCosts
			WHERE RunCarCostId IN (SELECT RunCarCostId FROM CTE_CostToDEl);
			;

		-- duplica le sostituzioni
		INSERT INTO dbo.RunCarReplacements
			(CarReplacementId, RunPeriodId,
			StartDate, EndDate, Note)
			SELECT cr2.New_CarReplacementId,
				COALESCE(rp.New_RunPeriodId, cr2.RunPeriodId),
				@RefDate, cr.EndDate, cr.Note
				FROM dbo.RunCarReplacements cr
				INNER JOIN @Tbl_CarReplacments cr2
					ON cr.CarReplacementId = cr2.CarReplacementId
				LEFT JOIN @Tbl_RunPeriods rp
					ON cr2.RunPeriodId = rp.RunPeriodId
					;
		-- duplica il dettaglio delle sostituzioni sostituendo gli id dei mezzi con queli nuovi
		INSERT INTO dbo.RunCarReplacementDetails
			(CarReplacementId, OriginaRunCarId, ReplacedRunCarId)
			SELECT cr.NEw_CarReplacementId, 
					COALESCE(o_c.New_RunCarId, crd.OriginaRunCarId),
					COALESCE(n_c.New_RunCarId, crd.ReplacedRunCarId)
				FROM dbo.RunCarReplacementDetails crd
				INNER JOIN @Tbl_CarReplacments cr
					ON crd.CarReplacementId = cr.CarReplacementId
				LEFT JOIN @Tbl_RunCars O_c
					ON crd.OriginaRunCarId = o_c.RunCarId
				LEFT JOIN @Tbl_RunCars n_c
				ON crd.ReplacedRunCarId = n_c.RunCarId
				;
	END;

	-- modifica le date di fine
	BEGIN;
		-- modifica la data di fine dei vecchi periodi
		UPDATE dbo.RunPeriods
			SET EndDate = @DayBefore
			FROM dbo.RunPeriods rp
			INNER JOIN @Tbl_RunPeriods rp2
				ON rp.RunPeriodId = rp2.RunPeriodId;
		
		--- modifica la data di fine delle vecchie sostituzioni
		UPDATE dbo.RunCarReplacements
			SET EndDate = @DayBefore
			FROM dbo.RunCarReplacements cr
			INNER JOIN @Tbl_CarReplacments cr2
				ON cr.CarReplacementId = cr2.CarReplacementId;
	END;

	-- aggiorna i dati dei mezzi sostituiendo mezzo e ditta
	WITH CTE_RunCars AS
	(
		(
			SELECT DISTINCT rc.RunCarId
				FROM dbo.RunCars rc
				INNER JOIN @Tbl_RunCars rc_2
					ON rc.RunCarId = rc_2.New_RunCarId
		)
		UNION 
		(
			SELECT DISTINCT rc.RunCarId
				FROM dbo.RunCars rc
				INNER JOIN Dbo.RunPeriods rp
					ON Rc.RunPeriodId = rp.RunPeriodId
				WHERE Rp.StartDate >= @RefDate
		)
		UNION 
		(
			SELECT DISTINCT rc.RunCarId
				FROM dbo.RunCars rc
				INNER JOIN dbo.RunCarReplacementDetails crp
					ON crp.ReplacedRunCarId = rc.RunCarId
				INNER JOIN dbo.RunCarReplacements cr
					ON Crp.CarReplacementId = cr.CarReplacementId
				WHERE cr.StartDate >= @RefDate
		)
	)
	UPDATE dbo.RunCars 
		SET AssociateId =c.AssociateId,
			CarId = c.CarId
		FROM dbo.RunCars rc
		INNER JOIN CTE_RunCars rc_2
			ON rc.RunCarId = rc_2.RunCarId
		INNER JOIN @Tbl_CarMap cm
			ON cm.Old_CarId = rc.CarId
		INNER JOIN dbo.Cars c
			ON cm.New_CarId = c.CarId;
		
	-- esegue la pulizia dei dati
	BEGIN;
		-- Elimina tutti i periodi con la data di fine minore di qeulla di inizio
		DELETE FROM dbo.RunPeriods
			WHERE EndDate < StartDate;

		-- elimina tutte le sostituzioni con la data din fine minore di quella di inizio
		DELETE FROM dbo.RunCarReplacements
			WHERE EndDate < StartDate;

		-- elimina tutte le sostituzioni  che sono furi del periodo
		WITH CTE_CarRepl AS
		(
			SELECT cr.CarReplacementId
				FROM dbo.RunCarReplacements cr
				INNER JOIN dbo.RunPeriods rp
					ON cr.RunPeriodId = rp.RunPeriodId
				INNER JOIN dbo.Runs r
					ON r.RunId = r.RunId
				INNER JOIN dbo.Contracts c
					ON r.ContractId = c.ContractId
				WHERE COALESCE(cr.StartDate, rp.StartDate, r.StartDate, c.StartDate)
						> COALESCE(rp.EndDate, r.EndDate, c.EndDate)
					OR COALESCE(cr.EndDate, rp.EndDate, r.EndDate, c.EndDate)
						< COALESCE(rp.StartDate, r.StartDate, c.StartDate)
		)
		DELETE FROM dbo.RunCarReplacements
			WHERE CarReplacementId IN (SELECT CarReplacementId FROM CTE_CarRepl);
			
		-- elimina tutti i runcar di tipo R che non sono associati a nesuna sostituzione
		WITH CTE_RuncarsToDel AS
		(
			SELECT rc.RunCarId
				FROM Dbo.RunCars rc
				LEFT JOIN dbo.RunCarReplacementDetails crd
					ON crd.ReplacedRunCarId = rc.RunCarId
			WHERE rc.CarType = 'R'
				AND crd.CarReplacementId IS NULL
		)
		DELETE FROM dbo.RunCars
			WHERE RunCarId IN (SELECT RunCarID FROM CTE_RuncarsToDel);

		-- elimina tutti i costi dei mezzi originali che sono successivi alla data di riferimento		
		WITH CTE_CostToDEl AS 
		(
			SELECT cc.RunCarCostId
				FROM dbo.RunCarCosts cc
				INNER JOIN @Tbl_RunCars rc
					ON cc.RunCarId = rc.RunCarId
			WHERE cc.StartDAte > @RefDate
		)
		DELETE FROM dbo.RunCarCosts 
			WHERE RunCarCostId IN (SELECT RunCarCostId FROM CTE_CostToDEl);
	END;

	-- aggiorna la tabella delle corse che necessitano il ricalcolo
	WITH CTE_RunCars AS
	(
		(
		SELECT cc.RunCarId
			FROM @Tbl_RunCars cc
		)
		UNION
		(
		SELECT cc.New_RunCarId
			FROM @Tbl_RunCars cc
		)
	)
	INSERT INTO dbo.Runs_NeedsDayRecalc
		(RunId)
		SELECT DISTINCT rp.RunId
			FROM dbo.RunCarCosts rc
			INNER JOIN CTE_RunCars c
				ON rc.RunCarId = c.RunCarId
			INNER JOIN dbo.RunPeriods rp
				ON rc.RunCarCostId = rp.RunPeriodId;
	COMMIT;
	
	RETURN 0;
END