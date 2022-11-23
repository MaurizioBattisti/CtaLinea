CREATE TABLE [dbo].[RunCarReplacementDetails]
(
	[CarReplacement] UNIQUEIDENTIFIER NOT NULL , 
    [OriginaRunCarId] UNIQUEIDENTIFIER NOT NULL, 
    [ReplacedRunCarId] UNIQUEIDENTIFIER NOT NULL, 

    PRIMARY KEY ([CarReplacement], [ReplacedRunCarId], [OriginaRunCarId]), 
    CONSTRAINT [FK_Details_Replacement] 
        FOREIGN KEY ([CarReplacement]) 
        REFERENCES [dbo].[RunCarReplacements]([CarReplacement])
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT [FK_Details_OriginalRunCar] 
        FOREIGN KEY ([OriginaRunCarId]) 
        REFERENCES [dbo].[RunCars]([RunCarId])
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT [FK_Details_ReplacedRunCar] 
        FOREIGN KEY ([ReplacedRunCarId]) 
        REFERENCES [dbo].[RunCars]([RunCarId])
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)
