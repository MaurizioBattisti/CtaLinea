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
			COALESCE(v.StartDAte, '20010101') AS StartDate
		FROM dbo.RunVariations v
), CTE_Variants_2 AS
(
	SELECT v.RunVariationId,
			v.RunId,
			v.StartDate,
			ROW_NUMBER() OVER (PARTITION BY v.RunId ORDER BY v.StartDate ASC) as n
		FROM CTE_Variants v
), CTE_Variants_3 AS
(
	SELECT v1.RunId,
			v1.RunVariationId,
			v1.StartDate,
			COALESCE(
				DATEADD(day,  -1, v2.StartDate ),
				'99991231'
			) AS EndDAte
			
		FROM CTE_Variants_2 v1
		LEFT JOIN CTE_Variants_2 v2
			ON v1.RunId = v2.RunId
			AND v1.n = v2.n - 1
), CTE_PAths AS
(
	SELECT DISTINCT  rv.RunId,
			rv.Path
		FROM  dbo.RunVariations rv
		INNER JOIN CTE_Variants_3 AS v3
			ON rv.RunVariationId = v3.RunVariationId
		WHERE (v3.StartDate <= dbo.fn_Session_GetPeriodEndDate()
				OR dbo.fn_Session_GetPeriodEndDate() IS NULL)
			AND (v3.EndDAte >= dbo.fn_Session_GetPeriodStartDate()
				OR dbo.fn_Session_GetPeriodStartDate() IS NULL)
)
SELECT rc.RunId ,
		STRING_AGG(rc.Path, ', ') AS PathsDescr
	FROM CTE_PAths rc
	GROUP BY rc.RunId
	;
