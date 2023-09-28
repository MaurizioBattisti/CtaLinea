/* ****************************************************************
	Maurizio Battisti
	22/09/2023
	Restituisce i dati dei gironi elastibus per l'esportaizone verso la TT
**************************************************************** */
CREATE PROCEDURE [dbo].[up_GetElastibusDayForExport]
	@ContractId		INT = NULL,
	@StartDate		DATE = NULL,
	@EndDAte		DATE = NULL
AS
BEGIN
	-- calcola le date di inizo e fine nel caso non siano impostate
	DECLARE @Dt		DATE;
	DECLARE @Start	DATE;
	DECLARE @End	DATE;
	SET @Dt = dbo.fn_Session_GetCurrentDate();
	SET @Start = COALESCE(@StartDate, [dbo].[fn_GetStartDate](@dt));
	SET @End = COALESCE(@EndDate, [dbo].[fn_GetEndtDate](@dt));

	WITH CTE_Start_End_Base AS
	(
		SELECT n.RunVariationId,
				MIN(n.Hour) AS StartTime,
				MAX(n.Hour) AS EndTime
			FROM dbo.RunNodes n
			GROUP BY n.RunVariationId
	), CTE_Base AS
	(
		SELECT d.RunId,
				d.Day, 
				d.Km, d.PeopleCount,
				COALESCE(r.RunName, v.path, '') AS Descr,
				COALESCE (h.StartTime, v.StartTime) AS StartTime,
				COALESCE (h.EndTime, v.EndTime) AS EndTime,
				ROW_NUMBER() OVER (PARTITION BY d.RunId, d.Day ORDER BY v.StartDate DESC) AS Number
			FROM dbo.RunElastibusDays d
			INNER JOIN dbo.Runs r
				ON d.RunId = r.RunId
			INNER JOIN dbo.RunVariations v
				ON d.RunId = v.RunId
				AND d.Day >= COALESCE(v.StartDate, '20000101')
			LEFT JOIN CTE_Start_End_Base h
				ON v.RunVariationId = h.RunVariationId
			WHERE r.Elastibus = 1
				AND (@ContractId IS NULL
					OR r.ContractId = @ContractId)
				AND d.Day BETWEEN @Start AND @End
	)
	SELECT b.RunId,
			b.Day,
			b.Km,
			b.PeopleCount,
			b.Descr 
				+ COALESCE(' ' + CONVERT(VARCHAR(5), b.StartTime, 114), '')
				+ COALESCE(' - ' + CONVERT(VARCHAR(5), b.EndTime, 114), '')
				AS Description
		FROM CTE_Base b
		WHERE b.Number = 1;
	
	RETURN 0;
END
