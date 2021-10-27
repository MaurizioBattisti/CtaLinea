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
