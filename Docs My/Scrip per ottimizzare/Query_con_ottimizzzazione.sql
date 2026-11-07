USE CtaLineaDb
GO

-- TUTTO INUTILER !
-- meglio odiicare la query nel programma
-- questo sicuramente è veloce
-- casomai modificare il ring

SET STATISTICS TIME ON;
SET STATISTICS IO ON;


DECLARE @StartDate DATE = '20260901';
DECLARE @EndDate DATE = '20270831';
DECLARE @AssociateId UNIQUEIDENTIFIER  = NULL;
DECLARE @CarId UNIQUEIDENTIFIER  = NULL;
DECLARE @RunId UNIQUEIDENTIFIER  = NULL;
DECLARE @RinVariationId UNIQUEIDENTIFIER  = NULL;


WITH CTE_Var AS 
(
	SELECT DISTINCT rd.RunVariationId
		FROM Dbo.RunDays rd
		INNER JOIN dbo.RunCars rc
			ON rd.RunCarId = rc.RunCarId
		WHERE rd.Day BETWEEN @StartDate AND @EndDate
			AND Rd.Suspended = 0
			AND rd.OutOfPeriod = 0
			AND rd.Replaced = 0
			AND (@RunId IS NULL
				OR rd.RunId = @RunId)
			AND (@RinVariationId IS NULL
				OR rd.RunVariationId = @RinVariationId)
			AND (@ASsociateId IS NULL
				OR rc.AssociateId = @ASsociateId)
			AND (@CarId IS NULL
				OR rc.CarId = @CarId)
)
SELECT rv.RunVariationId AS Id,
		r.CtaRunId AS CtaId,
		-- descrizione
		CAST(rv.LineNumber AS varchar(MAX)) + ' / ' 
		+ COALESCE(rv.RunNumber, '') 
		+ COALESCE(', ' + r.RunName, '') 
		+ COALESCE(' ' +  p.PathsDescr, '')
		AS Description,
		
		rv.Km AS PathKm
	FROM CTE_Var v
	INNER JOIN Dbo.RunVariations rv
		ON V.RunVariationId = rv.RunVariationId
	INNER JOIN dbo.Runs r
		ON rv.RunId = r.RunId
	LEFT JOIN [dbo].[vw_RunPaths] p
		ON r.RunId = p.RunId
	OPTION (HASH JOIN, MERGE JOIN, RECOMPILE);
