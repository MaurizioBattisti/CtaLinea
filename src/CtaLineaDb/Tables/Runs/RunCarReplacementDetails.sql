CREATE TABLE [dbo].[RunCarReplacementDetails]
(
	[CarReplacementId] UNIQUEIDENTIFIER NOT NULL , 
    [OriginaRunCarId] UNIQUEIDENTIFIER NOT NULL, 
    [ReplacedRunCarId] UNIQUEIDENTIFIER NOT NULL, 

    PRIMARY KEY ([CarReplacementId], [ReplacedRunCarId], [OriginaRunCarId]), 
    CONSTRAINT [FK_Details_Replacement] 
        FOREIGN KEY ([CarReplacementId]) 
        REFERENCES [dbo].[RunCarReplacements]([CarReplacementId])
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
