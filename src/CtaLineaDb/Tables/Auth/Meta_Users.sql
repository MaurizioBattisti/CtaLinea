CREATE TABLE [dbo].[Meta_Users]
(
	[UserName] VARCHAR(128) NOT NULL PRIMARY KEY, 
    [PasswordHash] VARCHAR(MAX) NOT NULL, 
    [Description] VARCHAR(1024) NULL, 
    [Email] VARCHAR(1024) NULL, 
    [Expiration] DATE NULL, 
    [MustChangePassword] BIT NOT NULL DEFAULT 1, 
    [AssociateId] UNIQUEIDENTIFIER NULL, 

    [Interactive] BIT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_Associate_Users] 
        FOREIGN KEY (AssociateId) 
        REFERENCES [dbo].[Associates]([AssociateId])
        ON DELETE SET NULL
        ON UPDATE CASCADE
)
