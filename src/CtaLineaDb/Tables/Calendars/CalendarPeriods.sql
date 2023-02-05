CREATE TABLE [dbo].[CalendarPeriods]
(
	[CalendarPeriodId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CalendarId] INT NOT NULL,
    
    [StartDate] DATE NULL, 
    [EndDate] DATE NULL, 
    
    [Note] VARCHAR(MAX) NULL, 

    CONSTRAINT [FK_Periods_Calendar] 
        FOREIGN KEY ([CalendarId]) 
        REFERENCES [dbo].[Calendars]( [CalendarId])
        ON DELETE CASCADE
        ON UPDATE CASCADE
)
