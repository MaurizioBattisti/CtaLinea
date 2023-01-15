CREATE TABLE [dbo].[RunDays]
(
	[RunId]				uniqueidentifier NOT NULL,
	[Day]				datetime NOT NULL,
	[CarNum]			int DEFAULT(1),

	[RunVariationId]	uniqueidentifier NOT NULL,
	[RunPeriodId]		uniqueidentifier,
	[WeekDay]			int NOT NULL,
	[Suspended]			bit NOT NULL DEFAULT 0,
	[OutOfPeriod]		bit NOT NULL DEFAULT 0,

	[RunCarId]			uniqueidentifier,
	[OriginalRunCarId]	uniqueidentifier,
	[Replaced]			bit NOT NULL DEFAULT(0),

	PRIMARY KEY ([RunId], [Day], [CarNum]), 
    CONSTRAINT [FK_Run_Days] 
		FOREIGN KEY (RunId) 
		REFERENCES [dbo].[Runs]([RunId])
		ON DELETE CASCADE
		ON UPDATE NO ACTION
)
