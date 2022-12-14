CREATE TABLE [dbo].[RunNodes]
(
	[RunNodeId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NewID(), 
    [RunVariationId] UNIQUEIDENTIFIER NOT NULL, 
    [CollectionPointId] VARCHAR(20) NOT NULL, 
    [Hour] TIME NOT NULL, 
    [ProgrNumber] INT NOT NULL DEFAULT 0, 

    CONSTRAINT [FK_Nodes_Variation] 
        FOREIGN KEY ([RunVariationId]) 
        REFERENCES [dbo].[RunVariations]([RunVariationId])
        ON DELETE CASCADE
        ON UPDATE CASCADE, 
    CONSTRAINT [FK_Nodes_CollectionPoint] 
        FOREIGN KEY ([CollectionPointId]) 
        REFERENCES [dbo].[CollectionPoints]([CollectionPointId])
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
)
