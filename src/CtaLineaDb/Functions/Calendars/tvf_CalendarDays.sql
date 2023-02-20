/* ************************************************************************************************************
*	Maurizio Battisti
*	18/12/2022
*	Calcola il numero dei giorni di un calndario
*	considera anche i calendari di base del calendario stesso
************************************************************************************************************ */
CREATE FUNCTION [dbo].[tvf_CalendarDays]
(
	@CalendarId		 int,
	@StartDate		date,
	@EndDate		date
)
RETURNS @CalendarDays TABLE
(
	Day		DAte NOT NULL PRIMARY KEY
)
AS
BEGIN
	DECLARE @Start	Date = DATEADD(d, -10, @StartDate);
	DECLARE @End	Date = DATEADD(d, 10, @EndDate);
	
	DECLARE @Monday int = DATEPART(dw, '20221226');
	DECLARE @Tuesday int = DATEPART(dw, '20221227');
	DECLARE @Wednesday int = DATEPART(dw, '20221228');
	DECLARE @Thursday  int = DATEPART(dw, '20221229');
	DECLARE @Friday int = DATEPART(dw, '20221230');
	DECLARE @Saturday int = DATEPART(dw, '20221224');
	DECLARE @Sunday int= DATEPART(dw, '20221225');

	DECLARE @CalendarTree AS TABLE 
	(
		[CalendarId] INT NOT NULL PRIMARY KEY, 
		[BaseCalendarId] INT NULL , 

		[CalendarName] VARCHAR(200) NOT NULL, 
    
		[CalendarType] CHAR(3) NOT NULL DEFAULT 'EXC', 
		
		[Sundays] BIT NOT NULL DEFAULT 0, 

		[PreHolyday] BIT NOT NULL DEFAULT 0, 
		[PostHolyday] BIT NOT NULL DEFAULT 0, 
		[TreeLevel]  int NOT NULL DEFAULT 0
	);
	INSERT INTO @CalendarTree
		SELECT * 
			FROM [dbo].[tvf_CalendarTree](@CalendarId);

	DECLARE @Days AS TABLE 
	(
		Day					date PRIMARY KEY,
		Number				int NOT NULL,
		ExcludedByCalendar	bit NOT NULL DEFAULT (0),
		ExcludedByPeriod	bit NOT NULL DEFAULT (0)
	);
	INSERT INTO @Days (Day, Number)
		SELECT d.Day, d.Number
			FROM [dbo].[tvf_Days](@Start, @End) d

	-- variabili locali per ciclare sui calendari e applicare le variaizoni
	DECLARE @Int_CalendarId		INT;
	DECLARE @Int_CalendarType	CHAR(3);
	
	DECLARE @Int_Mondays		BIT
	DECLARE @Int_Tuesdays		BIT
	DECLARE @Int_Wednesdays		BIT
	DECLARE @Int_Thursdays		BIT
	DECLARE @Int_Fridays		BIT
	DECLARE @Int_Saturdays		BIT
	DECLARE @Int_Sundays		BIT
	DECLARE @Int_Pre			BIT
	DECLARE @Int_Post			Bit;
	DECLARE @Int_Lvele			int;

	SELECT @Int_Lvele =  c.TreeLevel
		FROM @CalendarTree c
		WHERE c.BaseCalendarId IS NULL;

	-- SELECT * FROM @CalendarTree;

	WHILE @Int_Lvele > 0
	bEGIN
		SELECT @Int_CalendarId = c.CalendarId,
				@Int_CalendarType = c.CalendarType,
				@Int_Mondays = cc.Mondays,
				@Int_Tuesdays = cc.Tuesdays,
				@Int_Wednesdays = cc.Wednesdays,
				@Int_Thursdays = cc.Thursdays,
				@Int_Fridays = cc.Fridays,
				@Int_Saturdays = cc.Saturdays,
				@Int_Sundays = cc.Sundays,
				@Int_Pre = c.PreHolyday,
				@Int_Post = c.PostHolyday
			FROM @CalendarTree c
			INNER JOIN dbo.Calendars cc
				ON C.CalendarId = cc.CalendarId
			WHERE c.TreeLevel = @Int_Lvele;

		-- esclude i gironi fuori dai periodi dei calendari
		IF EXISTS(SELECT 1 FROM dbo.CalendarPeriods WHERE CalendarId = @Int_CalendarId)
		BEGIN
			WITH CTE_Periods AS
			(
				SELECT cp.CalendarId,
					cp.StartDate, cp.EndDate
				FROM dbo.CalendarPeriods cp
				WHERE cp.CalendarId = @Int_CalendarId
			)
			UPDATE @Days
				SET ExcludedByPeriod = 1
				FROM @Days d
				LEFT JOIN CTE_Periods cp
					ON d.Day BETWEEN cp.StartDate AND cp.EndDate
				WHERE cp.CalendarId IS NULL
					;
		END

		-- controlla il valroe del tipo di calendario
		IF @Int_CalendarType = 'NOP' 
		BEGIN
			-- i valodir i post  e rpe festivo vengono considerati solo sui calendari NO operation
			IF @Int_Pre = 1 OR @Int_Post = 1
			BEGIN
				WITH CTE_DAys AS 
				(
					SELECT 
						*
						FROM @Days d
						WHERE d.ExcludedByCalendar = 0
				)
				UPDATE @Days
					SET ExcludedByCalendar =CASE WHEN @Int_Post * CASE WHEN d_post.Day IS NULL  THEN 1 ELSE 0 END + @Int_Pre * (CASE WHEN d_Pre.Day IS NULL  THEN 1 ELSE 0 END ) < 1 THEN 1 ELSE d.ExcludedByCalendar  END 
					FROM @Days dd
					INNER JOIN CTE_DAys d
						ON dd.Day  = d.Day
					LEFT JOIN CTE_DAys d_Pre 
						ON d.Number +1 = d_Pre.Number 
					LEFT JOIN CTE_DAys d_post 
						ON d.Number - 1 = d_post.Number 
					
			END
		END
		IF @Int_CalendarType = 'EXC' 
		BEGIN
			-- segna com esclusi tutti i giorni del calendario corrente e di quelli sottostanti
			UPDATE @Days 
				SET ExcludedByCalendar = 1
				FROM @Days d
				INNER JOIN dbo.CalendarHolidays h
					ON d.Day = h.Holiday
				INNER JOIN @CalendarTree c
					ON h.CalendarId = c.CalendarId
					WHERE c.TreeLevel >= @Int_Lvele;
			-- se necessario esclude anche le domeniche
			IF @Int_Mondays  = 1 
			BEGIN 
				UPDATE @Days 
					SET ExcludedByCalendar = 1
				WHERE  DATEPART(WEEKDAY, Day) = @Monday ;
			END
			IF @Int_Tuesdays  = 1 
			BEGIN 
				UPDATE @Days 
					SET ExcludedByCalendar = 1
				WHERE  DATEPART(WEEKDAY, Day) = @Tuesday ;
			END
			IF @Int_Wednesdays  = 1 
			BEGIN 
				UPDATE @Days 
					SET ExcludedByCalendar = 1
				WHERE  DATEPART(WEEKDAY, Day) =@Wednesday;
			END
			IF @Int_Thursdays  = 1 
			BEGIN 
				UPDATE @Days 
					SET ExcludedByCalendar = 1
				WHERE  DATEPART(WEEKDAY, Day) =@Thursday;
			END
			IF @Int_Fridays  = 1 
			BEGIN 
				UPDATE @Days 
					SET ExcludedByCalendar = 1
				WHERE  DATEPART(WEEKDAY, Day) =@Friday;
			END
			IF @Int_Saturdays  = 1 
			BEGIN 
				UPDATE @Days 
					SET ExcludedByCalendar = 1
				WHERE  DATEPART(WEEKDAY, Day) =@Saturday;
			END
			IF @Int_Sundays  = 1 
			BEGIN 
				UPDATE @Days 
					SET ExcludedByCalendar = 1
				WHERE  DATEPART(WEEKDAY, Day) = @Sunday ;
			END
		END
		IF @Int_CalendarType = 'IPL' 
		BEGIN
			-- nverte tutto il calendario già calcolato
			UPDATE @Days 
				SET ExcludedByCalendar = (CASE ExcludedByCalendar WHEN 0 THEN 1 ELSE 0 END)
				;
		END
		IF @Int_CalendarType = 'INC' 
		BEGIN
			-- include solo quanto richiesto
			WITH CTE_Holidays AS
			(
				SELECT h.*
				FROM dbo.CalendarHolidays h
				INNER JOIN @CalendarTree c
					ON h.CalendarId = c.CalendarId
					AND c.TreeLevel >= @Int_Lvele
			)
			UPDATE @Days 
				SET ExcludedByCalendar = 
					(CASE 
						-- se il giorno è presente  nella lista lo include
						WHEN h.Holiday IS NOT NULL THEN  0
						-- se è richiesot di includere le domeniche  e il giorno è una domenica lo include
						WHEN @Int_Mondays = 1 AND DATEPART(WEEKDAY, d.Day) = @Monday  THEN 0
						WHEN @Int_Tuesdays = 1 AND DATEPART(WEEKDAY, d.Day) = @Tuesday  THEN 0
						WHEN @Int_Wednesdays = 1 AND DATEPART(WEEKDAY, d.Day) = @Wednesday  THEN 0
						WHEN @Int_Thursdays = 1 AND DATEPART(WEEKDAY, d.Day) = @Thursday  THEN 0
						WHEN @Int_Fridays = 1 AND DATEPART(WEEKDAY, d.Day) = @Friday  THEN 0
						WHEN @Int_Saturdays = 1 AND DATEPART(WEEKDAY, d.Day) = @Saturday  THEN 0
						WHEN @Int_Sundays = 1 AND DATEPART(WEEKDAY, d.Day) = @Sunday  THEN 0
						ELSE 1
					END)
				FROM @Days d
				LEFT JOIN CTE_Holidays h
				-- LEFT JOIN dbo.CalendarHolidays h
					ON d.Day = h.Holiday
					;

		END

		SET @Int_Lvele = @Int_Lvele -1;
	END

	INSERT @CalendarDays
		SELECT d.Day
			--,d.Number, d.ExcludedByCalendar, d.ExcludedByPeriod
		FROM @Days d
		WHERE d.Day BETWEEN @StartDAte AND @EndDate
			AND d.ExcludedByCalendar = 0
			AND d.ExcludedByPeriod = 0
		;
	RETURN
END