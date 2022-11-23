CREATE TABLE [dbo].[Runs]
(
	[RunId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [ContractId] INT NOT NULL, 
    [Extra] BIT NOT NULL DEFAULT 0, 
    [ContractRowNumber] VARCHAR(20) NULL,
    [StartDate] DATE NOT NULL, 
    [EndDate] DATE NOT NULL, 
    [RequestedDays] INT NULL, 
    [Note] VARCHAR(MAX) NULL, 

    CONSTRAINT [FK_Funs_Contract] 
        FOREIGN KEY (ContractId) 
        REFERENCES [dbo].[Contracts](ContractId)
        ON DELETE CASCADE
        ON UPDATE CASCADE
)
