CREATE TABLE [dbo].[MultiRunForfaitDetails]
(
	[ForfaitId] INT NOT NULL , 
    [RunId] UNIQUEIDENTIFIER NOT NULL, 
    PRIMARY KEY ([ForfaitId], [RunId]), 

    CONSTRAINT [FK_Run_Forfaits] 
        FOREIGN KEY ([RunId]) 
        REFERENCES [dbo].[Runs]([RunId])
        ON DELETE CASCADE,
        
    CONSTRAINT [FK_Forfait_Runs] 
        FOREIGN KEY ([ForfaitId]) 
        REFERENCES [dbo].[MultiRunForfait]([ForfaitId])
        ON DELETE CASCADE
)

GO

CREATE INDEX [IDS_MultiRunForfaitDetails_Unique] ON [dbo].[MultiRunForfaitDetails] ([RunId])
