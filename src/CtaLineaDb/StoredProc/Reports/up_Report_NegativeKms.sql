/* *************************************************************
	Maurizio Battisti
	16/10/2023
	Estrazione dei Km negativi
************************************************************* */
CREATE PROCEDURE [dbo].[up_Report_NegativeKms]
	@ContractId			int = NULL,
	@StartDate			date = NULL,
	@EndDate			date = NULL,
	@ConsiderSuspended	bit = 0
AS
BEGIN
	IF @ContractId IS NULL SET @ContractId = [dbo].[fn_Session_GetContractId] ();
	IF @StartDate IS NULL SET @StartDate = dbo.fn_GetStartDate( COALESCE(@EndDate, GetDATE()) );
	IF @EndDate IS NULL  SET @EndDate = dbo.fn_GetEndtDate( COALESCE(@StartDate, GetDATE()) );
	IF @ConsiderSuspended IS NULL  SET @ConsiderSuspended = 0;

	-- si assicura che tutti i dati da ricalcolare siano ricalcolati
	EXEC [dbo].[uo_RecalcRunDays_Massive] 0, NULL;

	WITH CTE_Suspensions AS
	(
		SELECT DISTINCT s.RunId
			FROM dbo.RunSuspensions s
			WHERE s.StartDate <= @EndDate
				AND s.EndDate >= @StartDate
				AND @ConsiderSuspended = 1
	), CTE_Runs AS
	(
		SELECT r.RunId
			FROM dbo.Runs r
			LEFT JOIN CTE_Suspensions s
				ON r.RunId = s.RunId
			WHERE (r.EndDate < @EndDate
					OR s.RunId IS NOT NULL)
				AND r.Extra = 0
				AND r.ContractId = @ContractId			
	), CTE_Data AS
	(
		SELECT r.RunId, 
				COUNT(d.Day) AS DayCount,				
				SUM(CASE WHEN d.OutOfPeriod = 1 OR (@ConsiderSuspended = 1 AND d.Suspended = 1) THEN 1 ELSE 0 END ) AS NotWorkedDayCount,
				SUM(CASE WHEN d.OutOfPeriod = 0 AND (@ConsiderSuspended = 0 OR d.Suspended = 0) THEN 1 ELSE 0 END ) AS WorkedDayCount,
				SUM(CASE WHEN d.OutOfPeriod = 0 AND (@ConsiderSuspended = 0 OR d.Suspended = 0) THEN v.Km - fv.Km ELSE 0 END ) AS KmExtra
			FROM CTE_Runs r
			INNER JOIN dbo.RunDays d
				ON d.RunId = r.RunId
			INNER JOIN dbo.RunVariations fv
				ON fv.RunId = r.RunId
				AND fv.StartDate IS NULL
			INNER JOIN Dbo.RunVariations v
				ON v.RunVariationId = d.RunVariationId
			WHERE d.CarNum = 1								
				AND d.Day BETWEEN @StartDate AND @EndDate
			GROUP BY r.RunId
	)
	SELECT r.RunId,
			r.CtaRunId,
			r.EndDate,
			a.AssociatesDescr AS PrimaryAssociate,
			r.RunName,
			v.LineNumber,
			v.RunNumber,			
			p.Path AS PathDescription,
			COALESCE(v.Km, 0) AS DayKm, 
			COALESCE(d.DayCount,0) AS DayCount,
			COALESCE(d.WorkedDayCount, 0) AS WorkedDayCount,
			COALESCE(d.NotWorkedDayCount, 0) AS NotWorkedDayCount,
			
			COALESCE(d.WorkedDayCount * v.Km, 0) AS ContractKmTotal,
			COALESCE(d.NotWorkedDayCount * v.Km, 0) AS CetConttWorkedKm,
			COALESCE(d.NotWorkedDayCount * v.Km, 0) AS ContractNegativeKm,
			COALESCE(d.KmExtra, 0) AS KmExtra
		FROM dbo.Runs r
		INNER JOIN CTE_Data d
			ON r.RunId = d.RunId
		INNER JOIN dbo.RunVariations v
			ON r.RunId = v.RunId
			AND v.StartDate IS NULL
		INNER JOIN dbo.vw_RunAssociates a
			ON a.RunId =r.RunId
		INNER JOIN dbo.vw_PathByNodes p
			ON p.RunVariationId =v.RunVariationId
			;

	RETURN 0;
END
