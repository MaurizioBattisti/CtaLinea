/* *******************************************************************
*   Per ogni corsa viendi indicato  una serie di mezzi
* il valore di CarType è il tipo di mezzo  tra Titolare , Riserva 1  e Riserva 2 e sostituzione
* i codici saranno P -> titolare , S -> Scorta , R => Sostituzione
* P stà per Primary
******************************************************************* */
CREATE TABLE [dbo].[RunCars]
(
	[RunCarId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [RunPeriodId] UNIQUEIDENTIFIER NOT NULL, 
    [CarType] CHAR NOT NULL DEFAULT 'P', 
    [AssociateId] UNIQUEIDENTIFIER NOT NULL, 
    [CarId] UNIQUEIDENTIFIER NOT NULL, 

    [Note] VARCHAR(MAX) NULL, 
    [DriverId] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [FK_Cars_Run] 
        FOREIGN KEY ([RunPeriodId]) 
        REFERENCES [dbo].[RunPeriods]([RunPeriodId])
        ON DELETE CASCADE
        ON UPDATE CASCADE, 
    CONSTRAINT [FL_RunCars_Associate] 
        FOREIGN KEY ([AssociateId]) 
        REFERENCES [dbo].[Associates]([AssociateId])
        ON DELETE NO ACTION
        ON UPDATE NO ACTION, 
    CONSTRAINT [FK_RunCars_Car] 
        FOREIGN KEY ([CarId]) 
        REFERENCES [dbo].[Cars]([CarId])
        ON DELETE NO ACTION
        ON UPDATE NO ACTION, 
    CONSTRAINT [FL_RunCars_Driver]
        FOREIGN KEY ([DriverId])
        REFERENCES [dbo].[Drivers]([DriverId])
        ON DELETE NO ACTION
        ON UPDATE NO ACTION, 
    CONSTRAINT [CHK_RunCar_CarType] 
        CHECK ([CarType] IN (
        'P',    -- Mezzo Titolar
        'S',    -- scorta
        'R'     -- sostituzione
        ))
)
