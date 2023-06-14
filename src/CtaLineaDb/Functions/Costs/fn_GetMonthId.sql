/* ***************************************************************
	Maurizio Battisti
	04/06/2023
	restituisce un intero da usare per confrontare l e date delo stesso mese
	usata per i multirun forfaits
*************************************************************** */
CREATE FUNCTION [dbo].[fn_GetMonthId]
(
	@Day		date
)
RETURNS INT
As
bEGIN
	DECLARE @MonthId		int;
	SET @MonthId = CAST(FORMAT(@Day, 'yyyyMM') AS INT);
	RETURN @MonthId;
END
