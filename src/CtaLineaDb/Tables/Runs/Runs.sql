CREATE TABLE [dbo].[Runs]
(
	[RunId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [ContractId] INT NOT NULL, 
    [Extra] BIT NOT NULL DEFAULT 0, 
    [ContractRowNumber] INT NULL,
    [StartDate] DATE NULL, 
    [EndDate] DATE NULL, 
    [RequestedDays] INT NULL, 
    [Note] VARCHAR(MAX) NULL, 

    -- Identificativo univoco autoincrementante non più modificabile di una corsa
    [CtaRunId] INT NOT NULL IDENTITY, 

    [RunName] VARCHAR(MAX) NULL, 
    CONSTRAINT [FK_Funs_Contract] 
        FOREIGN KEY (ContractId) 
        REFERENCES [dbo].[Contracts](ContractId)
        ON DELETE CASCADE
        ON UPDATE CASCADE
)

GO

CREATE INDEX [IDX_Run_Unique] 
    ON [dbo].[Runs] ([CtaRunId])
