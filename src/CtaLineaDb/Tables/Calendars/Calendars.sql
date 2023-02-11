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
    [Ordinal]   INT NOT NULL DEFAULT 1000,
    
    [CalendarType] CHAR(3) NOT NULL DEFAULT 'EXC', 

    [Mondays] BIT NOT NULL DEFAULT 0, 
    [Tuesdays] BIT NOT NULL DEFAULT 0, 
    [Wednesdays] BIT NOT NULL DEFAULT 0, 
    [Thursdays] BIT NOT NULL DEFAULT 0, 
    [Fridays] BIT NOT NULL DEFAULT 0, 
    [Saturdays] BIT NOT NULL DEFAULT 0, 
    [Sundays] BIT NOT NULL DEFAULT 0, 

    [PreHolyday] BIT NOT NULL DEFAULT 0, 
    [PostHolyday] BIT NOT NULL DEFAULT 0, 

    CONSTRAINT [FK_CalendarHierarchy] 
        FOREIGN KEY ([BaseCalendarId]) 
        REFERENCES [dbo].[Calendars]([CalendarId])
        ON DELETE NO ACTION
        ON UPDATE NO ACTION, 
    CONSTRAINT [CHK_CalendarType] 
        CHECK ([CalendarType] IN (
        'EXC',   -- Calendario In esclusione
        'INC',   -- Calendario a inclusione
        'IPL',   --Calendario che include solo quanto escluso dal livello precedente
        'NOP'   -- Nessuna operaizone ne di inclusione nè di esclusione
        ))
)
