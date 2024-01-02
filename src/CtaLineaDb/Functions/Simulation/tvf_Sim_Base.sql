/* ********************************************************************
	Maurizio Battisti
	07/10/2023
	Base per la simulaizone
******************************************************************** */
CREATE FUNCTION [dbo].[tvf_Sim_Base]
(
	@SimulationName		varchar(50),
	@ContractId			int,
	@StartDate			Date,
	@EndDate			date
)
RETURNS @Tbl_Result TABLE
(
	[RunId]				UNIQUEIDENTIFIER NOT NULL, 
	[RunPeriodId]		UNIQUEIDENTIFIER NOT NULL, 
    [RunCarId]			UNIQUEIDENTIFIER NOT NULL, 
	[RunCarCostId]		UNIQUEIDENTIFIER NULL, 

    [DayPrice]			MONEY NOT NULL DEFAULT 0 , 
    [KmPrice]			MONEY NOT NULL DEFAULT 0 , 
    [KmPriceExtra]		MONEY NOT NULL DEFAULT 0 , 

    [DayIntegration]	MONEY NULL , 
    [DayForfait]		MONEY NULL, 
    
	[MinCDayost]		MONEY NULL , 
    [MaxDayCost]		MONEY NULL, 
    
    [SimulationName]	VARCHAR(50) NULL, 

	[Simulated]			bit DEFAULT 0
)
AS
BEGIN
	IF @StartDate IS NULL SET @StartDate = dbo.fn_GetStartDate( COALESCE(@EndDate, GetDATE()) );
	IF @EndDate IS NULL  SET @EndDate = dbo.fn_GetEndtDate( COALESCE(@StartDate, GetDATE()) );

	WITH CTE_Costs AS
	(
		SELECT cc.*,				
				ROW_NUMBER() OVER (PARTITION BY cc.RunCarId ORDER BY cc.SimulationName DESC, cc.StartDate DESC ) AS num
			FROM Dbo.RunCarCosts cc
			INNER JOIN dbo.RunCars rc
				ON cc.RunCarId = rc.RunCarId
			WHERE (cc.SimulationName IS NULL
					OR cc.SimulationName = @SimulationName
				)
				AND (cc.StartDAte IS NULL 
					OR cc.StartDAte <= @StartDate)
	)
	INSERT INTO @Tbl_Result 
			(RunId, RunPeriodId, RunCarId,
			RunCarCostId,
			KmPrice, KmPriceExtra,
			DayPrice, DayForfait, DayIntegration,
			MinCDayost, MaxDayCost,
			SimulationName, 
			Simulated)
		SELECT rp.RunId,
				rc.RunPeriodId,
				rc.RunCarId,
				CASE WHEN cc.SimulationName IS NULL THEN NULL ELSE cc.RunCarCostId END as RunCarCostId,
				COALESCE(cc.KmPrice, 0),
				COALESCE(cc.KmPriceExtra, 0),
				COALESCE(cc.DayPrice, 0),
				cc.DayForfait,
				cc.DayIntegration,
				cc.MinDayCost, cc.MaxDayCost,
				@SimulationName AS SimulationName,
				CASE WHEN cc.SimulationName IS NULL THEN 0 ELSE 1 END as Simulated
			FROM dbo.RunCars rc
			INNER JOIN dbo.RunPeriods rp
				ON rc.RunPeriodId = rp.RunPeriodId
			INNER JOIN dbo.Runs r 
				ON rp.RunId = r.RunId
			LEFT JOIN CTE_Costs cc
				ON rc.RunCarId = cc.RunCarId
				AND cc.num = 1
			WHERE rc.CarType IN ('P', 'R')
				AND (r.StartDate IS NULL
					OR r.StartDate <= @EndDate)
				AND (r.EndDate IS NULL
					OR r.EndDate >= @StartDate)
				AND (@ContractId IS NULL
					OR r.ContractId =  @ContractId)
			;

	RETURN;
END
