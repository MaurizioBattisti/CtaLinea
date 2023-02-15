/* *********************************************************************************
	Maurizio Battisti
	13/02/2023
	restituisce una lista da usare come planning per la corsa
********************************************************************************* */
CREATE PROCEDURE [dbo].[up_GetRunPlanning]
	@RunId		uniqueidentifier = NULL,
	@StartDate	date = NULL,
	@EndDate	date = NULL
AS
BEGIN
	SET @StartDate = COALESCE(@StartDate, GETDATE());
	SET @EndDate = COALESCE(@EndDate, GETDATE());

	-- si assicura che tutti i dati da ricalcolare siano ricalcolati
	EXEC [dbo].[uo_RecalcRunDays_Massive] 0;

	WITH CTE_Hours AS
	(
		SELECT n.RunVariationId,
   			 MIN(n.Hour) AS MinHour,
   			 MAX(n.Hour) AS MaxHour
   		 FROM dbo.RunNodes n
   		 GROUP BY n.RunVariationId
	), CTE_Days_Base AS
	(
		SELECT DISTINCT rd.RunId,
			rd.Day,
			rd.RunVariationId,
			rd.RunPeriodId
		FROM dbo.RunDays rd
		WHERE rd.OutOfPeriod = 0
			AND rd.Suspended = 0
			AND rd.CarNum = 1
			AND rd.Day BETWEEN @StartDate AND @EndDate
			AND rd.RunId = @RunId
	), CTE_DaySDescr AS
	(
		SELECT rd.RunId,
			CAST(rd.Day AS DATETIME) + CAST(COALESCE(h.MinHour, v.StartTime) AS DATETIME) AS StartTime,
			CAST(rd.Day AS DATETIME) + CAST(COALESCE(h.MaxHour, v.EndTime) AS DATETIME) AS EndTime,
			rd.RunVariationId,
			rd.RunPeriodId,
			COALESCE(
				'Dal ' + FORMAT(v.StartDate, 'd', 'it-IT'),
				'Dall''Inizio'
			) AS VariationDescr,
			COALESCE(
				FORMAT(p.StartDate, 'd', 'it-IT') + ' - ',
				'Dall''Inizio'
			)  + ' - ' +
			COALESCE(
				FORMAT(p.EndDate, 'd', 'it-IT'),
				'Alla Fine'
			)  AS PeriodDate_Descr,
			'' 
			+ CASE WHEN  p.Monday = 1 THEN 'Lun, ' ELSE '' END
			+ CASE WHEN  p.Tuesday = 1 THEN 'Mar, ' ELSE '' END
			+ CASE WHEN  p.Wednesday = 1 THEN 'Mer, ' ELSE '' END
			+ CASE WHEN  p.Thursday = 1 THEN 'Gio, ' ELSE '' END
			+ CASE WHEN  p.Friday = 1 THEN 'Ven, ' ELSE '' END
			+ CASE WHEN  p.Saturday = 1 THEN 'Sab, ' ELSE '' END
			+ CASE WHEN  p.Sunday = 1 THEN 'Dom, ' ELSE '' END
			AS PeriodWeek_Descr
		FROM CTE_Days_Base rd
		INNER JOIN dbo.RunPeriods p
			ON rd.RunPeriodId = p.RunPeriodId
		INNER JOIN dbo.RunVariations v
			ON rd.RunVariationId = v.RunVariationId
		LEFT JOIN CTE_Hours h
			ON v.RunVariationId = h.RunVariationId
	)
	SELECT 
		rd.RunId,
		rd.StartTime,
		rd.EndTime,
		rd.RunPeriodId,
		rd.RunVariationId,
		rd.VariationDescr,
		rd.PeriodDate_Descr + ' ( '+
			SUBSTRING(rd.PeriodWeek_Descr, 0, LEN(rd.PeriodWeek_Descr) )
			+ ' )' AS PeriodDescr
	FROM CTE_DaySDescr rd
		;

	RETURN 0;
END
