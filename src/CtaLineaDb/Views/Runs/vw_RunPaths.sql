/* *************************************************
*	Maurizio Battisti
*	19/12/2022
*	Crea la lista dei Percorsi associati ad una corsa
 ************************************************* */
CREATE VIEW [dbo].[vw_RunPaths]
AS 
WITH CTE_Variants AS
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
		WHERE v.StartDate <= [dbo].[fn_Session_GetCurrentDate] ()
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
SELECT rc.RunId ,
		STRING_AGG(rc.Path, ', ') AS PathsDescr
	FROM CTE_PAths rc
	GROUP BY rc.RunId
	;
