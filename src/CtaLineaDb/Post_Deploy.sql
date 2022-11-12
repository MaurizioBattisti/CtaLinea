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
-- Run Types
MERGE dbo.RunTypes AS t
	USING  (SELECT * FROM  (VALUES
		('RUN', 'IN ESSERE'),
		('TOASS', 'DA ASSEGNARE'),
		('TOMOD', 'DA MODIFICARE'),
		('END', 'FINITA')
		) AS src(Id, Descr)
		) AS s
	ON  t.RunTypeId = s.Id
	WHEN NOT MATCHED  THEN
		INSERT VALUES (s.Id, s.Descr)
	WHEN NOT MATCHED  BY SOURCE  THEN
		DELETE
	;

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
	;