USE CtaLineaDb
GO

-- TUTTO INUTILE MEGLIO DIFICARE LA QUERU NEL PROGRAMMA
-- Ottimizzaizoen del driver ring
-- non ha funzionato  o non lo so
-- c'è ancora da indagare

-- Modifiche per iottimizzazione
CREATE NONCLUSTERED INDEX IX_RunVariations_RunId_StartDate 
    ON dbo.RunVariations (RunId, StartDate DESC)
    INCLUDE (RunVariationId, Path);
GO

/* *************************************************
*	Maurizio Battisti
*	19/12/2022
*	Crea la lista dei Percorsi associati ad una corsa
*	Changtes
*	15/10/2026	Ottimizzaizone
 ************************************************* */
CREATE OR ALTER VIEW [dbo].[vw_RunPaths]
AS 
WITH CTE_CurrentDate AS (
    -- Calcoliamo la data UNA SOLA VOLTA all'inizio della vista
    SELECT [dbo].[fn_Session_GetCurrentDate]() AS CurDate
), CTE_Variants AS
(
	SELECT v.RunVariationId,
			v.RunId,
			v.Path,
			COALESCE(v.StartDAte, '20010101') AS StartDate			
		FROM dbo.RunVariations v
), CTE_Variants_2 AS
(
	SELECT v.RunId,
			v.RunVariationId,
			v.Path,
			ROW_NUMBER() OVER (PARTITION BY v.RunId ORDER BY v.StartDate DESC) as n
		FROM CTE_Variants v
		CROSS JOIN CTE_CurrentDate d -- Inseriamo la data calcolata una sola volta
		-- WHERE v.StartDate <= [dbo].[fn_Session_GetCurrentDate] ()
		WHERE v.StartDate <= d.CurDate
), CTE_Nodes AS 
(
	SELECT n.RunVariationId,
			COALESCE(TRIM(cp.Description) + ' ', '') +
				CASE WHEN n.CoincidenceDescr IS NOT NULL THEN '(*)' ELSE  '' END AS Descr,
			ROW_NUMBER() OVER (PARTITION BY n.RunVariationId ORDER BY n.Hour, n.ProgrNumber) AS Asc_Num,
			ROW_NUMBER() OVER (PARTITION BY n.RunVariationId ORDER BY n.Hour DESC, n.ProgrNumber DESC) AS Desc_Num
		FROM dbo.RunNodes n
		INNER JOIN dbo.CollectionPoints cp
			ON n.CollectionPointId = cp.CollectionPointId
), CTE_NodePathDescr AS
(
	SELECT ns.RunVariationId,
			COALESCE(ne.Descr + ' - ', '') + COALESCE(ne.Descr, '') AS PathDescr
		FROM CTE_Nodes ns
		INNER JOIN CTE_Nodes ne
			ON ns.RunVariationId = ne.RunVariationId
			AND ns.Asc_Num = 1 AND ne.Desc_Num = 1
), CTE_PAths AS
(
	SELECT DISTINCT  rv.RunId,
			CASE WHEN TRIM(p.PathDescr) = '' 
				THEN rv.Path
				ELSE COALESCE(TRIM(p.PathDescr), rv.Path)
			END AS Path
		FROM  CTE_Variants_2 rv
		LEFT JOIN CTE_NodePathDescr p
			ON rv.RunVariationId = p.RunVariationId
		WHERE rv.n = 1
)
SELECT
		rc.RunId ,
		STRING_AGG(rc.Path, ', ') AS PathsDescr
	FROM CTE_PAths rc
	GROUP BY rc.RunId
	;
GO

-- Per eliminare
-- EXEC sp_control_plan_guide N'DROP', N'CTA-Ring-Linea-Driver-Performance';

-- **************************

EXEC sp_control_plan_guide N'DROP', N'CTA-Ring-Linea-Driver-Performance';
GO

-- 2. Crea la nuova Plan Guide agganciata a sp_executesql
EXEC sp_create_plan_guide 
    @name = N'CTA-Ring-Linea-Driver-Performance',
    @stmt = N'  WITH CTE_Var AS   (   SELECT DISTINCT rd.RunVariationId    FROM Dbo.RunDays rd    INNER JOIN dbo.RunCars rc     ON rd.RunCarId = rc.RunCarId    WHERE rd.Day BETWEEN @StartDate AND @EndDate     AND Rd.Suspended = 0     AND rd.OutOfPeriod = 0     AND rd.Replaced = 0     AND (@RunId IS NULL      OR rd.RunId = @RunId)     AND (@RinVariationId IS NULL      OR rd.RunVariationId = @RinVariationId)     AND (@ASsociateId IS NULL      OR rc.AssociateId = @ASsociateId)     AND (@CarId IS NULL      OR rc.CarId = @CarId)  )  SELECT rv.RunVariationId AS Id,    r.CtaRunId AS CtaId,    -- descrizione    CAST(rv.LineNumber AS varchar(MAX)) + '' / ''     + COALESCE(rv.RunNumber, '''')     + COALESCE('', '' + r.RunName, '''')     + COALESCE('' '' +  p.PathsDescr, '''')    AS Description,       rv.Km AS PathKm   FROM CTE_Var v   INNER JOIN Dbo.RunVariations rv    ON V.RunVariationId = rv.RunVariationId   INNER JOIN dbo.Runs r    ON rv.RunId = r.RunId   LEFT JOIN [dbo].[vw_RunPaths] p    ON r.RunId = p.RunId   ;',
    @type = N'SQL',
    @module_or_batch = NULL,
    @params = N'@StartDate datetime,@EndDate datetime,@AssociateId uniqueidentifier,@CarId uniqueidentifier,@RunId uniqueidentifier,@RinVariationId uniqueidentifier',
    @hints = N'OPTION (HASH JOIN, MERGE JOIN, RECOMPILE)';
GO