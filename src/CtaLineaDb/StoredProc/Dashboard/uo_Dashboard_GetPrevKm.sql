/* ***************************************************************************************
	Maurizio Battisti
	23/10/2023
	Restituisce la lista dei chilometir  previsti  mese  per mese
*************************************************************************************** */
CREATE PROCEDURE [dbo].[uo_Dashboard_GetPrevKm]
	@ContractId			int = NULL,
	@StartDate			date = NULL,
	@EndDate			date = NULL,
	@ConsiderSuspended	bit = 0,
	@RealElastibusKm	bit = 0
WITH RECOMPILE
AS
BEGIN
	IF @ContractId IS NULL SET @ContractId = [dbo].[fn_Session_GetContractId] ();
	IF @StartDate IS NULL SET @StartDate = dbo.fn_GetStartDate( COALESCE(@EndDate, GetDATE()) );
	IF @EndDate IS NULL  SET @EndDate = dbo.fn_GetEndtDate( COALESCE(@StartDate, GetDATE()) );
	IF @ConsiderSuspended IS NULL  SET @ConsiderSuspended = 0;
	IF @RealElastibusKm IS NULL SET @RealElastibusKm = 0;

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

				r.Km AS Contract_Km,
				
				-- calcoal i chilometri reali
				CASE 
					WHEN r.Elastibus = 1
						AND @RealElastibusKm = 1
						THEN COALESCE( e.Km, 0)
					ELSE v.Km
				END AS Real_Km,

				--  calcola i chilometri da  contratto
				CASE
					WHEN r.Extra = 0
						THEN r.Km
					ELSE 0
				END AS ContractKm,
				-- calcola i chilometri a contratto lavorati
				CASE
					WHEN d.OutOfPeriod = 0 
						AND (@ConsiderSuspended =0
							OR d.Suspended = 0)
						AND r.Extra = 0
						THEN r.Km
					ELSE 0
				END AS WorkedContractKm,
				-- calcola i chilometri negativi
				CASE
					WHEN d.OutOfPeriod = 1 
						OR (@ConsiderSuspended =1
							AND d.Suspended = 1)
						AND r.Extra = 0
						THEN r.Km
					ELSE 0
				END AS NegativeContractKm,

				-- calcola i chilometri extra captiolato per il giorno in questione
				CASE 
					WHEN d.OutOfPeriod = 0 
						AND (@ConsiderSuspended =0
							OR d.Suspended = 0)
						AND r.Extra = 1 
						THEN v.km 
					WHEN d.OutOfPeriod = 0 
						AND (@ConsiderSuspended =0
							OR d.Suspended = 0)
					THEN v.Km - r.Km 
					ELSE 0
				END AS ExtraKm
			FROM dbo.RunDays d
			INNER JOIN CTE_Base r
				ON r.RunId = d.RunId
			INNER JOIN dbo.RunVariations v
				ON d.RunVariationId = v.RunVariationId
			LEFT JOIN CTE_ElastibusKms e
				ON d.RunId = e.RunId
				AND d.Day = e.Day
			WHERE d.Day BETWEEN @StartDate AND @EndDate
	)
	SELECT d.MonthId,
			sUM(d.Real_Km) AS RealKm,
			sUM(d.ContractKm) AS ContractKm,
			SUM(d.WorkedContractKm) AS WorkedContractKm,
			SUM(d.NegativeContractKm) AS NegativeContractKm,
			sUM(d.ExtraKm) AS ExtraKm
		FROM CTE_Days d
		WHERE d.CarNum = 1
		GROUP BY d.MonthId
		ORDER BY d.MonthId
		;
	
	RETURN 0;
END