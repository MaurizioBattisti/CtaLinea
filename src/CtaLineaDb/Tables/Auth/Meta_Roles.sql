CREATE TABLE [dbo].[Meta_Roles]
(
	[UserName] VARCHAR(128) NOT NULL , 
    [RoleId] VARCHAR(32) NOT NULL, 

    PRIMARY KEY ([UserName], [RoleId]),
        CONSTRAINT [FK_Roles_User] 
        FOREIGN KEY ([UserName]) 
        REFERENCES [dbo].[Meta_Users]([UserName])
        ON DELETE CASCADE
        ON UPDATE CASCADE
)
