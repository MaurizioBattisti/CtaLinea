/* *********************************************************************
	Maurizio Battisti
	30/07/2023
	Restituisce il nome utente associato alla sessione
********************************************************************* */
CREATE FUNCTION [dbo].[fn_Session_GetUserName] ()
RETURNS varchar(MAX)
BEGIN
	DECLARE @Data varchar(MAX);
	SELECT @Data = CASE WHEN d.value = '' OR d.value IS NULL
						THEN NULL
						ELSE d.value
					END
		FROM dbo.tvf_Session_DecodeInfo() d
		WHERE d.Id = 4;
	RETURN @Data;
END