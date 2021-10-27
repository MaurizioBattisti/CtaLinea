CREATE TABLE [dbo].[CtaServicePEriods]
(
	[ServicePEriodId] INT IDENTITY (1,1) NOT NULL PRIMARY KEY, 
    [ServiceId] INT NOT NULL, 
    [StartDate] DATE NOT NULL, 
    [EndDate] DATE NULL, 
    [CalendarId] VARCHAR(10) NULL, 
    [Monday] BIT NOT NULL DEFAULT 0, 
    [Tuesday] BIT NOT NULL DEFAULT 0, 
    [Wednesday] BIT NOT NULL DEFAULT 0, 
    [Thursday] BIT NOT NULL DEFAULT 0, 
    [Friday] BIT NOT NULL DEFAULT 0, 
    [Saturday] BIT NOT NULL DEFAULT 0, 
    [Sunday] BIT NOT NULL DEFAULT 0, 
    
    CONSTRAINT [FK_Periods_CtaService] 
        FOREIGN KEY (ServiceId) 
        REFERENCES dbo.CtaServices(ServiceId)
        ON DELETE CASCADE, 
    CONSTRAINT [FK_CtaServicePeriods_Calendar] 
        FOREIGN KEY (CalendarId) 
        REFERENCES dbo.Calendars(CalendarId)
        ON DELETE NO ACTION
        ON UPDATE CASCADE
)

GO

CREATE UNIQUE INDEX [IDX_CtaSerivePERiod_Unique] 
    ON [dbo].[CtaServicePEriods] ([ServiceId], [StartDate])
