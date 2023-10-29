/* ***************************************************************************************
	Maurizio Battisti
	28/10/2023
	recupera i  costi per la dashboard n con le stesse regole dei chilometri
*************************************************************************************** */
CREATE PROCEDURE [dbo].[uo_Dashboard_GetCosts]
	@ContractId			int = NULL,
	@StartDate			date = NULL,
	@EndDate			date = NULL,
	@ConsiderSuspended	bit = 0,
	@RealElastibusKm	bit = 0,
	@SimulationName		varchar(50) = NULL
WITH RECOMPILE
AS
BEGIN
	IF @ContractId IS NULL SET @ContractId = [dbo].[fn_Session_GetContractId] ();
	IF @StartDate IS NULL SET @StartDate = dbo.fn_GetStartDate( COALESCE(@EndDate, GetDATE()) );
	IF @EndDate IS NULL  SET @EndDate = dbo.fn_GetEndtDate( COALESCE(@StartDate, GetDATE()) );
	IF @ConsiderSuspended IS NULL  SET @ConsiderSuspended = 0;
	IF @RealElastibusKm IS NULL SET @RealElastibusKm = 0;
	IF @SimulationName = '' SET @SimulationName= NULL;

	-- si assicura che tutti i dati da ricalcolare siano ricalcolati
	EXEC [dbo].[uo_RecalcRunDays_Massive] 0, NULL;

	WITH CTE_Base AS
	(
		SELECT r.RunId,
				r.Extra,
				r.Elastibus,
				fv.RunVariationId ,
				fv.Km
			FROM dbo.Runs r
			INNER JOIN dbo.RunVariations fv
				ON r.RunId = fv.RunId
				AND fv.StartDate IS NULL
			WHERE r.ContractId = @ContractId
	), CTE_Costs AS
	(
		SELECT cc.RunCarId,
				COALESCE(cc.StartDAte, '19000101') AS StartDate,
				cc.RunCarCostId,
				cc.KmPrice, cc.KmPriceExtra,
				cc.DayPrice, cc.DayForfait, cc.DayIntegration,
				Row_NUMBER() OVER (PARTITION BY cc.RunCarId ORDER BY cc.StartDAte ASC ) AS Num

			FROM dbo.RunCarCosts cc
			WHERE (@SimulationName IS NULL
					AND cc.SimulationName IS NULL)
				OR (@SimulationName IS NOT NULL
					AND cc.SimulationName = @SimulationName)
	), CTE_CostRanges AS
	(
		SELECT c1.*,
				CASE WHEN c2.StartDAte IS NULL 
						THEN '99991231'
					ELSE DATEADD(d, -1, c2.StartDAte ) 
				END AS EndDAte
			FROM CTE_Costs c1
			LEFT JOIN  CTE_Costs c2
				ON c1.RunCarId = c2.RunCarId
				AND c1.Num + 1  = c2.num 
	), CTE_ElastibusKms AS
	(
		SELECT r.RunId,
				e.Day,
				e.Km
			FROM CTE_Base r
			LEFT JOIN dbo.RunElastibusDays e
				ON e.RunId = r.RunId
				AND @RealElastibusKm = 1
			WHERE r.Elastibus = 1
	), CTE_Days AS
	(
		SELECT d.RunCarId,
				d.RunVariationId,				
				r.RunVariationId AS Org_RunVariationId,
				d.OutOfPeriod,
				d.Suspended,
				d.CarNum,
				d.Day,

				CONVERT (varchar(6),  d.Day, 112) AS MonthId,

				cc.DayPrice + COALESCE(cc.DayForfait, 0) + COALESCE(cc.DayIntegration, 0) AS DayCost,
				
				-- calcoal i chilometri reali
				CASE 
					WHEN r.Elastibus = 1
						AND @RealElastibusKm = 1
						AND r.Extra = 0
						THEN COALESCE( e.Km, 0) * cc.KmPrice
					WHEN r.Elastibus = 1
						AND @RealElastibusKm = 1
						AND r.Extra = 1
						THEN COALESCE( e.Km, 0) * cc.KmPriceExtra
					WHEN @RealElastibusKm =  0
						AND r.Extra = 1
						THEN COALESCE(v.Km, 0) * cc.KmPriceExtra
					ELSE (COALESCE(v.Km, 0) - COALESCE(r.km, 0) ) * cc.KmPriceExtra 
						+ COALESCE(r.km, 0) * cc.KmPrice
				END AS Real_Cost,

				--  calcola i chilometri da  contratto
				CASE
					WHEN r.Extra = 0
						THEN r.Km * cc.KmPrice
					ELSE 0
				END AS ContracCosts,
				-- calcola i chilometri a contratto lavorati
				CASE
					WHEN d.OutOfPeriod = 0 
						AND (@ConsiderSuspended =0
							OR d.Suspended = 0)
						AND r.Extra = 0
						THEN r.Km * cc.KmPrice
					ELSE 0
				END AS WorkedContractCost,
				-- calcola i chilometri negativi
				CASE
					WHEN d.OutOfPeriod = 1 
						OR (@ConsiderSuspended =1
							AND d.Suspended = 1)
						AND r.Extra = 0
						THEN r.Km *cc.KmPrice
					ELSE 0
				END AS NegativeContractCost,

				-- calcola i chilometri extra captiolato per il giorno in questione
				CASE 
					WHEN d.OutOfPeriod = 0 
						AND (@ConsiderSuspended =0
							OR d.Suspended = 0)
						AND r.Extra = 1 
						THEN v.km * cc.KmPriceExtra
					WHEN d.OutOfPeriod = 0 
						AND (@ConsiderSuspended =0
							OR d.Suspended = 0)
					THEN (v.Km - r.Km ) * cc.KmPriceExtra
					ELSE 0
				END AS ExtraCost
			FROM dbo.RunDays d
			INNER JOIN CTE_Base r
				ON r.RunId = d.RunId
			INNER JOIN dbo.RunVariations v
				ON d.RunVariationId = v.RunVariationId
			INNER JOIN CTE_CostRanges cc
				ON d.RunCarId = cc.RunCarId
				AND d.Day BETWEEN cc.StartDate AND cc.EndDAte
			LEFT JOIN CTE_ElastibusKms e
				ON d.RunId = e.RunId
				AND d.Day = e.Day
			WHERE d.Day BETWEEN @StartDate AND @EndDate
	)
	SELECT d.MonthId,
			sUM(d.DayCost) AS DayCost,
			sUM(d.Real_Cost) AS RealCost,			
			sUM(d.ContracCosts) AS ContracCosts,
			SUM(d.WorkedContractCost) AS WorkedContractCost,
			SUM(d.NegativeContractCost) AS NegativeContractCost,
			sUM(d.ExtraCost) AS ExtraCost
		FROM CTE_Days d
		WHERE d.CarNum = 1
		GROUP BY d.MonthId
		ORDER BY d.MonthId
		;
	
	RETURN 0;
END