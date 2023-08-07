/* ***************************************************************
	Maurizio Battisti
	05/08/2023
	Restituisce una lista di corse che contengono il consorziato indicato ma non come titolare
*************************************************************** */
CREATE FUNCTION [dbo].[tvf_NotPrimaryAssociateRuns]
(
	@AssociateId	uniqueidentifier = NULL
)
RETURNS @Tbl_Runs TABLE
(
	RunId	uniqueidentifier
)
AS
BEGIN
	IF @AssociateId IS NOT NULL
	BEGIN
		WITH CTE_ASs AS
		(
			SELECT DISTINCT p.RunId,
					c.AssociateId,
					c.CarType
				FROM dbo.RunCars c
				INNER JOIN dbo.RunPeriods p
					ON  c.RunPeriodId = p.RunPEriodId
				WHERE (p.StartDate IS NULL	
						OR dbo.fn_Session_GetPeriodStartDate() IS NULL
						OR p.StartDate <= dbo.fn_Session_GetPeriodEndDate()
						)
					AND (p.EndDate IS NULL	
						OR dbo.fn_Session_GetPeriodStartDate() IS NULL
						OR p.EndDate >= dbo.fn_Session_GetPeriodStartDate()
						)
					AND c.AssociateId = @AssociateId
		), CTE_Primary_Ass AS
		(
			SELECT a.RunId,
					a.AssociateId
				FROM CTE_ASs a
				WHERE a.CarType = 'P'
		), CTE_Spare_Ass AS
		(
			SELECT a.RunId,
					a.AssociateId
				FROM CTE_ASs a
				WHERE a.CarType <> 'P'
		)
		INSERT INTO @Tbl_Runs (RunId)
		(
			SELECT o.RunId
				FROM CTE_Spare_Ass o
		)
		EXCEPT
		(
			SELECT p.RunId
				FROM CTE_Primary_Ass p
		)
	END

	RETURN;
END
