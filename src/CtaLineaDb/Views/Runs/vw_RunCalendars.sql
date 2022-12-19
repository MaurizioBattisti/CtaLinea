/* *************************************************
*	Maurizio Battisti
*	19/12/2022
*	Crea la lista dei calendari di una corsa
 ************************************************* */
CREATE VIEW [dbo].[vw_RunCalendars]
AS 
WITH CTE_Calendars AS
(
	SELECT DISTINCT  rv.RunId,
			rv.CalendarId
		FROM  dbo.RunVariations rv
)
SELECT rc.RunId ,
		STRING_AGG(c.CalendarName, ', ') AS CalendarsDescr
	FROM CTE_Calendars rc
	INNER JOIN dbo.Calendars c
		ON rc.CalendarId  = c.CalendarId
	GROUP BY rc.RunId