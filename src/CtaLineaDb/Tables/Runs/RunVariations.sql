/* ***************************************************************************
* contiene i sottperiodi di una corsa
* la data di inizio del sottoperiodo inidca la data della variazione
* il sottoperido con la data nulla è il sottoperidoo iniziale
* quello NON Cancellabile
*************************************************************************** */
CREATE TABLE [dbo].[RunVariations]
(
	[RunVariationId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [RunId] UNIQUEIDENTIFIER NOT NULL,
    [StartDate] DATE NULL DEFAULT NULL, 
    [LineNumber] INT NULL, 
    [RunNumber] INT NULL,
    [StartTime] TIME NULL, 
    [EndTime] TIME NULL,
    [CalendarId] INT NULL, 


    [RequestedFrequency] VARCHAR(200) NULL, 
    [Km] REAL NULL, 
    [Note] VARCHAR(MAX) NULL, 

    CONSTRAINT [FK_Variations_Run] 
        FOREIGN KEY ([RunId]) 
        REFERENCES [dbo].[Runs]([RunId])
        ON DELETE CASCADE
        ON UPDATE CASCADE, 
    CONSTRAINT [FK_Runs_Calenadr] 
        FOREIGN KEY ([CalendarId]) 
        REFERENCES [dbo].[Calendars]([CalendarId])
        ON DELETE NO ACTION
        ON UPDATE CASCADE
)
