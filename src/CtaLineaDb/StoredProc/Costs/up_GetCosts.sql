/* ******************************************************************************************
	Maurizio Battisti
	14/01/2023
	calcola il dettaglio dei costi prendendo in considerazione i parametri indicati
****************************************************************************************** */
CREATE PROCEDURE [dbo].[up_GetCosts]
(
	@ContractId		int = NULL,
	@StartDate		Date = NULL,
	@EndDate		Date = NULL,
	@AssociateId	uniqueidentifier = NULL,
	@CarId			uniqueidentifier = NULL,
	@RunId			uniqueidentifier = NULL
)
AS
BEGIN
	
	SELECT *
		FROM [dbo].[tvf_RunCostDetails]()
		;


	RETURN 0;
END
