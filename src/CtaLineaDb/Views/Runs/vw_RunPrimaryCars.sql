/* *************************************************
*	Maurizio Battisti
*	19/12/2022
*	Crea la lista dei mezzi titolari usati su una corsa, nei titolari mette anche le sostituzioni
 ************************************************* */
CREATE VIEW [dbo].[vw_RunPrimaryCars]
AS 
WITH CTE_Cars AS
(
	SELECT DISTINCT  rp.RunId,
			rc.CarId
		FROM  dbo.RunCars rc
		INNER JOIN dbo.RunPeriods rp
			ON Rc.RunPEriodId = rp.[RunPeriodId]
		WHERE rc.CarType IN ('P', 'R')
)
SELECT rc.RunId,
		STRING_AGG(c.Description, ', ') AS PrimaryCarsDescr
	FROM CTE_Cars rc
	INNER JOIN dbo.Cars c
		ON rc.CarId = c.CarId
	GROUP BY rc.RunId
	;