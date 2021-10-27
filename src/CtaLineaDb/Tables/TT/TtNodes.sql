CREATE TABLE [dbo].[TtNodes]
(
	[NodeId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [ServiceId] INT NOT NULL, 
    [CollectionPointId] VARCHAR(20) NOT NULL, 
    [Description] VARCHAR(200) NULL, 
    [ProgrNumber] INT NOT NULL DEFAULT 0, 
    [Hour] TIME NULL, 
    [MinutesStop] INT NULL, 
    [ArrivedAtHour] TIME NULL, 
    [Longitude] DECIMAL(18, 14) NULL, 
    [Latitude] DECIMAL(18, 14) NULL,     

    CONSTRAINT [FK_Hours_Service] 
        FOREIGN KEY (ServiceId) 
        REFERENCES dbo.TtServices(ServiceId)
        ON DELETE CASCADE
)
