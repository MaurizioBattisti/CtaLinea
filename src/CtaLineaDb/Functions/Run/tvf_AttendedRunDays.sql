/* *****************************************************************************
	Maurizio Battisti
	28/12/2022
	Calcola il numero di giorni attesi per una determinata corsa
	le sospensioni vengono idncate con un glaìg
******************************************************************************/
CREATE FUNCTION [dbo].[tvf_AttendedRunDays]
(
	@RunId	uniqueidentifier
)
RETURNS @Tbl_Days TABLE
(
		RunId		uniqueidentifier NOT NULL,
		Day			datetime NOT NULL,
		WeekDay		int NOT NULL,
		Suspended	bit NOT NULL DEFAULT 0,

		PRIMARY KEY (RunId, Day)
)
AS
BEGIN
	-- calcola i valori dei grioni della settimana
	DECLARE @Monday int = DATEPART(dw, '20221226');
	DECLARE @Tuesday int = DATEPART(dw, '20221227');
	DECLARE @Wednesday int = DATEPART(dw, '20221228');
	DECLARE @Thursday  int = DATEPART(dw, '20221229');
	DECLARE @Friday int = DATEPART(dw, '20221230');
	DECLARE @Saturday int = DATEPART(dw, '20221224');
	DECLARE @Sunday int= DATEPART(dw, '20221225');

	DECLARE @StartDate Date;
	DECLARE @EndDAte AS DAte;

	DECLARE @Tbl_Calendars AS TABLE
	(
		id				INT NOT NULL PRIMARY KEY,
		StartDate		date NOT NULL,
		EndDate			date NOT NULL,
		CalendarId		int NOT NULL,
		RunVariantId	uniqueidentifier NOT NULL
	); 
	
	SELECT @StartDate = CASE WHEN  COALESCE(r.StartDate, c.StartDate) < c.StartDate THEN C.StartDAte ELSE COALESCE(r.StartDate, c.StartDate) END,
			@EndDAte = CASE WHEN  COALESCE(r.EndDate, c.EndDate) > c.EndDate THEN C.EndDate ELSE COALESCE(r.EndDate, c.EndDate) END
		FROM dbo.Runs r
		INNER JOIN Dbo.Contracts c
			ON R.ContractId = c.ContractId
		WHERE r.RunId = @RunId;

	WITH CTE_RunCalendars AS
	(
		SELECT ROW_NUMBER() OVER (ORDER BY COALESCE(v.StartDate, @StartDate) ) AS Num,
				COALESCE(v.StartDate, @StartDate) AS StartDate,
				v.CalendarId,
				v.RunVariationId
			FROM dbo.RunVariations v
			WHERE v.RunId = @RunId
	)
	INSERT INTO @Tbl_Calendars
		(id, StartDate, EndDate, CalendarId, RunVariantId)

		SELECT c.num, 
				c.StartDate,
				COALESCE (DATEADD(d, -1,  ce.StartDate), @EndDAte) AS EndDAte,
				c.CalendarId,
				c.RunVariationId
			FROM CTE_RunCalendars c
			LEFT JOIN  CTE_RunCalendars ce
				ON ce.Num = (c.num + 1)
		;

	-- calcola i giorni come da clanedario e variante della corsa
	DECLARE @Id					int = 1;
	DECLARE @Cal_id				int;
	DECLARE @Cal_Start			date;
	DECLARE @Cal_End			date;
	DECLARE @Cal_VariantID	uniqueidentifier;
	
	DECLARE @MaxID int;
	SELECT @MaxID = MAX(Id) FROM @Tbl_Calendars;

	WHILE @Id <= @MaxID
	BEGIN
		SELECT 
				@Cal_id = c.CalendarId,
				@Cal_Start = c.StartDate,
				@Cal_End = c.EndDate,
				@Cal_VariantID = c.RunVariantId
			FROM @Tbl_Calendars c
			WHERE c.id = @Id;
		SET @Id = @Id + 1;

		-- inserisce le date nella tabella dei gironi della corsa
		WITH CTE_Days AS (
			SELECT @RunId As RunId, 
					d.Day AS Day, 
					DATEPART(dw, day)  AS WeekDay
				FROM dbo.tvf_CalendarDays(@Cal_id, @Cal_Start, @Cal_End) d
		)
		INSERT INTO @Tbl_Days
				(RunId, Day, WeekDay)
			SELECT d.RunId, 
					d.Day,
					d.WeekDay
				FROM CTE_Days d
				CROSS JOIN Dbo.RunVariations v
				WHERE V.RunVariationId = @Cal_VariantID
					AND (
						(d.WeekDay = @Monday AND  v.Monday = 1)
						OR (d.WeekDay = @Tuesday AND  v.Tuesday = 1)
						OR (d.WeekDay = @Wednesday AND  v.Wednesday = 1)
						OR (d.WeekDay = @Thursday AND  v.Thursday = 1)
						OR (d.WeekDay = @Friday AND  v.Friday = 1)
						OR (d.WeekDay = @Saturday AND  v.Saturday = 1)
						OR (d.WeekDay = @Sunday AND  v.Sunday = 1)
						)
				;
	END

	-- segna i giorni sospesi come "sospesi" appunto
	UPDATE @Tbl_Days 
		SET Suspended = 1
		FROM @Tbl_Days d
			INNER JOIN dbo.RunSuspensions s
			ON D.RunId = s.RunId
		WHERE d.Day BETWEEN s.StartDate AND s.EndDate
		;

	RETURN;
END
