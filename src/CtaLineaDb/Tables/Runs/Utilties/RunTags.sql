CREATE TABLE [dbo].[RunTags]
(
	[RunId] UNIQUEIDENTIFIER NOT NULL , 
    [TagId] INT NOT NULL, 
    PRIMARY KEY ([RunId], [TagId]), 
    CONSTRAINT [FK_Run_Tags] 
        FOREIGN KEY (RunId) 
        REFERENCES [dbo].[Runs](RunId)
        ON DELETE CASCADE
        ON UPDATE NO ACTION, 
    CONSTRAINT [FK_Tag_Runs] 
        FOREIGN KEY ([TagId]) 
        REFERENCES [dbo].[Tags] (TagId)
        ON DELETE CASCADE
        ON UPDATE NO ACTION
)
