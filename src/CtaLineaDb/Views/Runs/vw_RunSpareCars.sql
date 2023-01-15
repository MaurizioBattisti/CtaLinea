/* *************************************************
*	Maurizio Battisti
*	19/12/2022
*	Crea la lista dei mezzi di riserva
 ************************************************* */
CREATE VIEW [dbo].[vw_RunSpareCars]
AS
WITH CTE_Cars AS
(
	SELECT DISTINCT  rp.RunId,
			rc.CarId
		FROM  dbo.RunCars rc
		INNER JOIN dbo.RunPeriods rp
			ON Rc.RunPEriodId = rp.[RunPeriodId]
		WHERE rc.CarType IN ('S')
)
SELECT rc.RunId,
		STRING_AGG(c.Description, ', ') AS SpareCarsDescr
	FROM CTE_Cars rc
	INNER JOIN dbo.Cars c
		ON rc.CarId = c.CarId
	GROUP BY rc.RunId
	;