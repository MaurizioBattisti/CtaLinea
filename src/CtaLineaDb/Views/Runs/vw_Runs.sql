/* *************************************************
*	Maurizio Battisti
*	19/12/2022
*	Lista delle corse con tutti i dati 
 ************************************************* */
CREATE VIEW [dbo].[vw_Runs]
AS 
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
), CTE_VarCount AS
(
	SELECT v.RunId,
			COUNT(*) As VariationCount
	FROM dbo.RunVariations v
	GROUP BY v.RunId
)
SELECT  r.RunId,
		r.CtaRunId,
		r.RunName,
		r.ContractId, r.Extra,
		r.ContractRowNumber,
		r.StartDate, r.EndDate,
		r.RequestedDays,
		r.Note AS RunNote,

		-- descrizioni
		rAss.AssociatesDescr,
		rPrimCar.PrimaryCarsDescr,
		rSpareAss.SpareAssociatesDescr,
		rSpareCar.SpareCarsDescr,
		rCal.CalendarsDescr,
		rPath.PathsDescr,
		
		v.RunVariationId,
		v.LineNumber, v.RunNumber,
		v.StartDate AS VariationStartDate,
		v.StartTime, v.EndTime,
		v.Monday, v.Tuesday, v.Wednesday, v.Thursday, v.Friday, v.Saturday, v.Sunday,
		v.Path, v.RequestedFrequency,
		v.Km, v.RequestedCapacity,
		v.Note AS VariationNote,

		ctr.ContractDescription,
		ctr.StartDate AS ContractStart,
		ctr.EndDate AS ctrEndDAte,
		COALESCE(vc.VariationCount, 0) as VariationCount,
		
		-- tag con priorità più alta
		tag.TagName,
		tag.BgColor,
		tag.Color,
		(CASE
			WHEN r.Extra = 0 AND r.EndDate IS NOT  NULL AND r.EndDate < GETDATE() THEN 1		-- soppressa
			WHEN r.Extra = 1 AND r.EndDate IS NOT  NULL AND r.EndDate < GETDATE() THEN 2		-- Terminata
			WHEN r.StartDate IS NOT NULL AND r.StartDate > GETDATE()  THEN 100					-- non ancora attivata
			ELSE 0
		END) AS RunStatus,
		(CASE
			WHEN r.Extra = 0 AND r.EndDate IS NOT  NULL AND r.EndDate < GETDATE() THEN 'Soppressa'
			WHEN r.Extra = 1 AND r.EndDate IS NOT  NULL AND r.EndDate < GETDATE() THEN 'Terminata'
			WHEN r.StartDate IS NOT NULL AND r.StartDate > GETDATE()  THEN 'Non ancora attivata'
			ELSE NULL
		END) AS RunStatusDesvr,
		mrf.ForfaitId,
		mrf.ForfaitName,
		mrf.[ForfaitType],
		CASE WHEN note.RunId IS NOT NULL THEN 1 ELSE 0 END AS HasNote
	FROM dbo.Runs r
	INNER JOIN CTE_Variants v
		ON R.RunId = v.RunId
	INNER JOIN dbo.Contracts ctr
		ON R.ContractId = ctr.ContractId
	LEFT JOIN CTE_VarCount vc
		ON r.RunId = vc.RunId
	LEFT JOIN dbo.vw_RunAssociates rAss
		ON r.RunId = rAss.RunId
	LEFT JOIN [dbo].[vw_RunSpareAssociates] rSpareAss
		ON r.RunId = rSpareAss.RunId
	LEFT JOIN dbo.vw_RunPrimaryCars rPrimCar
		ON r.RunId = rPrimCar.RunId
	LEFT JOIN dbo.vw_RunSpareCars rSpareCar
		ON r.RunId = rSpareCar.RunId
	LEFT JOIN dbo.vw_RunCalendars rCal
		ON r.RunId = rCal.RunId
	LEFT JOIN dbo.vw_RunPaths rPath
		ON r.RunId = rPath.RunId
	LEFT JOIN dbo.vw_RunFirstTags tag
		ON r.RunId = tag.RunId
	LEFT JOIN dbo.MultiRunForfaitDetails mrfd
		ON mrfd.RunId = r.RunId
	LEFT JOIN dbo.MultiRunForfait mrf
		ON mrf.ForfaitId = mrfd.ForfaitId
	LEFT JOIN [dbo].[RunInternalNotes] note
		ON R.RunId = note.RunId
	;
