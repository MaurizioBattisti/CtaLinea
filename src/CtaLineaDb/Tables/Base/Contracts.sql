CREATE TABLE [dbo].[Contracts]
(
	[ContractId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ContractName] VARCHAR(200) NOT NULL, 
    [ContractDescription] VARCHAR(MAX) NULL, 
    [StartDate] DATE NOT NULL, 
    [EndDate] DATE NOT NULL
)
