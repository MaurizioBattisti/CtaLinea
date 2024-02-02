CREATE TABLE [dbo].[SchedulerTasks]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ActivityId] VARCHAR(200) NOT NULL, 
    [Description] Varchar(200) NULL,
    [Frequency] INT NOT NULL DEFAULT 0, 
    [RrequencyMask] INT NOT NULL DEFAULT 0, 
    [StartTime] TIME NOT NULL DEFAULT '0:00:00' ,  
    [EndTime] TIME NOT NULL DEFAULT '0:00:00', 
    [Interval] TIME NOT NULL DEFAULT '0:00:00', 
    [Arguments] VARCHAR(MAX) NULL, 
    [Active] BIT NOT NULL DEFAULT 1, 
    [LastStart] DATETIME NULL, 
    [LastEnd] DATETIME NULL, 
    [Timeout] INT NOT NULL DEFAULT 0
)
