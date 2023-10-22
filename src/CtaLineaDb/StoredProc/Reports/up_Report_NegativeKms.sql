/* *************************************************************
	Maurizio Battisti
	16/10/2023
	Estrazione dei Km negativi
************************************************************* */
CREATE PROCEDURE [dbo].[up_Report_NegativeKms]
	@ContractId			int = NULL,
	@StartDate			date = NULL,
	@EndDate			date = NULL
AS
BEGIN
	IF @ContractId IS NULL SET @ContractId = [dbo].[fn_Session_GetContractId] ();
	IF @StartDate IS NULL SET @StartDate = dbo.fn_GetStartDate( COALESCE(@EndDate, GetDATE()) );
	IF @EndDate IS NULL  SET @EndDate = dbo.fn_GetEndtDate( COALESCE(@StartDate, GetDATE()) );

	WITH CTE_Data AS
	(
		SELECT r.RunId, 
				COUNT(d.Day) AS DayCount,
				SUM(CASE WHEN d.OutOfPeriod = 1 THEN 1 ELSE 0 END ) AS NotWorkedDayCount,
				SUM(CASE WHEN d.OutOfPeriod = 0 THEN 1 ELSE 0 END ) AS WorkedDayCount
			FROM dbo.Runs r
			INNER JOIN dbo.RunDays d
				ON d.RunId = r.RunId
			WHERE r.EndDate < @EndDate
				AND r.Extra = 0
				AND r.ContractId = @ContractId			
				AND d.Day BETWEEN @StartDate AND @EndDate
			GROUP BY r.RunId
	)
	SELECT r.RunId,
			r.CtaRunId,
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
			COALESCE(d.NotWorkedDayCount * v.Km, 0) AS ContractNegativeKm
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


/*
USE CtaLineaDb
gO

EXEC [dbo].[uo_RunsNeedRecalc];
EXEC [dbo].[uo_RecalcRunDays_Massive];

*/