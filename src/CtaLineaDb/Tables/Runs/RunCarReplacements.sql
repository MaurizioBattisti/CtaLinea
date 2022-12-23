CREATE TABLE [dbo].[RunCarReplacements]
(
	[CarReplacementId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [RunPeriodId] UNIQUEIDENTIFIER NOT NULL, 
    [StartDate] DATE NOT NULL, 
    [EndDate] DATE NOT NULL, 
    [Note] VARCHAR(MAX) NULL
)
