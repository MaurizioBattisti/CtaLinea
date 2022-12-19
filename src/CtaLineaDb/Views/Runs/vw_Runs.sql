/* *************************************************
*	Maurizio Battisti
*	19/12/2022
*	Lista delle corse con tutti i dati 
 ************************************************* */
CREATE VIEW [dbo].[vw_Runs]
AS 
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

		-- descrizioni
		rAss.AssociatesDescr,
		rPrimCar.PrimaryCarsDescr,
		rSpareCar.SpareCarsDescr,
		rCal.CalendarsDescr,
		rPath.PathsDescr,
		
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
	LEFT JOIN dbo.vw_RunAssociates rAss
		ON r.RunId = rAss.RunId
	LEFT JOIN dbo.vw_RunPrimaryCars rPrimCar
		ON r.RunId = rPrimCar.RunId
	LEFT JOIN dbo.vw_RunSpareCars rSpareCar
		ON r.RunId = rSpareCar.RunId
	LEFT JOIN dbo.vw_RunCalendars rCal
		ON r.RunId = rCal.RunId
	LEFT JOIN dbo.vw_RunPaths rPath
		ON r.RunId = rPath.RunId
	;
