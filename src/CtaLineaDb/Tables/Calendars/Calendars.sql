/* *****************************************************************
Calenadri
NOTE
- un calendario Inclusivo non pul essere anche Esclusivo
- un calendario Inclusivo non può essere basto su uno Esclusivo e vice-versa
- i flag pre e  post possono essere attivati solo su calendari esclusivi
***************************************************************** */
CREATE TABLE [dbo].[Calendars]
(
	[CalendarId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [BaseCalendarId] INT NULL , 

    [CalendarName] VARCHAR(200) NOT NULL, 
    [Inclusive] BIT NOT NULL DEFAULT 0, 
    
    [Exclusive] BIT NOT NULL DEFAULT 0, 
    [PreHolyday] BIT NOT NULL DEFAULT 0, 
    [PostHolyday] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_CalendarHierarchy] 
        FOREIGN KEY ([BaseCalendarId]) 
        REFERENCES [dbo].[Calendars]([CalendarId])
        ON DELETE NO ACTION
        ON UPDATE CASCADE
)
