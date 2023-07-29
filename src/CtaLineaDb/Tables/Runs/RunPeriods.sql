CREATE TABLE [dbo].[RunPeriods]
(
	[RunPeriodId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [RunId] UNIQUEIDENTIFIER NOT NULL, 
    [StartDate] DATE NULL, 
    [EndDate] DATE NULL, 
    [Monday] BIT NOT NULL DEFAULT 1, 
    [Tuesday] BIT NOT NULL DEFAULT 1, 
    [Wednesday] BIT NOT NULL DEFAULT 1, 
    [Thursday] BIT NOT NULL DEFAULT 1, 
    [Friday] BIT NOT NULL DEFAULT 1, 
    [Saturday] BIT NOT NULL DEFAULT 1, 
    [Sunday] BIT NOT NULL DEFAULT 1, 
    [Note] VARCHAR(MAX) NULL, 
    
    [RepeatType] CHAR NOT NULL DEFAULT 'A', 
    [RepeatPattern] VARCHAR(MAX) NULL, 

    CONSTRAINT [FK_Periods_Run] 
        FOREIGN KEY ([RunId]) 
        REFERENCES [dbo].[Runs] ([RunId])
        ON DELETE CASCADE
        ON UPDATE CASCADE
)
