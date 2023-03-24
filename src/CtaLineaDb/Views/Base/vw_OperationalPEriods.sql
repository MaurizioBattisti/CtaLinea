/* ******************************************************
	Maurizio Battisti
	12/03/2023
	vista con i periodi opperativi dei contratti
****************************************************** */
CREATE VIEW [dbo].[vw_OperationalPEriods]
AS
WITH x AS 
(
	SELECT n FROM 
	(VALUES (0),(1),(2),(3),(4),(5),(6),(7),(8),(9)) v(n)
), CTE_Num AS 
(
	SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS number
	FROM x ones, x tens, x hundreds, x thousands
), CTE_Dates AS
(
	SELECT DATEADD(year, n.number, CAST('1999-09-1' AS Date)) AS StartDate,
		DATEADD(year, n.number, CAST('2000-08-31' AS Date)) AS EndDAte
		FROM CTE_Num n
		WHERE n.number <3000
)
SELECT DISTINCT d.StartDate, d.EndDAte
	FROM CTE_Dates d
	INNER JOIN dbo.Contracts c
		ON c.StartDate <= d.EndDAte
		AND c.EndDate >= d.StartDate
	;
