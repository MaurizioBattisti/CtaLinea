/* *********************************************************************
	Maurizio Battisti
	18/02/2023
	Filtri avanzati per le corse
********************************************************************* */
CREATE FUNCTION [dbo].[tvf_Runs_AdvancedFilter]
(
	@AssociateId		uniqueidentifier = NULL,
	@CarId				uniqueidentifier = NULL,
	@MinSittings		int = NULL,
	@MaxSittings		int = NULL,
	@LineNumber			int = NULL,
	@RunNumber			VARCHAR(MAX) = NULL,
	@Node				VARCHAR(MAX) = NULL,
	@StartDate			Date = NULL,
	@EndDate			Date = NULL,
	@StartTime			Time = NULL,
	@EndTime			time = NULL,
	@Frequency			VARCHAR(MAX) = NULL,
	@CalendarIds		VARCHAR(MAX) = NULL,
	@WeekDays			VARCHAR(MAX) = NULL,

	@InContract			INT = NULL,
							-- 1	Capitolato
							-- 2	Extra capitolato
							-- 3	Paraialmente extracapitolato (non implementato)
	@ActiveRun			INT = NULL,
							-- 1	Attiva
							-- 2	terminata
							-- 3	non ancora avviata
	@DateRef			Date = NULL,
	@TabIds				VARCHAR(MAX) = NULL,
	@ForfaitId			int NULL,
	@CollectionPointId	varchar(20) NULL
)
RETURNS @Tbl TABLE
(
	RunId		uniqueidentifier NOT NULL PRIMARY KEY
)
AS
BEGIN
	-- Preprazioen dei parametri
	DECLARE @Tbl_CAlendars AS TABLE (
		CalendarId		int PRIMARY KEY
	);
	IF NOT @CalendarIds IS NULL
	BEGIN
		INSERT INTO @Tbl_CAlendars (CalendarId)
		SELECT DISTINCT value FROM STRING_SPLIT(@CalendarIds, ',');
	END
	DECLARE @Tbl_WeekDays AS TABLE (
		WeekDay		int PRIMARY KEY
	);
	IF NOT @WeekDays IS NULL
	BEGIN
		INSERT INTO @Tbl_WeekDays (WeekDay)
		SELECT DISTINCT 
				CASE value 
					WHEN 1 THEN DATEPART(dw, '20221226')
					WHEN 2 THEN DATEPART(dw, '20221227')
					WHEN 3 THEN DATEPART(dw, '20221228')
					WHEN 4 THEN DATEPART(dw, '20221229')
					WHEN 5 THEN DATEPART(dw, '20221230')
					WHEN 6 THEN DATEPART(dw, '20221224')
					WHEN 7 THEN DATEPART(dw, '20221225')
					ELSE -1
				END AS WeekDay
			FROM STRING_SPLIT(@WeekDays, ',');
	END
	IF @DateRef IS NULL SET @DateRef = GETDATE();
	DECLARE @Tbl_TagIds AS TABLE (
		TagId	INT	PRIMARY KEY
	);
	IF NOT @TabIds IS NULL
	BEGIN
		INSERT INTO @Tbl_TagIds (TagId)
		SELECT DISTINCT value FROM STRING_SPLIT(@TabIds, ',');
	END;

	-- esecuzione vera e propria
	INSERT INTO @Tbl (RunId)
	SELECT DISTINCT r.RunId
		FROM dbo.Runs r
		LEFT JOIN dbo.RunTags t
			ON r.RunId = t.RunId
		LEFT JOIN @Tbl_TagIds tag
			ON T.TagId = tag.TagId
		INNER JOIN dbo.Contracts contr
			ON r.ContractId = contr.ContractId
		LEFT JOIN dbo.RunVariations v
			ON v.RunId = r.RunId
		LEFT JOIN dbo.RunVariationCalendars cal
			ON V.RunVariationId = cal.RunVariationId
		LEFT JOIN @Tbl_CAlendars myCal
			ON cal.CalendarId = myCal.CalendarId
		LEFT JOIN dbo.RunNodes n
			ON v.RunVariationId = n.RunVariationId
		LEFT JOIN dbo.CollectionPoints cp
			ON cp.CollectionPointId = n.CollectionPointId
		LEFT JOIN dbo.RunPeriods p
			ON r.RunId = p.RunId
		LEFT JOIN dbo.RunCars rc
			ON rc.RunPeriodId = p.RunPEriodId
		LEFT JOIN dbo.Cars c
			ON Rc.CarId = c.CarId
		LEFT JOIN dbo.RunDays d
			ON r.RunId = d.RunId
		LEFT JOIN @Tbl_WeekDays wd
			ON d.WeekDay = wd.WeekDay
		LEFT JOIN dbo.MultiRunForfaitDetails mrfd
			ON r.RunId = mrfd.RunId

		WHERE (@AssociateId IS NULL
				OR rc.AssociateId = @AssociateId)
			AND (@CarId IS NULL
				OR rc.CarId = @CarId)
			AND (@MinSittings IS NULL
				OR c.NrSittings >= @MinSittings)
			AND (@MaxSittings IS NULL
				OR c.NrSittings <= @MaxSittings)
			AND (@LineNumber IS NULL
				OR v.LineNumber = @LineNumber)
			AND (@RunNumber IS NULL
				OR v.RunNumber = @RunNumber)
			AND (@Node IS NULL	
				OR n.RunNodeId LIKE '%' + @Node +  '%' 
				OR cp.Description LIKE '%' + @Node +  '%'
				OR cp.Address LIKE '%' + @Node +  '%'
				)
			AND (@CollectionPointId IS NULL
				OR n.CollectionPointId = @CollectionPointId)
			AND (@StartDate IS NULL
				OR d.Day>= @StartDate)
			AND (@EndDate IS NULL
				OR d.Day <= @EndDate)
			AND (@StartTime IS NULL
				OR n.Hour >= @StartTime)
			AND (@EndTime IS NULL
				OR n.Hour <= @EndTime)
		
			AND (@InContract IS NULL
				OR (@InContract = 1 AND r.Extra = 0)
				OR (@InContract = 2 AND r.Extra = 1)
				-- implementare meglio il filtor sulle corse extra  captiolato parziaale
				--OR (@InContract = 3 AND r.Extra = 0 AND cost.ExtraType = 2)
				)
			AND (@CalendarIds IS NULL
				OR myCal.CalendarId IS NOT NULL)
			AND (@WeekDays IS NULL
				OR wd.WeekDay IS NOT NULL)
			AND (@Frequency IS NULL
				OR v.RequestedFrequency LIKE '%' + @Frequency + '%')
			AND (@ActiveRun IS NULL
				OR( @ActiveRun = 1 AND @DateRef BETWEEN COALESCE (r.StartDate, contr.StartDAte) AND COALESCE (r.EndDate, contr.EndDate) )
				OR( @ActiveRun = 2 AND @DateRef > COALESCE (r.EndDate, contr.EndDate) )
				OR( @ActiveRun = 3 AND @DateRef < COALESCE (r.StartDate, contr.StartDate) )
				)
			AND (@TabIds IS NULL
				OR tag.TagId IS NOT NULL)
			AND (@ForfaitId IS NULL
				OR mrfd.ForfaitId = @ForfaitId)
			;

	RETURN
END
