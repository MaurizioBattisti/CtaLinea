/* ***********************************************************
    Maurizio Battisti
    09/02/2023
    Giorni aggiuntivi in cui la corsa va comunque fatta
*********************************************************** */
CREATE TABLE [dbo].[RunAdditionalDays]
(
	[RunId] UNIQUEIDENTIFIER NOT NULL , 
    [Day] DATE NOT NULL, 
    [Note] VARCHAR(MAX) NULL, 
    
    PRIMARY KEY ([RunId], [Day]),
    CONSTRAINT [FK_AdditionalDay_Run] 
        FOREIGN KEY ([RunId]) 
        REFERENCES [dbo].[Runs]([RunId])
        ON DELETE CASCADE
        ON UPDATE CASCADE
)
