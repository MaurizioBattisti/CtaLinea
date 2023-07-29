/* **************************************************************
	Maurizio Battisti
	30/04/2023
	Esegue una puliza del log delle attività schedulate
************************************************************** */
CREATE PROCEDURE [dbo].[up_CleanTaskLog]
	@DailyRetention		INT = NULL,
	@WeeklyRetention	INT = NULL,
	@MonthlyRetention	INT = NULL
AS
BEGIN
	SET DATEFIRST 1; -- this sets Monday to the first day of the week for the current connection.

	DECLARE @DailyMinDate	DATETIME2;
	DECLARE @WeeklyMinDate	DATETIME2;
	DECLARE @MonthlyMinDate	DATETIME2;
	DECLARE @MaxLimit	DATETIME2;

	IF COALESCE(@DailyRetention, 0) < 1 SET @DailyRetention = 3;
	IF COALESCE(@WeeklyRetention, 0) < 1 SET @WeeklyRetention = 3;
	IF COALESCE(@MonthlyRetention, 0) < 1 SET @MonthlyRetention = 3;

	SET @DailyMinDate = DATEADD(DAY, -1 * @DailyRetention, SYSDATETIME());
	SET @WeeklyMinDate = DATEADD(WEEK, -1 * @WeeklyRetention, SYSDATETIME());
	SET @MonthlyMinDate = DATEADD(MONTH, -1 * @MonthlyRetention, SYSDATETIME());
	SET @MaxLimit = DATEADD(YEAR, -1, SYSDATETIME());

	DELETE l
		FROM dbo.SchedulerTaskLog l
		INNER JOIN dbo.SchedulerTasks t
			ON l.TaskId = t.Id
			WHERE (t.Frequency = 0 AND [Time] < @DailyMinDate)
				OR (t.Frequency = 1 AND [Time] < @WeeklyMinDate)
				OR (t.Frequency = 2 AND [Time] < @MonthlyMinDate)
				;

	-- elimina tutto il rsto
	DELETE FROM dbo.SchedulerTaskLog 
		WHERE [Time] <@MaxLimit;

	RETURN 0
END;
