CREATE TABLE [dbo].[CalendaSchoolYearRanges]
(
	[CalendarId] VARCHAR(10) NOT NULL , 
    [SchoolYear] INT NOT NULL, 
    [StartDAte] DATE NOT NULL, 
    [EndDate] DATE NOT NULL, 

    CONSTRAINT [PK_CalendaSchoolYearRanges] 
        PRIMARY KEY ([CalendarId], [SchoolYear]), 
    CONSTRAINT [FK_RangesCalendar] 
        FOREIGN KEY (CalendarId) 
        REFERENCES dbo.Calendars(CalendarId)
        ON DELETE CASCADE
        ON UPDATE CASCADE
)
