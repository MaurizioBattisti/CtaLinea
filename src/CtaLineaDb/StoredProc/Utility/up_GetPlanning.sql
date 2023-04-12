/* *********************************************************************************
	Maurizio Battisti
	26/01/2023
	restituisce una lista di dati da usare per gestire le occupazioni sul planning
********************************************************************************* */
CREATE PROCEDURE [dbo].[up_GetPlanning]
	@AssociateId	uniqueidentifier = NULL,
	@CarId			uniqueidentifier = NULL,
	@StartDate		date = NULL,
	@EndDate		date = NULL
AS
BEGIN
	SET @StartDate = COALESCE(@StartDate, GETDATE());
	SET @EndDate = COALESCE(@EndDate, GETDATE());

	-- si assicura che tutti i dati da ricalcolare siano ricalcolati
	EXEC [dbo].[uo_RecalcRunDays_Massive] 0;

	WITH CTE_Base AS
	(
		SELECT  DISTINCT c.RunId,
				c.RunVariationId,
				r.RunName,
				r.ContractId,
				c.RunCarId,
				rc.AssociateId,
				rc.CarId,
				c.Day
			FROM [dbo].[vw_Costs] c
			INNER JOIN dbo.Runs r
				ON c.RunId = r.RunId
			INNER JOIN dbo.RunCars rc
				ON c.RunCarId = rc.RunCarId
			WHERE c.Suspended = 0
				AND c.OutOfPeriod = 0
				AND (@AssociateId IS NULL	
					OR rc.AssociateId = @AssociateId)
				AND (@CarId IS NULL
					OR rc.CarId = @CarId)
				AND c.Day BETWEEN @StartDate AND @EndDate
	), CTE_Hours AS
	(
		SELECT n.RunVariationId,
				MIN(n.Hour) AS MinHour,
				MAX(n.Hour) AS MaxHour
			FROM dbo.RunNodes n
			GROUP BY n.RunVariationId
	)
	SELECT d.RunId,
			CAST(d.Day AS DATETIME) + CAST(h.MinHour AS DATETIME) AS StartTime,
			CAST(d.Day AS DATETIME) + CAST(h.MaxHour AS DATETIME) AS EndTime,
			c.CarId,
			c.BsCarId,
			c.Description AS CarDescr,
			d.RunName,
			v.LineNumber,
			v.RunNumber,
			v.Path
		FROM CTE_Base d
		INNER JOIN dbo.Runs r
			ON d.RunId = r.RunId
		INNER JOIN dbo.RunVariations v
			ON d.RunVariationId = v.RunVariationId 
		INNER JOIN Dbo.Cars c
			ON d.CarId = c.CarId
		INNER JOIN CTE_Hours h
			ON d.RunVariationId = h.RunVariationId
			;
	RETURN 0;
END
