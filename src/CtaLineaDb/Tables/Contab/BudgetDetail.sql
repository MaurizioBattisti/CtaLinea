/* *****************************************************
    Maurizio Battisti
    02/06/2023
    Dettaglio dei preventivi salvati
***************************************************** */
CREATE TABLE [dbo].[BudgetDetail]
(
	[BudgetId] INT NOT NULL , 
    [RunId] UNIQUEIDENTIFIER NOT NULL, 
    [RunRowNum] INT NOT NULL, 
    [Day] DATE NOT NULL, 
    [AssociateId] UNIQUEIDENTIFIER NOT NULL, 
    [CarId] UNIQUEIDENTIFIER NOT NULL, 
    [ContractId] INT NOT NULL, 
    [RealKm_Contract] REAL NULL, 
    [RealKm_Extra] REAL NULL, 
    [ContractKm] REAL NULL, 
    [ExtraKm] REAL NULL, 
    [RealKm_Contract_Cost] MONEY NULL, 
    [RealKm_Extra_Cost] MONEY NULL, 
    [DayCost] MONEY NULL, 
    [DayIntegration] MONEY NULL, 
    [DayForfait] MONEY NULL, 
    [MultiRunForfait] MONEY NULL, 
    [MultiRunForfaitAmount] MONEY NULL, 
    [MultiRunForfaitType] CHAR NULL, 
    [MultiRunForfaitName] VARCHAR(MAX) NULL, 
    
    [MultiRunForfaitId] INT NULL, 
    PRIMARY KEY ([BudgetId], [RunId], [RunRowNum], [Day]), 
    CONSTRAINT [FK_Budget_Details] 
        FOREIGN KEY (BudgetId) 
        REFERENCES [dbo].[Budgets]([BudgetId])
        ON DELETE CASCADE
)
