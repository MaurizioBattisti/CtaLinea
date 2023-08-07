/* **************************************************************
	Maurizio Battisti
	30/07/2023
	Salva alcune informaizoni in sessione per poterle utilizzare nelle viste e e nelle query senza doverle passare sempre come argomento
************************************************************** */
CREATE PROCEDURE [dbo].[up_Session_SetInfo]
	@PeriodStart	date = NULL,
	@PeriodEnd		date = NULL,
	@ContractId		int = NULL,
	@UserName		varchar(128) = NULL
AS
BEGIN
	DECLARE @Data		varchar(MAX);

	SET @Data = COALESCE(CAST(@PeriodStart AS VARCHAR(10)), '') + ';' 
		+ COALESCE(CAST(@PeriodEnd AS VARCHAR(10)), '') + ';' 
		+ COALESCE(CAST(@ContractId AS VARCHAR(10)), '') + ';' 
		+ COALESCE(@UserName, '');

	DECLARE @UserIDBin AS varbinary(MAX);

	SET @UserIDBin = CAST (@Data as varbinary(MAX))
	SET CONTEXT_INFO @UserIDBin
END
