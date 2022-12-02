CREATE TABLE [dbo].[Calendars]
(
	[CalendarId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [BaseCalendarId] INT NULL , 

    [CalendarName] VARCHAR(200) NOT NULL, 
    [WorkDays] BIT NOT NULL DEFAULT 1, 
    
    CONSTRAINT [FK_CalendarHierarchy] 
        FOREIGN KEY ([BaseCalendarId]) 
        REFERENCES [dbo].[Calendars]([CalendarId])
        ON DELETE NO ACTION
        ON UPDATE CASCADE
)
