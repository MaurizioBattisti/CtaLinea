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
    [RunNumber] VARCHAR(MAX) NULL,
    [Path] VARCHAR(MAX) NOT NULL, 
    [StartTime] TIME NULL, 
    [EndTime] TIME NULL,

    [Monday] BIT NOT NULL DEFAULT 1, 
    [Tuesday] BIT NOT NULL DEFAULT 1, 
    [Wednesday] BIT NOT NULL DEFAULT 1, 
    [Thursday] BIT NOT NULL DEFAULT 1, 
    [Friday] BIT NOT NULL DEFAULT 1, 
    [Saturday] BIT NOT NULL DEFAULT 1, 
    [Sunday] BIT NOT NULL DEFAULT 1, 

    [RequestedFrequency] VARCHAR(200) NULL, 
    [RequestedCapacity] INT NOT NULL DEFAULT 0, 
    [Km] REAL NULL, 
    [Note] VARCHAR(MAX) NULL, 

    CONSTRAINT [FK_Variations_Run] 
        FOREIGN KEY ([RunId]) 
        REFERENCES [dbo].[Runs]([RunId])
        ON DELETE CASCADE
        ON UPDATE CASCADE
)
