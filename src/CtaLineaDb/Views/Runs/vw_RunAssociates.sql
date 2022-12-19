/* *************************************************
*	Maurizio Battisti
*	19/12/2022
*	Crea la lista delle ditte che lavorano su una corsa
 ************************************************* */
  CREATE VIEW [dbo].[vw_RunAssociates]
AS 
WITH CTE_Associates AS
(
	SELECT DISTINCT  rp.RunId,
			rc.AssociateId
		FROM  dbo.RunCars rc
		INNER JOIN dbo.RunPeriods rp
			ON Rc.RunPEriodId = rp.RunPEriodId
)
SELECT rc.RunId,
		STRING_AGG(a.Description, ', ') AS AssociatesDescr
	FROM CTE_Associates rc
	INNER JOIN dbo.Associates a
		ON rc.AssociateId = a.AssociateId
	GROUP BY rc.RunId
	;