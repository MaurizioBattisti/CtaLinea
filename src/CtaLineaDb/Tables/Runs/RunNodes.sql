CREATE TABLE [dbo].[RunNodes]
(
	[RunNodeId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NewID(), 
    [RunVariationId] UNIQUEIDENTIFIER NOT NULL, 
    [CollectionPointId] VARCHAR(20) NOT NULL, 
    [Hout] TIME NOT NULL, 
    [ProgrNumber] INT NOT NULL DEFAULT 0, 
    [Longitude] REAL NULL, 
    [Latitude] REAL NULL, 

    CONSTRAINT [FK_Nodes_Variation] 
        FOREIGN KEY ([RunVariationId]) 
        REFERENCES [dbo].[RunVariations]([RunVariationId])
        ON DELETE CASCADE
        ON UPDATE CASCADE, 
    CONSTRAINT [FK_Nodes_CollectionPoint] 
        FOREIGN KEY ([CollectionPointId]) 
        REFERENCES [dbo].[CollectionPoints]([CollectionPointId])
        ON DELETE SET NULL
        ON UPDATE CASCADE
)
