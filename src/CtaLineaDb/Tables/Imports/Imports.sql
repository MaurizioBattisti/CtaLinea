CREATE TABLE [dbo].[Imports]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [ImportDescr] VARCHAR(20) NOT NULL, 
    [Note] VARCHAR(MAX) NULL, 
    [User] VARCHAR(100) NULL, 
    [ImportStartDate] DATETIME NOT NULL DEFAULT SYSDATETIME(), 
    [LastUpdateDate] DATETIME NOT NULL DEFAULT SYSDATETIME(), 
    [ImportStatus] VARCHAR(20) NOT NULL DEFAULT 'PROGRESS', 
    [ImportedElements] INT NOT NULL DEFAULT 0, 
    [ProcessedElements] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [CHK_Imports_ImportStatus] CHECK (
        [ImportStatus] IN (
            'PROGRESS',
            'COMPLETED',
            'ABORTED',
            'ERROR'
        ))
)
