USE CtaLineaDb
GO

WITH CTE_Variants AS
(
	SELECT *
		FROM dbo.RunVariations v
		WHERE V.StartDate  IS NULL
)
SELECT  r.RunId,
		r.ContractId, r.Extra,
		r.ContractRowNumber,
		r.StartDate, r.EndDate,
		r.RequestedDays,
		r.Note AS RunNote,
		
		v.RunVariationId,
		v.CalendarId,
		c.CalendarName,
		v.LineNumber, v.RunNumber,
		v.StartTime, v.EndTime,
		v.Monday, v.Tuesday, v.Wednesday, v.Thursday, v.Friday, v.Saturday, v.Sunday,
		v.Path, v.RequestedFrequency,
		v.Km, v.RequestedCapacity,
		v.Note AS VariationNote,

		ctr.ContractDescription,
		ctr.StartDate AS ContractStart,
		ctr.EndDate AS ctrEndDAte
	FROM dbo.Runs r
	INNER JOIN CTE_Variants v
		ON R.RunId = v.RunId
	INNER JOIN dbo.Calendars c
		ON V.CalendarId =  c.CalendarId
	INNER JOIN dbo.Contracts ctr
		ON R.ContractId = ctr.ContractId