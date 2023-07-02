/* ************************************************************************
	Maurizio Battisti
	02/07/2023
	Restituisce la data di fine anno scolastico a partre da una data
 *********************************************************************** */
 CREATE FUNCTION [dbo].[fn_GetEndtDate]
(
	@Date		date = NULL
)
RETURNS DATE
As
bEGIN
	DECLARE @Year		int;
	DECLARE @StartDate	date;
	
	IF @Date IS NULL SET @Date = GETDATE();
	SET @Year = YEAR(@Date);

	-- calcola l'anno di inizio 
	IF MONTH(@Date) >= 9 SET @Year =@Year + 1;

	sET @StartDate= CAST((CAST(@Year AS Varchar(4)) + '0831') AS DATE);

	RETURN @StartDate;
END
