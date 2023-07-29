/* ***************************************************************************
	Maurizio Battisti
	20/04/2023
	Restituisce una lista di mezzi di un determinato consoorziato
	che sono stati usati da qualche parte sulle corse a partire da una certa data
*************************************************************************** */
CREATE PROCEDURE [dbo].[up_GetCarForDiscontinuation]
	@AssociateId		uniqueidentifier,
	@RefDate			Date = NULL
AS
BEGIN
	SET DATEFIRST 1; -- this sets Monday to the first day of the week for the current connection.

	IF @RefDate IS NULL SET @RefDate = GETDATE();

	SELECT * FROM
	(
		(
			SELECT DISTINCT rc.CarId
				FROM dbo.RunCars rc
				INNER JOIN dbo.RunPeriods rp
					ON rc.RunPeriodId = rp.RunPeriodId
				INNER JOIN dbo.Runs r
					ON rp.RunId = r.RunId
				INNER JOIN dbo.Contracts c
					ON r.ContractId = c.ContractId
				WHERE @RefDate BETWEEN COALESCE (rp.StartDate, r.StartDate, c.StartDate)
								AND COALESCE (rp.EndDate, r.EndDate, c.EndDate)
					AND rc.AssociateId = @AssociateId
		) UNION (
			SELECT DISTINCT rc.CarId
				FROM dbo.RunCars rc
				INNER JOIN dbo.RunCarReplacementDetails crd
					ON rc.RunCarID =crd.ReplacedRunCarId
				INNER JOIN dbo.RunCarReplacements cr
					ON cr.CarReplacementId = crd.CarReplacementId
				INNER JOIN dbo.RunPeriods rp
					ON cr.RunPeriodId = rp.RunPeriodId
				INNER JOIN dbo.Runs r
					ON rp.RunId = r.RunId
				INNER JOIN dbo.Contracts c
					ON r.ContractId = c.ContractId
				WHERE @RefDate BETWEEN COALESCE (cr.StartDate, rp.StartDate, r.StartDate, c.StartDate)
								AND COALESCE (cr.EndDate, rp.EndDate, r.EndDate, c.EndDate)
		)
	) AS data;
	
	RETURN 0;
END
