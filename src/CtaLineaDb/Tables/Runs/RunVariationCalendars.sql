CREATE TABLE [dbo].[RunVariationCalendars]
(
	[RunVariationId] UNIQUEIDENTIFIER NOT NULL ,
    [CalendarId] INT NOT NULL, 
    [Exclusion] BIT NOT NULL DEFAULT(0),
    
    [Monday] BIT NOT NULL DEFAULT 1, 
    [Tuesday] BIT NOT NULL DEFAULT 1, 
    [Wednesday] BIT NOT NULL DEFAULT 1, 
    [Thursday] BIT NOT NULL DEFAULT 1, 
    [Friday] BIT NOT NULL DEFAULT 1, 
    [Saturday] BIT NOT NULL DEFAULT 1, 
    [Sunday] BIT NOT NULL DEFAULT 1, 

    PRIMARY KEY ([RunVariationId], [CalendarId]),
    CONSTRAINT [FK_Calendars_Variation] 
        FOREIGN KEY ([RunVariationId]) 
        REFERENCES [dbo].[RunVariations]([RunVariationId])
        ON DELETE CASCADE
        ON UPDATE CASCADE
)
