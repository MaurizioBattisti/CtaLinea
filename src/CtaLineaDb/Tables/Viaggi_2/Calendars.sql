CREATE TABLE [dbo].[Calendars]
(
	[CalendarId] VARCHAR(10) NOT NULL PRIMARY KEY, 
    [Description] VARCHAR(200) NOT NULL, 
    [CalendarType] CHAR NOT NULL DEFAULT 'I'
)
