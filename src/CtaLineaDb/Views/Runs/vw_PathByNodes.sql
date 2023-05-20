/* ************************************************************************************
	Maurizio Battisti
	13/05/2023
	Restituisce  orario di partenza e  percorso di una variation prendendoli dai nodi
************************************************************************************  */
CREATE VIEW [dbo].[vw_PathByNodes]
AS 
WITH CTE_Nodes AS
(
	SELECT ROW_NUmber() OVER (PARTITION BY n.RunVariationId ORDER BY n.ProgrNumber, n.Hour) Asc_Ordinal,
		ROW_NUmber() OVER (PARTITION BY n.RunVariationId ORDER BY n.ProgrNumber DESC, n.Hour DESC) DESC_Ordinal,
			n.RunVariationId,
			n.ProgrNumber,
			n.Hour,
			cp.Description
		FROM dbo.RunNodes n
		INNER JOIN dbo.CollectionPoints cp
			ON n.CollectionPointId = cp.CollectionPointId
), CTE_NodePath AS
(
	SELECT *
		FROM CTE_Nodes n
		WHERE n.Asc_Ordinal = 1
			OR n.DESC_Ordinal = 1
)
SELECT n.RunVariationId ,
		MIN(n.Hour) AS StartTime,
		STRING_AGG(n.Description, ' - ') WITHIN GROUP (ORDER BY n.Asc_Ordinal) AS Path
	FROM CTE_NodePath  n
	GROUP BY n.RunVariationId;