CREATE TABLE [dbo].[MultiRunForfait]
(
	[ForfaitId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ContractId] INT NOT NULL,
    [ForfaitName] VARCHAR(MAX) NOT NULL, 
    -- il tipo di forfait può essere ti uno dei 3 tipu
    -- D -> giornaliero
    -- M -> Mensile
    -- Y -> Annuale
    [ForfaitTrpe] CHAR(1) NOT NULL DEFAULT 'D', 
    [Amount] MONEY NOT NULL DEFAULT 0, 
    
    CONSTRAINT [chk_MultiRunForfaitTypes] 
        CHECK ([ForfaitTrpe] IN ('D', 'M', 'Y')), 

    CONSTRAINT [FK_Contract_MultiRunForfaits] 
        FOREIGN KEY ([ContractId]) 
        REFERENCES [dbo].[Contracts]([ContractId])
        ON DELETE NO ACTION
)
