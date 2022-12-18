/* ***************************************************************************
*	Maurizio bAttisti
*	18/12/2022
*	Crea un elenco di giorni tra due date
*************************************************************************** */
CREATE FUNCTION [dbo].[tvf_Days]
(
	@Start		date,
	@End		date
)
RETURNS @DaysTable TABLE
(
	Day		date PRIMARY KEY,
	Number	int NOT NULL
)
AS
BEGIN
	WITH x AS 
	(SELECT n FROM (VALUES (0),(1),(2),(3),(4),(5),(6),(7),(8),(9)) v(n))
	, CTE_Num AS (SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS number
				FROM x ones, x tens, x hundreds, x thousands)
	INSERT INTO @DaysTable
		SELECT DATEADD(d, number, @Start) AS Day,
			number
		FROM CTE_Num
		WHERE DATEADD(d, number, @Start) <= @End

	RETURN
END
