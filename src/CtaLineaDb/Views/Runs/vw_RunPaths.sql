/* *************************************************
*	Maurizio Battisti
*	19/12/2022
*	Crea la lista dei Percorsi associati ad una corsa
 ************************************************* */
CREATE VIEW [dbo].[vw_RunPaths]
AS 
WITH CTE_PAths AS
(
	SELECT DISTINCT  rv.RunId,
			rv.Path
		FROM  dbo.RunVariations rv
)
SELECT rc.RunId ,
		STRING_AGG(rc.Path, ', ') AS PathsDescr
	FROM CTE_PAths rc
	GROUP BY rc.RunId
	;
