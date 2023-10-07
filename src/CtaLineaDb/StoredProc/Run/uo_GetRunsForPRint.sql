/* *****************************************************
	Maurizio Battisti
	05/10/2023
	Recupera i dati delle corse per la stampa della scheda di linea
***************************************************** */
CREATE PROCEDURE [dbo].[uo_GetRunsForPRint]
	@ContractId		int = NULL,
	@StartDate		date = NULL,
	@EndDate		date = NULL,
	@CtaIds			varchar(MAX) = NULL
AS
BEGIN
	DECLARE @Tbl_CtaIds TABLE 
	(
		CtaId	int NOT NULL PRIMARY KEY
	);
	INSERT INTO @Tbl_CtaIds (CtaId)
		SELECT DISTINCT value FROM STRING_SPLIT(@CtaIds, ',');

	-- sistema le date se sono nulle
	DECLARE @RefDate	date = COALESCE(@StartDate, @EndDate, GetDATE());
	DECLARE @UserName	varchar(128);
	SET @UserName = dbo.fn_Session_GetUserName();

	IF @StartDate IS NULL SET @StartDate = dbo.fn_GetEndtDate(@RefDate);
	IF @EndDate IS NULL SET @EndDate = dbo.fn_GetEndtDate(@RefDate);

	-- imposta i valori di sessione
	EXEC [dbo].[up_Session_SetInfo] @StartDate, @EndDate, @ContractId, @UserName;

	WITH CTE_Variants_Base AS
	(
		SELECT v.*,
				ROW_NUMBER() OVER (PARTITION BY v.RunId ORDER BY COALESCE (v.StartDate, '1900-01-01') DESC) AS num
			FROM dbo.RunVariations v
			WHERE v.StartDate IS NULL
				OR v.StartDate < dbo.fn_Session_GetCurrentDate()
	), CTE_Variants AS
	(
		SELECT v.*
			FROM CTE_Variants_Base v
			WHERE v.num = 1	
	), CTE_Runs AS
	(
		SELECT r.RunId,
			v.RunVariationId
		FROM dbo.Runs r
		INNER JOIN CTE_Variants v
			ON R.RunId = v.RunId
		WHERE (@ContractId IS NULL 
				OR r.ContractId = @ContractId)
			AND (r.StartDate IS NULL 
				OR r.StartDate <= @EndDate)
			AND (r.EndDate IS NULL
				OR r.EndDate >= @StartDate)
	), CTE_Nodes AS
	(
		SELECT n.RunVariationId,				
				ROW_NUMBER() OVER (PARTITION BY n.RunVariationId ORDER BY n.Hour, n.ProgrNumber) AS Asc_Num,
				ROW_NUMBER() OVER (PARTITION BY n.RunVariationId ORDER BY n.Hour DESC, n.ProgrNumber DESC) AS Desc_Num,
				n.Hour,
				n.CollectionPointId,
				cp.Description,
				n.CoincidenceDescr
			FROM dbo.RunNodes n
			INNER JOIN CTE_Runs r
				ON n.RunVariationId = r.RunVariationId
			INNER JOIN dbo.CollectionPoints cp
				ON n.CollectionPointId = cp.CollectionPointId
	), CTE_Calendars AS
	(
		SELECT r.RunId,
				r.RunVariationId,
				rc.Exclusion,
				STRING_AGG(c.CalendarName, ', ') AS CalendarsDescr
			FROM CTE_Runs r
			INNER JOIN dbo.RunVariationCalendars rc
				ON r.RunVariationId = rc.RunVariationId
			INNER JOIN dbo.Calendars c
				ON rc.CalendarId = c.CalendarId
			GROUP BY r.RunId,
				r.RunVariationId,
				rc.Exclusion
	)
	SELECT r.RunId,
			r.ContractId,
			c.ContractName,
			r.StartDate, 
			r.EndDate,
			r.CtaRunId,
			r.Extra,
			r.ContractRowNumber,
			r.Elastibus,
			pa.AssociatesDescr AS PrimaryAssociateDescr,
			pc.PrimaryCarsDescr AS PrimaryCarDescr,
			sa.SpareAssociatesDescr AS SpareAssociateDescr,
			sc.SpareCarsDescr AS SpareCarDescr,

			v.LineNumber,
			v.RunNumber,
			r.RunName,
			v.RequestedFrequency,
			v.Km,
			v.RequestedCapacity,

			cal_i.CalendarsDescr AS Inc_CalendarDescr,
			cal_e.CalendarsDescr AS Exc_CalendarDescr,

			-- primo nodo
			fn.Hour AS Fn_Hour,
			fn.CollectionPointId AS Fn_CollectionPointId,
			fn.Description AS Fn_cpDescr,
			fn.CoincidenceDescr AS Fn_CoincidenceDescr,

			-- ultimo nodo
			ln.Hour AS Ln_Hour,
			ln.CollectionPointId AS Ln_CollectionPointId,
			ln.Description AS Ln_cpDescr,
			ln.CoincidenceDescr AS Ln_CoincidenceDescr

		FROM CTE_Runs rr
		INNER JOIN dbo.Runs r
			ON r.RunId = rr.RunId
		INNER JOIN @Tbl_CtaIds ids
			ON r.CtaRunId = ids.CtaId
		INNER JOIN dbo.Contracts c
			ON R.ContractId = c.ContractId
		INNER JOIN dbo.RunVariations v
			ON rr.RunVariationId = v.RunVariationId 
		LEFT JOIN [dbo].[vw_RunAssociates] pa
			ON pa.RunId = r.RunId
		LEFT JOIN [dbo].[vw_RunPrimaryCars] pc
			ON pc.RunId = r.RunId
		LEFT JOIN [dbo].[vw_RunSpareAssociates] AS sa
			ON sa.RunId = r.RunId
		LEFT JOIN [dbo].[vw_RunSpareCars] sc
			ON sc.RunId = r.RunId
		LEFT JOIN CTE_Calendars cal_i
			ON cal_i.RunId = rr.RunId
			AND cal_i.RunVariationId = rr.RunVariationId
			AND cal_i.Exclusion = 0
		LEFT JOIN CTE_Calendars cal_e
			ON cal_e.RunId = rr.RunId
			AND cal_e.RunVariationId = rr.RunVariationId
			AND cal_e.Exclusion = 1
		LEFT JOIN CTE_Nodes fn
			ON rr.RunVariationId = fn.RunVariationId
			AND fn.Asc_Num = 1
		LEFT JOIN CTE_Nodes ln
			ON rr.RunVariationId = ln.RunVariationId
			AND ln.Desc_Num = 1
		ORDER BY r.CtaRunId ASC
		;

	RETURN 0;
END
