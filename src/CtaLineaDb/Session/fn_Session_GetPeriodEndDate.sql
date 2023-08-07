/* *********************************************************************
	Maurizio Battisti
	30/07/2023
	Restituisce la data di fine del eprido memorizzata in sessione
********************************************************************* */
CREATE FUNCTION [dbo].[fn_Session_GetPeriodEndDate] ()
RETURNS Date
BEGIN
	DECLARE @Data date;
	SELECT @Data = CASE WHEN d.value = '' OR d.value IS NULL
						THEN NULL
						ELSE CAST(d.value AS Date)
					END
		FROM dbo.tvf_Session_DecodeInfo() d
		WHERE d.Id = 2;
	RETURN @Data;
END