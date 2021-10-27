CREATE TABLE [dbo].[CtaServiceCars]
(
	[CtaServiceCarId] INT IDENTITY (1,1) NOT NULL PRIMARY KEY, 
    [ServiceId] INT NOT NULL, 
    [CarType] INT NOT NULL DEFAULT 1, 
    [StartDate] DATE NOT NULL, 
    [CarId] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [FK_Cars_CtaSErvice] 
        FOREIGN KEY (ServiceId) 
        REFERENCES dbo.CtaServices(ServiceId)
        ON DELETE CASCADE, 
    CONSTRAINT [FK_CtaServiceCars_Car] 
        FOREIGN KEY (CarId) 
        REFERENCES dbo.Cars(CarId)
        ON DELETE NO ACTION, 
    CONSTRAINT [CHL_CtaSErviceCars_CarType] 
        CHECK (CarType IN ( 1 , 2, 3))
)

GO

CREATE UNIQUE INDEX [IDX_CtaServiceCars_Unique] ON [dbo].[CtaServiceCars] ([ServiceId], [CarType], [StartDate])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'1 = primario, 2 e 3 = seconda e  terza scorta',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CtaServiceCars',
    @level2type = N'COLUMN',
    @level2name = N'CarType'