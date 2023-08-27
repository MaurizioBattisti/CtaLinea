/* **********************************************************************************
    Maurizio Battisti
    27/08/2023
    tabella che contiene i dati effettivi di effettuazione dei servizi di tipo elastibusd
********************************************************************************** */
CREATE TABLE [dbo].[RunElastibusDays]
(
	[RunId] UNIQUEIDENTIFIER NOT NULL , 
    [Day] DATE NOT NULL, 
    [Km] REAL NOT NULL,
    [PeopleCount] INT NOT NULL DEFAULT(0),
    [Note] VARCHAR(MAX) NULL, 

    PRIMARY KEY ([RunId], [Day]),
    CONSTRAINT [FK_ElastibusDay_Run] 
        FOREIGN KEY ([RunId]) 
        REFERENCES [dbo].[Runs]([RunId])
        ON DELETE CASCADE
        ON UPDATE CASCADE
)

