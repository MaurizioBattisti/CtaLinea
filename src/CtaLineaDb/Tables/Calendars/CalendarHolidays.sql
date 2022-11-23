CREATE TABLE [dbo].[CalendarHolidays]
(
	[CalendarId] INT NOT NULL , 
    [Holiday] DATE NOT NULL, 
    [HolidayDescription] NCHAR(10) NULL, 

    PRIMARY KEY ([CalendarId], [Holiday]), 
    CONSTRAINT [FK_Holidays_Calendar] 
        FOREIGN KEY ([CalendarId]) 
        REFERENCES [dbo].[Calendars]([CalendarId])
        ON DELETE CASCADE
        ON UPDATE CASCADE
)
