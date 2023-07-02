CREATE TABLE [dbo].[RunVariationCalendars]
(
	[RunVariationId] UNIQUEIDENTIFIER NOT NULL ,
    [CalendarId] INT NOT NULL, 
    [Exclusion] BIT NOT NULL DEFAULT(0)
    
    PRIMARY KEY ([RunVariationId], [CalendarId]),
    CONSTRAINT [FK_Calendars_Variation] 
        FOREIGN KEY ([RunVariationId]) 
        REFERENCES [dbo].[RunVariations]([RunVariationId])
        ON DELETE CASCADE
        ON UPDATE CASCADE
)
