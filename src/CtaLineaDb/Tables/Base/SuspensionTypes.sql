/* ****************************************************************************
tiene traccia di tipi di sospensione per poter poi identificare tutte le sospension di un certo tipo
**************************************************************************** */
CREATE TABLE [dbo].[SuspensionTypes]
(
	[SuspensionTypeId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SuspensionTypeDescription] VARCHAR(200) NOT NULL
)
