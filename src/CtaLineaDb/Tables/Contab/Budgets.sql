/* *****************************************************
    Maurizio Battisti
    02/06/2023
    TAbella con la lista dei prventivi salvati
***************************************************** */
CREATE TABLE [dbo].[Budgets]
(
    [BudgetId] int NOT NULL IDENTITY, 
    [BudgetName] VARCHAR(MAX) NOT NULL, 
    [BudgetType] VARCHAR(10) NOT NULL, 
    [BudgetDate] DATE NOT NULL DEFAULT GETDATE(), 
    [ContractId] INT NULL, 
    [StartDate] DATE NULL, 
    [EndDate] DATE NULL, 
    [AssociateId] UNIQUEIDENTIFIER NULL, 
    [CarId] UNIQUEIDENTIFIER NULL, 
    [RunId] UNIQUEIDENTIFIER NULL, 
    [OutOfPEriod] BIT NOT NULL DEFAULT 0, 
    [Suspended] BIT NOT NULL DEFAULT 0, 
    [RplacedCars] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [PK_Budgets] PRIMARY KEY ([BudgetId])
)

