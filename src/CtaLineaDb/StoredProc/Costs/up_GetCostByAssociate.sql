/* ******************************************************************************************
	Maurizio Battisti
	11/06/2023
	Estrazione dei costi divisi per consorziato
	
	* Sommatoria raggruppata per
	- Ditta
	- mezzo
	- Appalto
	- mese
****************************************************************************************** */
CREATE PROCEDURE [dbo].[up_GetCostByAssociate]
(
	@BudgetId		int = NULL,
	@ContractId		int = NULL,
	@StartDate		Date = NULL,
	@EndDate		Date = NULL,
	@AssociateId	uniqueidentifier = NULL,
	@CarId			uniqueidentifier = NULL,
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

	DECLARE @UseFreshData	 bit = 1;
	
	IF @OutOfPEriod =0
		AND @Suspended = 0
		AND @RplacedCars = 1
		AND @BudgetId IS NULL
		AND @BudgetName IS NULL
	BEGIN
		SELECT @BudgetId = BudgetId FROM dbo.Budgets WHERE BudgetType = 'LAST CALC';
	END
	
	IF @BudgetId IS NOT NULL
	BEGIN
		SET @UseFreshData = 0;
	END
	
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
			EXEC [dbo].[up_GetCosts_New] @ContractId, @StartDate, @EndDate,
				@AssociateId, @CarId,
				NULL,
				@OutOfPEriod, @Suspended, @RplacedCars,
				@BudgetName, @BudgetType,
				@SimulationName;
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
		WHERE d.BudgetId = @BudgetId
			AND (@ContractId IS NULL
				OR d.ContractId = @ContractId)
			AND (@AssociateId IS NULL
				OR d.AssociateId = @AssociateId)
			AND (@CarId IS NULL
				OR d.CarId = @CarId)
			AND (@StartDate IS NULL
				OR d.Day >= @StartDate)
			AND (@EndDate IS NULL
				OR d.Day <= @EndDAte)
			;
	END;

	WITH CTE_Data AS
	(
		SELECT d.AssociateId,
			  d.CarId,
			  d.ContractId,
			  [dbo].[fn_GetMonthId](d.Day) AS Month,
			  
			  SUM(d.RealKm_Contract) AS Km,
			  SUM(d.RealKm_Extra) AS KmExtra,
			  SUM(d.DayCost) AS DayCost, 
			  SUM(d.RealKm_Contract_Cost) AS CostKm,
			  SUM(d.RealKm_Extra_Cost) AS CostKmExtra,
			  
			  SUM(d.DayIntegration) AS DayIntegration, 
			  SUM(d.DayForfait) AS DayForfait, 
			  SUM(d.MultiRunForfait) AS DayMultiRunForfait
			  
			FROM @Tbl_OutPut d
			GROUP BY  d.AssociateId,
			  d.CarId,
			  d.ContractId,
			  [dbo].[fn_GetMonthId](d.Day)
		)	
		SELECT d.*,
		  a.Description AS AssociateName,
		  c.Description AS CarDescription,
		  ct.ContractName AS ContractName
		FROM CTE_Data AS d
		INNER JOIN dbo.Associates a
			ON a.AssociateId = d.AssociateId
		INNER JOIN dbo.Cars c
			ON c.CarId = d.CarId
		INNER JOIN dbo.Contracts ct
			ON ct.ContractId = d.ContractId
		;
		
	RETURN 0;
END
