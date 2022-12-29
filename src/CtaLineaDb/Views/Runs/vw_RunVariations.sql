/* *******************************************************************
	Maurizio Battisti	
	29/12/2022
	vista delle variatti
******************************************************************* */
CREATE VIEW [dbo].[vw_RunVariations]
AS 
SELECT  v.RunId,
		v.RunVariationId,
		v.CalendarId,
		c.CalendarName,
		v.LineNumber, v.RunNumber,
		v.StartDate AS VariationStartDate,
		v.StartTime, v.EndTime,
		v.Monday, v.Tuesday, v.Wednesday, v.Thursday, v.Friday, v.Saturday, v.Sunday,
		v.Path, v.RequestedFrequency,
		v.Km, v.RequestedCapacity,
		v.Note AS VariationNote
	FROM dbo.RunVariations v
	INNER JOIN dbo.Calendars c
		ON V.CalendarId =  c.CalendarId
	;


