CREATE TABLE [dbo].[ImportDetails]
(
	[ImportId] UNIQUEIDENTIFIER NOT NULL , 
    [ServiceId] INT NOT NULL, 
    [Note] VARCHAR(MAX) NULL, 
    CONSTRAINT [PK_ImportDetails] 
        PRIMARY KEY ([ImportId], [ServiceId]), 
    CONSTRAINT [FK_Nodes_Import] 
        FOREIGN KEY (ImportId) 
        REFERENCES dbo.Imports(Id)
        ON DELETE CASCADE, 
    CONSTRAINT [FK_ImportNodes_Service] 
        FOREIGN KEY (ServiceId) 
        REFERENCES dbo.TtServices(ServiceId)
        ON DELETE CASCADE
)
