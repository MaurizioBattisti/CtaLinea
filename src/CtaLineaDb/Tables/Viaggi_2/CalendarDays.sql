CREATE TABLE [dbo].[CalendarDays]
(
	[CalendarId] VARCHAR(10) NOT NULL , 
    [Date] DATE NOT NULL, 

    PRIMARY KEY ([CalendarId], [Date]), 
    CONSTRAINT [FK_DaysCalendar] 
        FOREIGN KEY (CalendarId) 
        REFERENCES dbo.Calendars(CalendarId)
        ON DELETE CASCADE
        ON UPDATE CASCADE
)
