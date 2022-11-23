/* *******************************************************************
*   Per ogni corsa viendi indicato  una serie di mezzi
* il valore di CarType è il tipo di mezzo  tra Titolare , Riserva 1  e Riserva 2 e sostituzione
* i codici saranno P -> titolare , 1 -> riserva 1 e 2 Riserva 2, R => Sostituzione
* P stà per Primary
******************************************************************* */
CREATE TABLE [dbo].[RunCars]
(
	[RunCarId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [RunPEriodId] UNIQUEIDENTIFIER NOT NULL, 
    [CarType] CHAR NOT NULL DEFAULT 'P', 
    [AssociateId] UNIQUEIDENTIFIER NOT NULL, 
    [CarId] UNIQUEIDENTIFIER NOT NULL, 

    CONSTRAINT [FK_Cars_Run] 
        FOREIGN KEY ([RunPEriodId]) 
        REFERENCES [dbo].[RunPeriods](RunPEriodId)
        ON DELETE CASCADE
        ON UPDATE CASCADE, 
    CONSTRAINT [FL_RunCars_Associate] 
        FOREIGN KEY ([AssociateId]) 
        REFERENCES [dbo].[Associates]([AssociateId])
        ON DELETE NO ACTION
        ON UPDATE CASCADE, 
    CONSTRAINT [FK_RunCars_Car] 
        FOREIGN KEY ([CarId]) 
        REFERENCES [dbo].[Cars]([CarId])
        ON DELETE NO ACTION
        ON UPDATE CASCADE, 
    CONSTRAINT [CHK_RunCar_CarType] 
        CHECK ([CarType] IN ('P', '1', '2', 'R'))
)
