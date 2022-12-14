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
-- contract
SET IDENTITY_INSERT dbo.Contracts ON;  
MERGE dbo.Contracts AS t
	USING (SELECT * FROM  (VALUES
		(1, 'Appalto Linee TT 2022 / 2026', 'Appalto per i servizi di linea per gli anni dal 2022 al 2026', '2022-09-01', '2026-08-31')
		) AS src(ContractId, ContractName, ContractDescription, StartDate, EndDate)
	) AS s
	ON  t.ContractId = s.ContractId
	WHEN NOT MATCHED  THEN
		INSERT (ContractId, ContractName, ContractDescription, StartDate, EndDate)
		VALUES (s.ContractId, s.ContractName, s.ContractDescription, s.StartDate, s.EndDate)
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
		(5, 2, 'Invernale Feriale', 'EXC', 1, 0, 0),
		(6, 3, 'Invernale Festivo', 'INC', 1, 0, 0),
		(7, 1, 'Estivo', 'NOP', 0, 0, 0),
		(8, 2, 'Estivo Feriale', 'EXC', 1, 0, 0),
		(9, 3, 'Estivo Festivo', 'INC', 1, 0, 0),
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
	;
SET IDENTITY_INSERT dbo.Calendars OFF;

SET IDENTITY_INSERT [dbo].[CalendarPeriods] ON;
MERGE [dbo].[CalendarPeriods] AS t
	USING  (SELECT * FROM  (VALUES
		(1, 1, '2022-09-01', '2023-08-31', NULL),
		(2, 1, '2023-09-01', '2024-08-31', NULL),
		(3, 1, '2024-09-01', '2025-08-31', NULL),
		(4, 1, '2025-09-01', '2026-08-31', NULL),
		(5, 4, '2022-09-01', '2023-06-30', NULL),
		(6, 4, '2023-09-01', '2024-06-30', NULL),
		(7, 4, '2024-09-01', '2025-06-30', NULL),
		(8, 4, '2025-09-01', '2026-06-30', NULL),
		(9, 7, '2023-07-01', '2023-08-31', NULL),
		(10, 7, '2024-07-01', '2024-08-31', NULL),
		(11, 7, '2025-07-01', '2025-08-31', NULL),
		(12, 7, '2026-07-01', '2026-08-31', NULL)
		) AS src(CalendarPeriodId, CalendarId, StartDate, EndDate, Note)
		) AS s
	ON  t.CalendarPeriodId = s.CalendarPeriodId
	WHEN NOT MATCHED  THEN
		INSERT ([CalendarPeriodId], [CalendarId], [StartDate], [EndDate], [Note]) 
		VALUES (s.CalendarPeriodId, s.CalendarId, s.StartDate, s.EndDate, s.Note)
	;
SET IDENTITY_INSERT [dbo].[CalendarPeriods] OFF;




-- users 
MERGE dbo.Meta_Users AS t
	USING  (SELECT * FROM  (VALUES
		-- Password = Pa$$w0rd
		('Admin', 'PMMc0kYUmuxoB5JB5x6Y9g==', 'Amministratore', '29991231', 0, 'maubatti@gmail.com', NULL)
		) AS src(UserName, PasswordHash, Description, Expiration, MustChangePassword, Email, AssociateId)
		) AS s
	ON  t.UserName = s.UserName
	WHEN NOT MATCHED  THEN
		INSERT VALUES (s.UserName, s.PasswordHash, s.Description, s.Email, s.Expiration, s.MustChangePassword, s.AssociateId)
	WHEN NOT MATCHED  BY SOURCE  THEN
		DELETE
	;
-- users roles
MERGE dbo.Meta_Roles AS t
	USING  (SELECT * FROM  (VALUES
		('Admin', 'ADMIN'),
		('Admin', 'TT'),
		('Admin', 'CTA'),
		('Admin', 'ASSOCIATE')
		) AS src(UserName, RoleId)
		) AS s
	ON  t.UserName = s.UserName 
		AND t.RoleId = s.RoleId
	WHEN NOT MATCHED  THEN
		INSERT VALUES (s.UserName, s.RoleId)
	WHEN NOT MATCHED  BY SOURCE  THEN
		DELETE
	;	;