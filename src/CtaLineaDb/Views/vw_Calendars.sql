/* **********************************************************************
	Maurizio Battisti
	27/01/2023
	lista di tutti i calendari con descrizione del tipo e con il riferimento al calendario di bAse
 ********************************************************************* */
CREATE VIEW [dbo].[vw_Calendars]
AS 
SELECT c.CalendarId,
		c.CalendarName,
		c.BaseCalendarId,
		b.CalendarName AS BaseCalendarName,
		c.Ordinal,
		c.CalendarType,
		CASE c.CalendarType
			WHEN 'NOP' THEN 'Completo' 
			WHEN 'EXC' THEN 'Esclusione' 
			WHEN 'INC' THEN 'Inclusione' 
			WHEN 'IPL' THEN 'Inversione livello precedente' 
			ELSE 'Indeifinito'
		END AS CalendarTypeDescr,
		c.Mondays,
		c.Tuesdays,
		c.Wednesdays,
		c.Thursdays,
		c.Fridays,
		c.Saturdays,
		c.Sundays,
		c.PreHolyday,
		c.PostHolyday
	FROM dbo.Calendars c
	LEFT JOIN dbo.Calendars b
		ON C.BaseCalendarId = b.CalendarId
