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
			vc.CalendarId,
			vc.Exclusion
		FROM  dbo.RunVariationCalendars vc
		INNER JOIN dbo.RunVariations rv
			ON rv.RunVariationId = vc.RunVariationId
)
SELECT rc.RunId ,
		STRING_AGG(c.CalendarName + (CASE WHEN rc.Exclusion = 1 THEN ' (E)' ELSE '' END)  , ', ') AS CalendarsDescr
	FROM CTE_Calendars rc
	INNER JOIN dbo.Calendars c
		ON rc.CalendarId  = c.CalendarId
	GROUP BY rc.RunId