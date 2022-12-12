/* **************************************************************
*   sistema di tariffazione del mezzo della corsa
* questo viene messo solo epr i mezzi titolari e per le sostituzioni
*  la tariffa verrà applicata a partire dalla data indicata
*   se la data è vuota coincide con l'inizio della corsa
************************************************************** */
CREATE TABLE [dbo].[RunCarCosts]
(
	[RunCarCostId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [RunCarId] UNIQUEIDENTIFIER NOT NULL, 
    [StartDAte] DATE NULL DEFAULT NULL, 
    [DayPrice] MONEY NOT NULL DEFAULT 0 , 
    [KmPrice] MONEY NOT NULL DEFAULT 0 , 

    [DayIntegration] MONEY NULL , 
    [DayForfait] MONEY NULL, 
    CONSTRAINT [FK_Costs_RunCar] 
        FOREIGN KEY ([RunCarId]) 
        REFERENCES [dbo].[RunCars](RunCarId)
        ON DELETE CASCADE
        ON UPDATE CASCADE
)
