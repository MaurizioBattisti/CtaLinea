/* ******************************************************************************************
	Maurizio Battisti
	10/06/2023
	Calcoal i costi dvisi per corsa
****************************************************************************************** */
CREATE PROCEDURE [dbo].[up_GetCostByRun]
(
	@RunId			uniqueidentifier,
	@StartDate		Date = NULL,
	@EndDate		Date = NULL,
	@OutOfPEriod	BIT = 0,
	@Suspended		BIT = 0,
	@RplacedCars	BIT = 1
)
AS
BEGIN
	DECLARE @UseFreshData	 bit = 1;
	DECLARE @BudgetId		 int = NULL;
	
	IF @OutOfPEriod =0
		AND @Suspended = 0
		AND @RplacedCars = 1
		AND @BudgetId IS NULL
	BEGIN
		SELECT @BudgetId = BudgetId FROM dbo.Budgets WHERE BudgetType = 'LAST CALC';
	END
	
	IF @BudgetId IS NOT NULL
	BEGIN
		SET @UseFreshData = 0;
	END	
	-- se non viene passato nessun id per la corsa restituisce una lista vuota
	IF @RunId IS NULL SET @RunId = NEWID();

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

	IF @UseFreshData = 1
	BEGIN
		INSERT INTO @Tbl_OutPut 
			(RunId, RunRowNum, Day,
			AssociateId, CarId, ContractId, 
			RealKm_Contract, RealKm_Extra,
			ContractKm, ExtraKm,
			RealKm_Contract_Cost, RealKm_Extra_Cost,
			DayCost, DayIntegration, DayForfait, MultiRunForfait,
			MultiRunForfaitAmount, MultiRunForfaitType, 
			MultiRunForfaitName, MultiRunForfaitId
			)
			EXEC [dbo].[up_GetCosts_New] NULL, @StartDate, @EndDate,
				NULL, NULL,
				@RunId,
				@OutOfPEriod, @Suspended, @RplacedCars,
				NULL, NULL;
	END
	ELSE
	BEGIN
		INSERT INTO @Tbl_OutPut 
			(RunId, RunRowNum, Day,
			AssociateId, CarId, ContractId, 
			RealKm_Contract, RealKm_Extra,
			ContractKm, ExtraKm,
			RealKm_Contract_Cost, RealKm_Extra_Cost,
			DayCost, DayIntegration, DayForfait, MultiRunForfait,
			MultiRunForfaitAmount, MultiRunForfaitType, 
			MultiRunForfaitName, MultiRunForfaitId
			)
		SELECT d.RunId, d.RunRowNum, d.Day,
			d.AssociateId, d.CarId, d.ContractId, 
			d.RealKm_Contract, d.RealKm_Extra,
			d.ContractKm, d.ExtraKm,
			d.RealKm_Contract_Cost, d.RealKm_Extra_Cost,
			d.DayCost, d.DayIntegration, d.DayForfait, d.MultiRunForfait,
			d.MultiRunForfaitAmount, d.MultiRunForfaitType, 
			d.MultiRunForfaitName, d.MultiRunForfaitId
		FROM dbo.BudgetDetail d
		INNER JOIN dbo.Budgets b
			ON b.BudgetId = d.BudgetId
		WHERE d.BudgetId = @BudgetId
			AND d.RunId = @RunId
			AND (@StartDate IS NULL
				OR d.Day >= @StartDate)
			AND (@EndDate IS NULL
				OR d.Day <= @EndDAte)
			;
	END;

	WITH CTE_Data AS
	(
		SELECT d.Day,
			  d.AssociateId,
			  d.CarId,
			  
			  SUM(d.RealKm_Contract) AS Km,
			  SUM(d.RealKm_Extra) AS KmExtra,
			  SUM(d.DayCost) AS DayCost, 
			  SUM(d.RealKm_Contract * d.RealKm_Contract_Cost) AS CostKm,
			  SUM(d.RealKm_Extra * d.RealKm_Extra_Cost) AS CostKmExtra,
			  
			  SUM(d.DayIntegration) AS DayIntegration, 
			  SUM(d.DayForfait) AS DayForfait, 
			  SUM(d.MultiRunForfait) AS DayMultiRunForfait, 
			  
			  d.MultiRunForfaitName AS MultiRunForfaitName,
			  d.MultiRunForfaitType AS MultirunForfaitType,
			  d.MultiRunForfaitAmount AS MultiRunForfaitAmount
			FROM @Tbl_OutPut d
			GROUP BY  d.Day,
			  d.AssociateId,
			  d.CarId,
			  d.MultiRunForfaitName,
			  d.MultiRunForfaitType,
			  d.MultiRunForfaitAmount
		)	
		SELECT d.*,
		  a.Description AS AssociateName,
		  c.Description AS CarDescription
		FROM CTE_Data AS d
		INNER JOIN dbo.Associates a
			ON a.AssociateId = d.AssociateId
		INNER JOIN Dbo.Cars c
			ON c.CarId = d.CarId
		;
		
	RETURN 0;
END
