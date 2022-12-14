CREATE TABLE [dbo].[RunSuspensions]
(
	[RunSuspensionId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [RunId] UNIQUEIDENTIFIER NOT NULL, 
    [StartDate] DATE NOT NULL, 
    [EndDate] DATE NOT NULL, 
    [SuspensionTypeId] INT NOT NULL, 
    [SuspensionNote] VARCHAR(MAX) NULL, 

    CONSTRAINT [FK_Suspensions_Run] 
        FOREIGN KEY ([RunId]) 
        REFERENCES [dbo].[Runs]([RunId])
        ON DELETE CASCADE
        ON UPDATE CASCADE,        
    CONSTRAINT [FK_RunSuspensions_Suspension] 
        FOREIGN KEY (SuspensionTypeId) 
        REFERENCES [dbo].[SuspensionTypes](SuspensionTypeId) 
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)
