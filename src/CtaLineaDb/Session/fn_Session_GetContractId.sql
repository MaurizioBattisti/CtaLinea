/* *********************************************************************
	Maurizio Battisti
	30/07/2023
	Restituisce l'id dell'appalto passato come argomento alla sessione
********************************************************************* */
CREATE FUNCTION [dbo].[fn_Session_GetContractId] ()
RETURNS int
BEGIN
	DECLARE @Data int;
	SELECT @Data = CASE WHEN d.value = '' OR d.value IS NULL
						THEN NULL
						ELSE CAST(d.value AS int)
					END
		FROM dbo.tvf_Session_DecodeInfo() d
		WHERE d.Id = 3;
	RETURN @Data;
END