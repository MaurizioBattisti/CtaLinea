CREATE TABLE [dbo].[RunNodes]
(
	[RunNodeId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NewID(), 
    [RunVariationId] UNIQUEIDENTIFIER NOT NULL, 
    [CollectionPointId] VARCHAR(20) NOT NULL, 
    [Hour] TIME NOT NULL, 
    [ProgrNumber] INT NOT NULL DEFAULT 0, 
    [CoincidenceDescr] VARCHAR(MAX) NULL, 

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

GO

CREATE INDEX [IDX_RunNodes_AscOrder] 
    ON [dbo].[RunNodes] ([RunVariationId], [Hour] ASC, [ProgrNumber] ASC)
    INCLUDE ([CollectionPointId])
    ;
GO

CREATE INDEX [IDX_RunNodes_DescOrder] 
    ON [dbo].[RunNodes] ([RunVariationId], [Hour] dESC, [ProgrNumber] dESC)
    INCLUDE ([CollectionPointId])
    ;