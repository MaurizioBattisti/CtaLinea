/* ***************************************************************************
*	Maurizio bAttisti
*	09/02/2023
*	Restituisce la lista dei calendari a scendere
*************************************************************************** */
CREATE FUNCTION [dbo].[tvf_CalendarTreeTopDown]
(
	@CalendarId int
)
RETURNS @CalendarTree TABLE
(
	[CalendarId] INT NOT NULL PRIMARY KEY, 
    [BaseCalendarId] INT NULL , 

    [CalendarName] VARCHAR(200) NOT NULL, 
    
    [CalendarType] CHAR(3) NOT NULL DEFAULT 'EXC', 
    [Sundays] BIT NOT NULL DEFAULT 0, 
    [PreHolyday] BIT NOT NULL DEFAULT 0, 
    [PostHolyday] BIT NOT NULL DEFAULT 0, 
    [TreeLevel]  int NOT NULL DEFAULT 0
)AS
BEGIN
	WITH CTE_CalendarTree (CalendarId, BaseCalendarId, CalendarName, CalendarType, Sundays, PreHolyday, PostHolyday, TreeLevel) AS
	(
		SELECT CalendarId, BaseCalendarId, CalendarName, CalendarType, Sundays, PreHolyday, PostHolyday, 1 as TreeLevel
		FROM dbo.Calendars
		WHERE CalendarId =@CalendarId
		UNION ALL
		SELECT c.CalendarId, c.BaseCalendarId, c.CalendarName, c.CalendarType, c.Sundays, c.PreHolyday, c.PostHolyday, r.TreeLevel + 1  as TreeLevel
		FROM dbo.Calendars c
			INNER JOIN CTE_CalendarTree  r
				ON r.CalendarId = c.BaseCalendarId
	)
	INSERT @CalendarTree
		(CalendarId, BaseCalendarId, CalendarName, CalendarType, Sundays, PreHolyday, PostHolyday, TreeLevel)
	SELECT CalendarId, BaseCalendarId, CalendarName, CalendarType, Sundays, PreHolyday, PostHolyday, TreeLevel
		FROM CTE_CalendarTree;
	
	RETURN;
END
