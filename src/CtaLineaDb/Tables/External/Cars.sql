CREATE TABLE [dbo].[Cars]
(
	[CarId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [AssociateId] UNIQUEIDENTIFIER NOT NULL, 
    [Description] VARCHAR(200) NOT NULL, 
    [NrSittings] INT NOT NULL,
    [RegNumber] VARCHAR(50) NOT NULL, 
    [BsCarId] VARCHAR(20) NULL, 

    -- dati nuovi da recuperare da qualche parte
    [ChassisNumber] VARCHAR(50) NULL,    -- numero di telaio
    [FirstRegistration] DATE NULL DEFAULT NULL,

    [DiscontinuationDate] DATE NULL DEFAULT NULL,
    [PrimaryCar] BIT NOT NULL DEFAULT 1, 
    [SpareCar] BIT NOT NULL DEFAULT 0, 
    [Active] BIT NOT NULL DEFAULT 1, 
    
    CONSTRAINT [FK_Cars_Associate] 
        FOREIGN KEY (AssociateId) 
        REFERENCES dbo.Associates(AssociateId) 
        ON DELETE CASCADE
        ON UPDATE CASCADE
)
