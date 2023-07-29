/* *****************************************************
*	Maurizio Battisti
*	25/07/2023
*	Calcola se un giorno è o meno nella ripetizione indicata
*	restituisce 1 se è nella ripetizione
***************************************************** */
CREATE FUNCTION [dbo].[fn_IsDatInRepeatRange]
(
	@Day		date,
	@StartDAte	date,
	@RepType	char(1) = NULL,
				-- NULL -> Always
				-- A -> Always
				-- W -> Weekly
				-- M -> Monthly
	@Pattern	varchar(MAX) = NULL
				-- s: '01', '10', '10010' ecc.
)
RETURNS BIT
AS
BEGIN
	DECLARE @Result		bit = 1;
	DECLARE @Index		int = 1;
	DECLARE @Delta		int;
	
	SET @RepType = UPPER(@RepType);

	IF @RepType IS NOT NULL  AND @RepType <> ''  AND @RepType <> 'A' 
	BEGIN
		SET @Result = 0;
		IF @Pattern IS NOT NULL AND @Pattern <> '' 
		BEGIN
			SET @Delta = CASE @RepType 
					WHEN 'W' THEN DATEDIFF(week, 0, DATEADD(day, -@@DATEFIRST, @Day)) - DATEDIFF(week, 0, DATEADD(day, -@@DATEFIRST, @StartDAte))
					WHEN 'M' THEN DATEDIFF (month, @StartDAte, @Day)
					ELSE 0
				END;

			SET @Delta = ABS(@Delta);
			SET @Index =  1 +(@Delta % LEN(@Pattern));
			SET @Result = CAST(SUBSTRING(@Pattern, @Index, 1) AS bit);
		END
	END

	RETURN @Result;
END
