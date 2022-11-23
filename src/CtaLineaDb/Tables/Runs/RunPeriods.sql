CREATE TABLE [dbo].[RunPeriods]
(
	[RunPEriodId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [RunId] UNIQUEIDENTIFIER NOT NULL, 
    [StartDate] DATE NULL, 
    [EndDate] DATE NULL, 
    [Monday] BIT NOT NULL DEFAULT 1, 
    [Tuesday] BIT NOT NULL DEFAULT 1, 
    [Wednesday] BIT NOT NULL DEFAULT 1, 
    [Thursday] BIT NOT NULL DEFAULT 1, 
    [Friday] BIT NOT NULL DEFAULT 1, 
    [Saturday] BIT NOT NULL DEFAULT 0, 
    [Sunday] BIT NOT NULL DEFAULT 0, 
    [RequestPrimaryCarCount] INT NOT NULL DEFAULT 1, 

    [Note] VARCHAR(MAX) NULL, 
    CONSTRAINT [FK_Periods_Run] 
        FOREIGN KEY ([RunId]) 
        REFERENCES [dbo].[Runs] ([RunId])
        ON DELETE CASCADE
        ON UPDATE CASCADE
)
