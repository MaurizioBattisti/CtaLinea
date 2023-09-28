/* ****************************************************************
	Maurizio Battisti
	25/09/2023
	Restiutisce la lsita delle corse con i dati per l'esportazione TT
**************************************************************** */
CREATE PROCEDURE [dbo].[up_GetRunsForExportTt]
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

	-- si assicura che tutti i dati da ricalcolare siano ricalcolati
	EXEC [dbo].[uo_RecalcRunDays_Massive] 0;

	WITH CTE_Days AS
	(
		(
			SELECT d.RunId,
					COUNT(*) AS DayCount,
					SUM(v.Km) AS KmTotal
				FROM dbo.RunDays d
				INNER JOIN dbo.Runs r
					ON d.RunId = r.RunId
					AND r.Elastibus = 0
				INNER JOIN dbo.RunVariations v
					ON d.RunVariationId = v.RunVariationId
				WHERE d.Day BETWEEN @Start AND @End
					AND d.CarNum = 1
				GROUP BY d.RunId
		) UNION (
			SELECT d.RunId,	
					COUNT(*) AS DayCount,
					SUM(d.Km) AS KmTotal
				FROM Dbo.RunElastibusDays d
				WHERE d.Day BETWEEN @Start AND @End
				GROUP BY d.RunId
		)
	)
	SELECT r.RunId,
			r.CtaRunId,
			r.RunNote,
			r.Extra,
			r.ContractRowNumber,
			r.LineNumber,
			r.RunNumber,
			r.StartTime,
			r.EndTime,

			r.PathsDescr,
			r.RequestedFrequency,
			r.AssociatesDescr,
			r.PrimaryCarsDescr,
			r.SpareAssociatesDescr,
			r.SpareCarsDescr,
			
			r.StartDate,
			r.EndDate,
			r.Km,

			d.DayCount,
			d.KmTotal
		FROM dbo.vw_Runs r
		INNER JOIN CTE_Days d
			ON r.RunId = d.RunId
		WHERE (@ContractId IS NULL OR r.ContractId = @ContractId)
			AND (r.StartDate IS NULL OR r.StartDate <= @End)
			AND (r.EndDate IS NULL OR r.EndDate >= @Start)
		ORDER BY r.[ContractRowNumber] ASC,
			r.[StartDate] ASC,
			r.[AssociatesDescr] ASC 
		;
	
	RETURN 0;
END
