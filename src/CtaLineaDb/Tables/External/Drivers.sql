CREATE TABLE [dbo].[Drivers]
(
	[DriverId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [AssociateId] UNIQUEIDENTIFIER NOT NULL, 
    [LastName] VARCHAR(50) NOT NULL, 
    [FirstName] VARCHAR(50) NOT NULL, 
    [BsDriverId] VARCHAR(20) NULL, 
    [LicenseNumber] VARCHAR(50) NOT NULL, 
    [LicenceCategory] VARCHAR(100) NULL, 
    [DismissionDate] DATE NULL,
    [Active] BIT NOT NULL DEFAULT 1, 

    CONSTRAINT [FK_DriversAssociate] 
        FOREIGN KEY (AssociateId) 
        REFERENCES dbo.Associates(AssociateId)
        ON DELETE CASCADE
        ON UPDATE CASCADE
)
