
/* *****************************************************************************
	Maurizio Battisti
	28/12/2022
	Calcola il numero di giorni attesi per una determinata corsa
	le sospensioni vengono idncate con un glaìg
******************************************************************************/
CREATE FUNCTION [dbo].[tvf_RunDays]
(
	@RunId	uniqueidentifier
)
RETURNS @Tbl_Days TABLE
(
	RunId			uniqueidentifier NOT NULL,
	Day				datetime NOT NULL,

	RunVariationId	uniqueidentifier NOT NULL,
	RunPeriodId		uniqueidentifier,
	WeekDay			int NOT NULL,
	Suspended		bit NOT NULL DEFAULT 0,
	OutOfPeriod		bit NOT NULL DEFAULT 0,

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
	DECLARE @RunEndDAte AS DAte;

	DECLARE @Tbl_Variants AS TABLE 
	(
		RunVariantId	uniqueidentifier PRIMARY KEY,
		StartDate		date NOT NULL,
		EndDate			date NOT NULL,
		LastOne			bit DEFAULT 0
	);
	
	SELECT @StartDate = CASE WHEN  COALESCE(r.StartDate, c.StartDate) < c.StartDate THEN C.StartDAte ELSE COALESCE(r.StartDate, c.StartDate) END,
			@RunEndDAte = CASE WHEN  COALESCE(r.EndDate, c.EndDate) > c.EndDate THEN C.EndDate ELSE COALESCE(r.EndDate, c.EndDate) END,
			@EndDAte = CASE 
					WHEN r.Extra = 0 THEN
						-- se è una corsa da capitolato tiene conto di una data più lunga per avere i giorni come se fossero
						c.EndDate
					ELSE
						(CASE WHEN  COALESCE(r.EndDate, c.EndDate) > c.EndDate THEN C.EndDate ELSE COALESCE(r.EndDate, c.EndDate) END)
					END
		FROM dbo.Runs r
		INNER JOIN Dbo.Contracts c
			ON R.ContractId = c.ContractId
		WHERE r.RunId = @RunId;

	-- crea la tabella con  le varianti con inizio e  fine del loro periodo
	WITH CTE_RunVariants  AS
	(
		SELECT ROW_NUMBER() OVER (ORDER BY COALESCE(v.StartDate, @StartDate) ) AS Num,
				COALESCE(v.StartDate, @StartDate) AS StartDate,
				v.RunVariationId
			FROM dbo.RunVariations v
			WHERE v.RunId = @RunId
	)
	INSERT INTO @Tbl_Variants
		(StartDate, EndDate, RunVariantId)
		SELECT c.StartDate,
				COALESCE (DATEADD(d, -1,  ce.StartDate), @EndDAte) AS EndDAte,
				c.RunVariationId
			FROM CTE_RunVariants c
			LEFT JOIN  CTE_RunVariants ce
				ON ce.Num = (c.num + 1)
		;
	WITH CTE_LastVar AS 
	(
		SELECT v.RunVariantId,
				ROW_NUMBER () OVER (ORDER BY v.EndDate dESC) AS num
			FROM @Tbl_Variants  v
	)
	UPDATE @Tbl_Variants 
		SET LastOne = 1
		FROM @Tbl_Variants v
		INNER JOIN CTE_LastVar v2
			ON v.RunVariantId = v2.RunVariantId
			AND v2.num = 1;
	
	-- calcola i gironi di tutti i calendari
	-- prendendo da tutti quelli nella lista di ogni calendario della variante
	DECLARE @Tbl_Var_Days AS TABLE
	(
		RunVariantId	uniqueidentifier NOT NULL,
		Day				date NOT NULL,
		WeekDay			int NOT NULL,
		OutOfPeriod		bit NOT NULL DEFAULT 0,

		PRIMARY KEY (RunVariantId, Day)
	);

	-- dichiara il cursore per eseguire il calcolo dei calendari
	DECLARE Var_Curr CURSOR LOCAL FORWARD_ONLY 
		FOR	
			SELECT v.RunVariantId,
					vc.CalendarId,
					v.StartDate,
					v.EndDate,
					v.LastOne
				FROM @Tbl_Variants v
				INNER JOIN dbo.RunVariationCalendars vc
					ON V.RunVariantId = vc.RunVariationId
		;

	DECLARE @v_RunVariationId	uniqueidentifier;
	DECLARE @v_CalendarId		int;
	DECLARE @v_StartDate		date;
	DECLARE @v_EndDate			date;
	DECLARE @v_LastOne			bit;
	OPEN Var_Curr;
	FETCH NEXT FROM Var_Curr INTO 
			@v_RunVariationId, @v_CalendarId, 
			@v_StartDate, @v_EndDate,
			@v_LastOne;

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		DECLARE @Limit_EndDate	date = @v_EndDate;
		IF @v_LastOne = 1 
		BEGIN
			SET @Limit_EndDate = @EndDAte;
		END;

		-- inserisce le date nella tabella dei gironi della corsa
		WITH CTE_Days AS (
			SELECT @v_RunVariationId AS RunVariantId,
					d.Day AS Day, 
					DATEPART(dw, day) AS WeekDay
				FROM dbo.tvf_CalendarDays(@v_CalendarId, 
						@v_StartDate, 
						@Limit_EndDate) d
		)
		INSERT INTO @Tbl_Var_Days
				(RunVariantId, Day, WeekDay, OutOfPeriod)
			SELECT d.RunVariantId, 
					d.Day,
					d.WeekDay,
					CASE WHEN d.day > @v_EndDate AND @v_LastOne = 1 AND d.day <= @EndDAte THEN 1 ELSE 0 END
				FROM CTE_Days d
				LEFT JOIN @Tbl_Var_Days dd
					ON dd.RunVariantId = @v_RunVariationId
					AND dd.Day = d.Day
				CROSS JOIN Dbo.RunVariations v
				WHERE dd.RunVariantId IS NULL
					AND V.RunVariationId = @v_RunVariationId
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

		-- aggiunge anche i giorni aggiuntivi relativi a questoa variante
		INSERT INTO @Tbl_Var_Days
				(RunVariantId, Day, WeekDay, OutOfPeriod)
			SELECT @v_RunVariationId, 
					d.Day,
					DATEPART(dw, d.Day) AS WeekDay,
					CASE WHEN d.day > @v_EndDate AND @v_LastOne = 1 AND d.day <= @EndDAte THEN 1 ELSE 0 END
				FROM dbo.RunAdditionalDays d
				LEFT JOIN @Tbl_Var_Days dd
					ON dd.RunVariantId = @v_RunVariationId
					AND dd.Day = d.Day
				WHERE d.RunId = @RunId
					AND d.Day BETWEEN @v_StartDate AND @Limit_EndDate
					AND dd.OutOfPeriod IS NULL;

		FETCH NEXT FROM Var_Curr INTO 
				@v_RunVariationId, @v_CalendarId, 
				@v_StartDate, @v_EndDate,
				@v_LastOne;
	END;

	CLOSE Var_Curr;  
	DEALLOCATE Var_Curr;  

	INSERT INTO @Tbl_Days
			(RunId, Day,
			RunVariationId, WeekDay,
			OutOfPeriod)
		SELECT @RunId,
				vd.Day,
				vd.RunVariantId,
				vd.WeekDay,
				vd.OutOfPeriod
			FROM @Tbl_Var_Days vd;

	-- gestisce i periodi
	WITH CTE_PeriodDay_base AS
	(
		SELECT p.*,
				COALESCE(p.StartDate, @StartDate) AS NewStart,
				COALESCE(p.EndDate, @EndDate) AS NEwEnd
			FROM dbo.RunPeriods p
	) , CTE_PeriodDay AS
	(
		SELECT p.[RunPeriodId],
				p.RunId,
				d.Day,
				ROW_NUMBER() OVER (PARTITION BY p.RunId, d.DAy ORDER BY p.NewStart, p.NewEnd) AS Num
			FROM CTE_PeriodDay_base p
			INNER JOIN @Tbl_Days d
				ON d.RunId = p.RunId
				AND d.Day BETWEEN p.NewStart AND p.NEwEnd
				AND (
					(d.WeekDay = @Monday AND  p.Monday = 1)
					OR (d.WeekDay = @Tuesday AND  p.Tuesday = 1)
					OR (d.WeekDay = @Wednesday AND  p.Wednesday = 1)
					OR (d.WeekDay = @Thursday AND  p.Thursday = 1)
					OR (d.WeekDay = @Friday AND  p.Friday = 1)
					OR (d.WeekDay = @Saturday AND  p.Saturday = 1)
					OR (d.WeekDay = @Sunday AND  p.Sunday = 1)
					)			
	)
	UPDATE @Tbl_Days 
		SET RunPeriodId = pd.[RunPeriodId]
		FROM @Tbl_Days d 
		INNER JOIN CTE_PeriodDay pd
		ON d.RunId = pd.RunId
		AND d.Day = pd.Day
		AND pd.Num = 1;

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
