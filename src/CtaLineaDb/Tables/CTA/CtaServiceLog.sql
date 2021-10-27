CREATE TABLE [dbo].[CtaServiceLog]
(
	[Od] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [ServiceId] INT NOT NULL, 
    [ChangeDate] DATETIME NOT NULL DEFAULT SYSDATETIME(), 
    [User] VARCHAR(100) NOT NULL, 
    [Note] VARCHAR(MAX) NULL, 

    CONSTRAINT [FK_Log_CtaService] 
        FOREIGN KEY (ServiceId) 
        REFERENCES dbo.CtaServices(ServiceId)
        ON DELETE CASCADE
)
