/* *******************************************************************
	Maurizio Battisti	
	29/12/2022
	vista delle variatti
******************************************************************* */
CREATE VIEW [dbo].[vw_RunVariations]
AS 
WITH CTE_Calendars AS
(
	SELECT DISTINCT  vc.RunVariationId,
			vc.CalendarId
		FROM  dbo.RunVariationCalendars vc
), CTE_CalendarNames AS
(
	SELECT rc.RunVariationId,
			STRING_AGG(c.CalendarName, ', ') AS CalendarsDescr
		FROM CTE_Calendars rc
		INNER JOIN dbo.Calendars c
			ON rc.CalendarId  = c.CalendarId
		GROUP BY rc.RunVariationId
)	
SELECT  v.RunId,
		v.RunVariationId,
		c.CalendarsDescr,
		v.LineNumber, v.RunNumber,
		v.StartDate AS VariationStartDate,
		v.StartTime, v.EndTime,
		v.Monday, v.Tuesday, v.Wednesday, v.Thursday, v.Friday, v.Saturday, v.Sunday,
		v.Path, v.RequestedFrequency,
		v.Km, v.RequestedCapacity,
		v.Note AS VariationNote
	FROM dbo.RunVariations v
	INNER JOIN CTE_CalendarNames c
		ON V.RunVariationId =  c.RunVariationId

	;


