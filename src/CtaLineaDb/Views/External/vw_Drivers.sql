/* ***************************************************
	Maurizio Battisti
	10/08/2023
	vista degli autsti
*************************************************** */

CREATE VIEW [dbo].[vw_Drivers]
AS 
SELECT d.*,
		d.LastName + ' '  + d.FirstName AS CompleteName
	FROM dbo.Drivers d
