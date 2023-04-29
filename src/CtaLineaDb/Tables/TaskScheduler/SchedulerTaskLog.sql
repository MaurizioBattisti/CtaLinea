CREATE TABLE [dbo].[SchedulerTaskLog]
(
	[LogId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TaskId] INT NOT NULL, 
    [Tag] UNIQUEIDENTIFIER NOT NULL, 
    [Code] VARCHAR(50) NULL,
    [Message] VARCHAR(MAX) NULL,
    [Time] DATETIME2 NOT NULL DEFAULT SYSDATETIME(), 
    
    CONSTRAINT [FK_TaskLog] 
        FOREIGN KEY ([TaskId]) 
        REFERENCES [dbo].[SchedulerTasks]([Id])
        ON DELETE CASCADE
)
