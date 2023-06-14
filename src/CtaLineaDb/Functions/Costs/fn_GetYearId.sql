/* ***************************************************************
	Maurizio Battisti
	04/06/2023
	restituisce un intero da usare per confrontare l e date delo stesso mese
	usata per i multirun forfaits
*************************************************************** */
CREATE FUNCTION [dbo].[fn_GetYearId]
(
	@Day		date
)
RETURNS INT
As
bEGIN
	DECLARE @YearId		int;
	SET @YearId = YEAR(@DAy);
	IF MONTH(@DAy) < 9 SET @YearId =@YearId -1;
	SET @YearId = @YearId * 10001 +  1;

	RETURN @YearId;
END
