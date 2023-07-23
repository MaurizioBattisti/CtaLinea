/* *******************************************************
    Maurizio Battisti
    21/07/2023
    Mantiene delle note interne legate alla corsa
******************************************************* */
CREATE TABLE [dbo].[RunInternalNotes]
(
	[RunId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Note] VARCHAR(MAX) NOT NULL, 

    CONSTRAINT [FK_Run_Notes] 
        FOREIGN KEY (RunId) 
        REFERENCES dbo.Runs(RunId)
        ON DELETE CASCADE
)
