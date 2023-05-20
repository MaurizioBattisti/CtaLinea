/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
/*
-- contract
SET IDENTITY_INSERT dbo.Contracts ON;  
MERGE dbo.Contracts AS t
	USING (SELECT * FROM  (VALUES
		(1, 'Appalto Linee TT 2022 / 2025', 'Appalto per i servizi di linea per gli anni dal 2022 al 2025', '2022-09-01', '2025-08-31'),,
		(2, 'Appalto Urano 2022 / 2025', 'Appalto per i servizi urbani per gli anni dal 2022 al 2025', '2022-09-01', '2025-08-31'),
		) AS src(ContractId, ContractName, ContractDescription, StartDate, EndDate)
	) AS s
	ON  t.ContractId = s.ContractId
	WHEN NOT MATCHED  THEN
		INSERT (ContractId, ContractName, ContractDescription, StartDate, EndDate)
		VALUES (s.ContractId, s.ContractName, s.ContractDescription, s.StartDate, s.EndDate)
	WHEN MATCHED THEN 
		UPDATE 
			SET ContractName = s.ContractName,
			ContractDescription = s.ContractDescription,
			StartDate = s.StartDate,
			EndDate = s.EndDate
	;
SET IDENTITY_INSERT dbo.Contracts OFF;

-- calendars
SET IDENTITY_INSERT dbo.Calendars ON;
MERGE dbo.Calendars AS t
	USING (SELECT * FROM  (VALUES
		(1, NULL, 'Annuale', 'NOP', 0, 0, 0),
		(2, 1, 'Annuale Feriale', 'EXC', 1, 0, 0),
		(3, 1, 'Annuale Festivo', 'INC', 1, 0, 0),
		(4, 1, 'Invernale', 'NOP', 0, 0, 0),
		(5, 4, 'Invernale Feriale', 'EXC', 1, 0, 0),
		(6, 4, 'Invernale Festivo', 'INC', 1, 0, 0),
		(7, 1, 'Estivo', 'NOP', 0, 0, 0),
		(8, 7, 'Estivo Feriale', 'EXC', 1, 0, 0),
		(9, 7, 'Estivo Festivo', 'INC', 1, 0, 0),
		(10, 5, 'Scolastico', 'EXC', 0, 0, 0),
		(11, 10, 'Non Scolastico', 'IPL', 0, 0, 0),
		(12, 10, 'Prefestivo', 'NOP', 0, 1, 0),
		(13, 10, 'Post Festivo', 'NOP', 0, 0, 1)
		) AS src(CalendarId, BaseCalendarId, CalendarName, CalendarType, Sundays, PreHolyday, PostHolyday)
	) AS s
	ON  t.CalendarId = s.CalendarId
	WHEN NOT MATCHED  THEN
		INSERT (CalendarId, BaseCalendarId, CalendarName, CalendarType, Sundays, PreHolyday, PostHolyday)
		VALUES (s.CalendarId, s.BaseCalendarId, s.CalendarName, s.CalendarType, s.Sundays, s.PreHolyday, s.PostHolyday)
	WHEN MATCHED THEN
		UPDATE
			SET BaseCalendarId = s.BaseCalendarId,
			CalendarName = s.CalendarName,
			CalendarType = s.CalendarType,
			Sundays = s.Sundays,
			PreHolyday = s.PreHolyday,
			PostHolyday = s.PostHolyday
	;
SET IDENTITY_INSERT dbo.Calendars OFF;

SET IDENTITY_INSERT [dbo].[CalendarPeriods] ON;
MERGE [dbo].[CalendarPeriods] AS t
	USING  (SELECT * FROM  (VALUES
		(1, 4, '2022-09-01', '2023-06-30', NULL),
		(2, 4, '2023-09-01', '2024-06-30', NULL),
		(3, 4, '2024-09-01', '2025-06-30', NULL),
		(4, 7, '2023-07-01', '2023-08-31', NULL),
		(5, 7, '2024-07-01', '2024-08-31', NULL),
		(6, 7, '2025-07-01', '2025-08-31', NULL)
		) AS src(CalendarPeriodId, CalendarId, StartDate, EndDate, Note)
		) AS s
	ON  t.CalendarPeriodId = s.CalendarPeriodId
	WHEN NOT MATCHED  THEN
		INSERT ([CalendarPeriodId], [CalendarId], [StartDate], [EndDate], [Note]) 
		VALUES (s.CalendarPeriodId, s.CalendarId, s.StartDate, s.EndDate, s.Note)
	;
SET IDENTITY_INSERT [dbo].[CalendarPeriods] OFF;

-- Calendar Holidays
MERGE dbo.CalendarHolidays AS t
	USING (SELECT * FROM  (VALUES
		(1, '20221101', 'Tutti i Santi'),
		(1, '20221208', 'Immacolata'),
		(1, '20221225', 'Natale'),
		(1, '20221226', 'Santo Stefano'),
		(1, '20230101', 'Capodanno'),
		(1, '20230106', 'Epifania'),
		(1, '20230425', 'La Liberazione'),
		(1, '20230501', 'Festa del lavoro'),
		(1, '20230815', 'Ferragosto'),

		(1, '20231101', 'Tutti i Santi'),
		(1, '20231208', 'Immacolata'),
		(1, '20231225', 'Natale'),
		(1, '20231226', 'Santo Stefano'),
		(1, '20240101', 'Capodanno'),
		(1, '20240106', 'Epifania'),
		(1, '20240425', 'La Liberazione'),
		(1, '20240501', 'Festa del lavoro'),
		(1, '20240815', 'Ferragosto'),

		(1, '20241101', 'Tutti i Santi'),
		(1, '20241208', 'Immacolata'),
		(1, '20241225', 'Natale'),
		(1, '20241226', 'Santo Stefano'),
		(1, '20250101', 'Capodanno'),
		(1, '20250106', 'Epifania'),
		(1, '20250425', 'La Liberazione'),
		(1, '20250501', 'Festa del lavoro'),
		(1, '20250815', 'Ferragosto')

		) AS src(CalendarId, Holiday, HolidayDescription)
	) AS s
	ON  t.CalendarId = s.CalendarId AND t.Holiday = s.Holiday
	WHEN NOT MATCHED  THEN
		INSERT (CalendarId, Holiday, HolidayDescription)
		VALUES (CalendarId, Holiday, HolidayDescription)
	WHEN MATCHED THEN
		UPDATE SET HolidayDescription = s.HolidayDescription
	-- WHEN NOT MATCHED BY SOURCE THEN
		-- DELETE
	;

*/

-- users 
MERGE dbo.Meta_Users AS t
	USING  (SELECT * FROM  (VALUES
		-- Password = Pa$$w0rd
		('Admin', 'PMMc0kYUmuxoB5JB5x6Y9g==', 'Amministratore', '29991231', 0, 'maubatti@gmail.com', NULL, 1)
		) AS src(UserName, PasswordHash, Description, Expiration, MustChangePassword, Email, AssociateId, Interactive)
		) AS s
	ON  t.UserName = s.UserName
	WHEN NOT MATCHED  THEN
		INSERT VALUES (s.UserName, s.PasswordHash, s.Description, s.Email, s.Expiration, s.MustChangePassword, s.AssociateId, s.Interactive)
	-- WHEN NOT MATCHED  BY SOURCE  THEN
		-- DELETE
	;

DELETE  FROM dbo.Meta_Roles
	WHERE UserName = 'Admin';

INSERT INTO dbo.Meta_Roles
	(UserName, RoleId)
	VALUES ('Admin', 'VIEW'),
		('Admin', 'EDIT'),
		('Admin', 'MANAGE'),
		('Admin', 'PLANNING'),
		('Admin', 'COSTS'),
		('Admin', 'DASHBOARD'),
		('Admin', 'TASK'),
		('Admin', 'USERS'),
		('Admin', 'ANAGS')
		;

-- aggiugne i dati dello scheduelr
SET IDENTITY_INSERT [dbo].[SchedulerTasks] ON;  
MERGE [dbo].[SchedulerTasks] AS t
	USING (SELECT * FROM  (VALUES
		(1, 'reloadscheduler', 0, 0, '8:00:00', '20:00:00', '1:00:00', NULL, 1, NULL, NULL,  0),
		(2, 'calcdays', 0, 0, '8:00:00', '20:00:00', '0:05:00', '{ "MaxRuns": 200 }', 1, NULL, NULL, 100),
		(3, 'cleanlog', 0, 0, '20:00:00', '00:00:00', '0:00:00', '{ "DailyRetention": 5, "WeeklyRetention": 4, "MonthlyRetention": 4 }', 1, NULL, NULL, 200)

		) AS src(Id, ActivityId, Frequency, RrequencyMask, StartTime, EndTime, Interval, Arguments, Active, LastStart, LastEnd, [Timeout])
	) AS s
	ON  t.Id = s.Id
	WHEN NOT MATCHED  THEN
		INSERT (Id, ActivityId, Frequency, RrequencyMask, StartTime, EndTime, Interval, Arguments, Active, LastStart, LastEnd, [Timeout])
		VALUES (s.Id, s.ActivityId, s.Frequency, s.RrequencyMask, s.StartTime, s.EndTime, s.Interval, s.Arguments, s.Active, s.LastStart, s.LastEnd, s.[Timeout])
	WHEN MATCHED THEN 
		UPDATE 
			SET ActivityId = s.ActivityId,
			Frequency = s.Frequency,
			RrequencyMask = s.RrequencyMask,

			StartTime = s.StartTime,
			EndTime = s.EndTime,
			Interval = s.Interval,

			Arguments = s.Arguments,
			Active = s.Active,
			LastStart = s.LastStart,
			LastEnd = s.LastEnd,
			[Timeout] = s.[Timeout]
	;
SET IDENTITY_INSERT [dbo].[SchedulerTasks]OFF;