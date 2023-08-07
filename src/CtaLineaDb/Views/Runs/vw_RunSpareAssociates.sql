/* *************************************************
*	Maurizio Battisti
*	21/07/2023
*	Crea la lista delle ditte che hanno mezzi di scorta su una corsa
 ************************************************* */
CREATE VIEW [dbo].[vw_RunSpareAssociates]
AS 
WITH CTE_Associates AS
(
	SELECT DISTINCT  rp.RunId,
			rc.AssociateId
		FROM  dbo.RunCars rc
		INNER JOIN dbo.RunPeriods rp
			ON Rc.RunPEriodId = rp.[RunPeriodId]
		WHERE rc.CarType = 'S'
			AND (rp.StartDate IS NULL
				OR dbo.fn_Session_GetPeriodEndDate() IS NULL
				OR rp.StartDate <= dbo.fn_Session_GetPeriodEndDate()
				)
			AND (rp.EndDate IS NULL
				OR dbo.fn_Session_GetPeriodStartDate() IS NULL
				OR rp.EndDate >= dbo.fn_Session_GetPeriodStartDate()
				)
)
SELECT rc.RunId,
		STRING_AGG(a.Description, ', ') AS SpareAssociatesDescr
	FROM CTE_Associates rc
	INNER JOIN dbo.Associates a
		ON rc.AssociateId = a.AssociateId
	GROUP BY rc.RunId
	;