/* *********************************************************************
	Maurizio Battisti
	30/07/2023
	Restituisce la data di inizio del eprido memorizzata in sessione
********************************************************************* */
CREATE FUNCTION [dbo].[fn_Session_GetPeriodStartDate] ()
RETURNS Date
BEGIN
	DECLARE @Data date;
	SELECT @Data = CASE WHEN d.value = '' OR d.value IS NULL
						THEN NULL
						ELSE CAST(d.value AS Date)
					END
		FROM dbo.tvf_Session_DecodeInfo() d
		WHERE d.Id = 1;
	RETURN @Data;
END