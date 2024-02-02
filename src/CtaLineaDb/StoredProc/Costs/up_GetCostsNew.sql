/* ******************************************************************************************
	Maurizio Battisti
	28/05/2023
	calcola il dettaglio dei costi prendendo in considerazione i parametri indicati
****************************************************************************************** */
CREATE PROCEDURE [dbo].[up_GetCosts_New]
(
	@ContractId		int = NULL,
	@StartDate		Date = NULL,
	@EndDate		Date = NULL,
	@AssociateId	uniqueidentifier = NULL,
	@CarId			uniqueidentifier = NULL,
	@RunId			uniqueidentifier = NULL,
	@OutOfPEriod	BIT = 0,
	@Suspended		BIT = 0,
	@RplacedCars	BIT = 1,
	@BudgetName		varchar(MAX) = NULL,
	@BudgetType		varchar(10) = NULL,

	@SimulationName	varchar(50) = NULL
)
AS
BEGIN
	SET DATEFIRST 1; -- this sets Monday to the first day of the week for the current connection.

	-- indic ache il modo di calcolare il peso deve  tenere conto della capacità richiesta da tT
	DECLARE @WeightCapacity	bit = 1;

	-- si assicura che tutti i dati da ricalcolare siano ricalcolati
	EXEC [dbo].[uo_RecalcRunDays_Massive] 0, @RunId;

	DECLARE @Tbl_OutPut AS TABLE 
	(
		RunId			uniqueidentifier,
		-- la riga di riferimetno per lacorsa in modo da poter avere un filtor sui dati globalid i  corsa
		RunRowNum		int,
		AssociateId		uniqueidentifier,
		CarId			uniqueidentifier,
		Day				date,
		
		-- ulteriroi dati per filtor
		ContractId		int,
		RunVariationId	uniqueidentifier,

		-- dati per i forfait  multi run
		MonthId			int,
		YearId			int,

		-- data
		RealKm_Contract	real,
		RealKm_Extra	real,
		ContractKm		real,
		ExtraKm			real,

		-- cposti
		RealKm_Contract_Cost	money,
		RealKm_Extra_Cost		money,
		DayCost					money,
		DayIntegration			money,
		DayForfait				money,

		-- forfait multi corsa
		MultiRunForfait			money,
		MultiRunForfaitAmount	money,
		MultiRunForfaitType		Char(1),
		MultiRunForfaitName		Varchar(MAX),
		MultiRunForfaitId		int,

		PRIMARY KEY (RunId, Day, RunRowNum)
	);

	INSERT INTO @Tbl_OutPut 
		(RunId, RunRowNum, Day,
		ContractId, RunVariationId,
		AssociateId, CarId,
		MonthId, YearId,
		RealKm_Contract, RealKm_Extra,
		RealKm_Contract_Cost, RealKm_Extra_Cost,
		DayCost, DayIntegration, DayForfait
		)
		SELECT DISTINCT c.RunId,
				c.CarNum,
				c.Day,
				r.ContractId,
				c.RunVariationId,
				(CASE @RplacedCars  
						WHEN 1 THEN  rc.AssociateId
						WHEN 0 THEN o_rc.AssociateId
					END) AS AssociateId,
				(CASE @RplacedCars  
						WHEN 1 THEN  rc.CarId
						WHEN 0 THEN o_rc.CarId
					END) AS CarId,

				dbo.fn_GetMonthId(c.Day),
				dbo.fn_GetYearId(c.Day),

				c.KmContract,
				c.KmExtra,

				-- Costi
				c.KmContract * 
					(CASE @RplacedCars  
						WHEN 1 THEN  c.KmPrice
						WHEN 0 THEN c.o_KmPrice
					END) AS RealKm_Contract_Cost,
				c.KmExtra * 
					(CASE @RplacedCars  
						WHEN 1 THEN  c.KmPriceExtra
						WHEN 0 THEN c.o_KmPriceExtra
					END) AS RealKm_Extra_Cost,
				(CASE @RplacedCars  
						WHEN 1 THEN  c.DayPrice
						WHEN 0 THEN c.o_DayPrice
				END) AS DayCost,
				(CASE @RplacedCars  
						WHEN 1 THEN  c.DayIntegration
						WHEN 0 THEN c.o_DayIntegration
				END) AS DayIntegration,
				(CASE @RplacedCars  
						WHEN 1 THEN  c.DayForfait
						WHEN 0 THEN c.o_DayForfait
				END) AS DayForfait

			FROM [dbo].[tvf_Costs](@SimulationName)  c
			INNER JOIN dbo.RunDays d
				ON c.RunId = d.RunId
				AND c.CarNum = d.CarNum
				AND c.Day = d.Day
			INNER JOIN dbo.Runs r
				ON c.RunId = r.RunId
			LEFT JOIN dbo.RunCars rc
				ON c.RunCarId = rc.RunCarId
			LEFT JOIN dbo.RunCars o_rc
				ON c.RunCarId = o_rc.RunCarId
			WHERE (d.Suspended = 0
					OR d.Suspended = @Suspended)
				AND (d.OutOfPeriod = 0
					OR d.OutOfPeriod =@OutOfPEriod)
				AND (@RunId IS NULL
					OR c.RunId = @RunId)
				AND (@ContractId IS  NULL
					OR r.ContractId = @ContractId)
				AND (@AssociateId IS NULL						
					OR (@RplacedCars = 1 AND rc.AssociateId = @AssociateId)
					OR (@RplacedCars = 0 AND o_rc.AssociateId = @AssociateId)
					)
				AND (@CarId IS NULL
					OR (@RplacedCars = 1  AND rc.CarId = @CarId)
					OR (@RplacedCars = 0  AND o_rc.CarId = @CarId)
					)
				AND (@StartDate IS NULL
					OR c.Day >= @StartDate)
				AND (@EndDate IS NULL
					OR c.Day <= @EndDate)
			;

	-- aggiorna i chilometri da notiifcare alla PAT per i calcoli dei km percorsi
	WITH CTE_Data AS
	(
		SELECT t.RunId, t.Day, t.RunRowNum,
				t.RealKm_Contract, t.RealKm_Extra
			fROM @Tbl_OutPut t
			WHERE t.RunRowNum  = 1
	)
	UPDATE @Tbl_OutPut
		SET ContractKm = t1.RealKm_Contract,
			ExtraKm = t1.RealKm_Extra
		FROM @Tbl_OutPut t
		INNER JOIN  CTE_Data t1
			ON t.RunId = t1.RunId
			AND t.Day = t1.Day
			AND t.RunRowNum = t1.RunRowNum
		;
	
	-- calcola il forfait multi corsa se necessario
	UPDATE @Tbl_OutPut 
		SET MultiRunForfaitName = f.ForfaitName,
			MultiRunForfaitAmount = f.Amount,
			MultiRunForfaitType = f.ForfaitType,
			MultiRunForfaitId = f.ForfaitId
		FROM @Tbl_OutPut  c
		INNER JOIN dbo.MultiRunForfaitDetails d
			ON c.RunId = d.RunId
		INNER JOIN dbo.MultiRunForfait f
			ON d.ForfaitId = f.ForfaitId
	
	-- calcola la quota aprte del forfait sul mezzo / consorziato /  corsa /  girono
	BEGIN
		-- forafit giornalieri
		DECLARE @Tbl_MRForfait_byDAy AS TABLE
		(
			RunId		uniqueidentifier NOT NULL,
			Day			date NOT NULL,
			ForfaitId	int NOT NULL,
			RunValue	money,

			PRIMARY KEY (RunId, Day, ForfaitId)
		);

		-- Gestisce i forfait giornalieri
		INSERT INTO @Tbl_MRForfait_byDAy 
			(RunId, Day, ForfaitId, RunValue)
			SELECT t.RunId, t.Day, 
					t.MultiRunForfaitId, 
					t.MultiRunForfaitAmount / f.TotalWeight *
					(rv.Km * 
					CASE WHEN @WeightCapacity = 0 THEN 1
						WHEN rv.RequestedCapacity <= 0 THEN 1
						ELSE rv.RequestedCapacity
					END) as sghei
				FROM @Tbl_OutPut t
				INNER JOIN dbo.RunVariations rv
					ON t.RunId = rv.RunId
					AND rv.StartDate IS NULL
				INNER JOIN [dbo].[tvf_DayForfaitWeight] (
						@StartDate, @EndDate,
						@OutOfPEriod, @Suspended, @RplacedCars,
						@WeightCapacity) as f
					ON f.ForfaitId = t.MultiRunForfaitId
					AND f.Day = t.Day
				WHERE t.RunRowNum = 1
					AND t.MultiRunForfaitType = 'D';
			
		-- Gestisce i forfait giornalieri
		INSERT INTO @Tbl_MRForfait_byDAy 
			(RunId, Day, ForfaitId, RunValue)
			SELECT t.RunId, t.Day, 
					t.MultiRunForfaitId, 
					t.MultiRunForfaitAmount / f.TotalWeight *
					(rv.Km * 
					CASE WHEN @WeightCapacity = 0 THEN 1
						WHEN rv.RequestedCapacity <= 0 THEN 1
						ELSE rv.RequestedCapacity
					END) as sghei
				FROM @Tbl_OutPut t
				INNER JOIN dbo.RunVariations rv
					ON t.RunId = rv.RunId
					AND rv.StartDate IS NULL
				INNER JOIN [dbo].[tvf_MonthForfaitWeight] (
						@StartDate, @EndDate,
						@OutOfPEriod, @Suspended, @RplacedCars,
						@WeightCapacity) as f
					ON f.ForfaitId = t.MultiRunForfaitId
					AND f.MonthId = t.MonthId
				WHERE t.RunRowNum = 1
					AND t.MultiRunForfaitType = 'M';
			
		-- calcola i forfait Annuali
		INSERT INTO @Tbl_MRForfait_byDAy 
			(RunId, Day, ForfaitId, RunValue)
			SELECT t.RunId, t.Day, 
					t.MultiRunForfaitId, 
					t.MultiRunForfaitAmount / f.TotalWeight *
					(rv.Km * 
					CASE WHEN @WeightCapacity = 0 THEN 1
						WHEN rv.RequestedCapacity <= 0 THEN 1
						ELSE rv.RequestedCapacity
					END) as sghei
				FROM @Tbl_OutPut t
				INNER JOIN dbo.RunVariations rv
					ON t.RunId = rv.RunId
					AND rv.StartDate IS NULL
				INNER JOIN [dbo].[tvf_YearForfaitWeight] (
						@StartDate, @EndDate,
						@OutOfPEriod, @Suspended, @RplacedCars,
						@WeightCapacity) as f
					ON f.ForfaitId = t.MultiRunForfaitId
					AND f.YearId = t.YearId
				WHERE t.RunRowNum = 1
					AND t.MultiRunForfaitType = 'Y';

		-- assegna la quota parte di ogni singolo mezzo 
		wITH CTE_AllForfaitsRow AS
		(
			SELECT t.RunId, t.Day, t.MultiRunForfaitId,
					SUM(c.NrSittings) TotalSittings
				FROM @Tbl_OutPut t
				INNER JOIN dbo.Cars c
					ON t.CarId = c.CarId
				WHERE t.MultiRunForfaitId IS NOT NULL
				GROUP BY t.RunId, 
					t.Day, 
					t.MultiRunForfaitId
		)
		UPDATE @Tbl_OutPut
			SET MultiRunForfait = fv.RunValue 
					/ (CASE WHEN tot.TotalSittings < 1 THEN 1 ELSE tot.TotalSittings END)
					* (CASE WHEN c.NrSittings  < 1 THEN 1 ELSE c.NrSittings  END)
			FROM @Tbl_OutPut d
			INNER JOIN @Tbl_MRForfait_byDAy fv
				ON d.RunId = fv.RunId
				AND d.Day = fv.Day
				AND d.MultiRunForfaitId = fv.ForfaitId
			INNER JOIN CTE_AllForfaitsRow tot
				ON fv.RunId = tot.RunId
				AND fv.Day = tot.Day
			INNER JOIN dbo.Cars c
				ON d.CarId = c.CarId;
	END

	IF @BudgetName IS NOT NULL 
	BEGIN
		DECLARE @BudgetId int;
		SET @BudgetId = 0;

		BEGIN TRAN;

		-- se il tipo è uno dei tipi spcieli
		-- recupera l'id se presetne  e  aggiorna i dati anzichè inserirrli
		IF @BudgetType IN ('LAST CALC')
		BEGIN
			SELECT @BudgetId = BudgetId
				FROM dbo.Budgets 
				WHERE BudgetType = @BudgetType;
			IF @BudgetId IS NULL SET @BudgetId = 0;
		END

		IF @BudgetId = 0 
		BEGIN
			-- salva il preventivo
			INSERT INTO dbo.Budgets 
				(BudgetName, BudgetType,
				ContractId, StartDate, EndDate,
				AssociateId, CarId, RunId,
				OutOfPEriod, Suspended, RplacedCars
				)
				VALUES (@BudgetName, @BudgetType,
					@ContractId, @StartDate, @EndDate,
					@AssociateId, @CarId, @RunId,
					@OutOfPEriod, @Suspended, @RplacedCars
					);
		
			SELECT @BudgetId = @@IDENTITY;
		END 
		ELSE
		BEGIN
			UPDATE dbo.Budgets 
				SET BudgetName = @BudgetName,
					ContractId = @ContractId, StartDate = StartDate, EndDate = EndDate,
					AssociateId = @AssociateId, CarId = @CarId, RunId = @RunId,
					OutOfPEriod = @OutOfPEriod, Suspended = @Suspended, RplacedCars = @RplacedCars
			WHERE BudgetId  = @BudgetId;

			-- elimina tutta la situazione corrente
			DELETE FROM dbo.BudgetDetail WHERE BudgetId  = @BudgetId; 
		END

		-- inserisce il dettaglio
		INSERT INTO dbo.BudgetDetail
			(BudgetId,
			RunId, RunRowNum, Day,
			AssociateId, CarId, ContractId,
			RealKm_Contract, RealKm_Extra, 
			ContractKm, ExtraKm,
			RealKm_Contract_Cost, RealKm_Extra_Cost,
			DayCost	, DayIntegration, DayForfait,
			MultiRunForfait, MultiRunForfaitAmount,
			MultiRunForfaitType, MultiRunForfaitName, MultiRunForfaitId
			)
			SELECT @BudgetId,
					RunId, RunRowNum, Day,
					AssociateId, CarId, ContractId,
					RealKm_Contract, RealKm_Extra, 
					ContractKm, ExtraKm,
					RealKm_Contract_Cost, RealKm_Extra_Cost,
					DayCost	, DayIntegration, DayForfait,
					MultiRunForfait, MultiRunForfaitAmount,
					MultiRunForfaitType, MultiRunForfaitName, MultiRunForfaitId
				FROM @Tbl_OutPut;

		COMMIT;
	END

	SELECT [RunId]
		  ,[RunRowNum]
		  ,[Day]
		  ,[AssociateId]
		  ,[CarId]
		  ,[ContractId]
		  ,[RealKm_Contract]
		  ,[RealKm_Extra]
		  ,[ContractKm]
		  ,[ExtraKm]
		  ,[RealKm_Contract_Cost]
		  ,[RealKm_Extra_Cost]
		  ,[DayCost]
		  ,[DayIntegration]
		  ,[DayForfait]
		  ,[MultiRunForfait]
		  ,[MultiRunForfaitAmount]
		  ,[MultiRunForfaitType]
		  ,[MultiRunForfaitName]
		  ,[MultiRunForfaitId]
		FROM @Tbl_OutPut;
		
	RETURN 0;
END
