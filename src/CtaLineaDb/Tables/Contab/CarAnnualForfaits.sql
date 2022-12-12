CREATE TABLE [dbo].[CarAnnualForfaits]
(
	[CarId] UNIQUEIDENTIFIER NOT NULL , 
    [Year] INT NOT NULL, 
    [Forfait] MONEY NOT NULL DEFAULT 0, 
    [Note] VARCHAR(50) NULL, 
    PRIMARY KEY ([CarId], [Year]), 
    CONSTRAINT [FK_Car_Forfaits] 
        FOREIGN KEY (CarId) 
        REFERENCES [dbo].[Cars]([CarId])
        ON DELETE NO ACTION
        ON UPDATE CASCADE

)
