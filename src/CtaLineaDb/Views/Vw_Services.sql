/* ******************************************************************************
*	Authro			Maurizio Battisti
*	Date			21/05/2021
*	Description		Vista con tutti i dati completi per Servizi CTA
****************************************************************************** */
CREATE VIEW [dbo].[Vw_Services]
AS
WITH CTE_ServiceCars AS
(
	SELECT sc.*,
			ROW_NUMBER() OVER (PARTITION BY sc.ServiceId, sc.CarType  ORDER BY sc.StartDate DESC) AS ORdinal,
			c.Description AS CarDescription,
			a.Description AS AssociateDescription
		FROM dbo.CtaServiceCars sc
		INNER JOIN dbo.Cars c
			ON sc.CarId = c.CarId
		INNER JOIN Dbo.Associates a
			ON c.AssociateId = a.AssociateId
		WHERE sc.StartDate <= SYSDATETIME()

),  CTE_ServicePeriods AS
(
	SELECT p.*,
			ROW_NUMBER() OVER (PARTITION BY p.ServiceId ORDER BY p.StartDate DESC) AS Ordinal,
			c.Description AS CalendarDescription
		FROM dbo.CtaServicePEriods p
		LEFT JOIN dbo.Calendars c
			ON p.CalendarId = c.CalendarId
		WHERE p.StartDate <= SYSDATETIME()
)
SELECT tt.*,
		s.Note AS  CtaNote,
		rt.Description AS RunTypeDescription,
		cat.Description AS Category,
		sub.Description AS SubCategory,
		p.EndDate AS Cta_Start,
		p.EndDate AS Cta_End,
		p.Monday,
		p.Tuesday,
		p.Wednesday,
		p.Thursday,
		p.Friday,
		p.Saturday,
		p.Sunday,
		p.CalendarId,
		p.CalendarDescription,
		-- mezzo 1 primario
		car_1.StartDate AS Car1_StartDate,
		car_1.AssociateDescription AS Car1_Associate,
		car_1.CarDescription AS Car1_CarDescription,
		-- Mezzo 2 (Prima scorta)
		car_2.StartDate AS Car2_StartDate,
		car_2.AssociateDescription AS Car2_Associate,
		car_2.CarDescription AS Car2_CarDescription,
		-- mezzo 3 (seconda scorta)
		car_3.StartDate AS Car3_StartDate,
		car_3.AssociateDescription AS Car3_Associate,
		car_3.CarDescription AS Car3_CarDescription

	FROM dbo.TtServices tt
	INNER JOIN  dbo.CtaServices s
		ON tt.ServiceId = s.ServiceId
	LEFT JOIN dbo.RunTypes rt
		ON s.RunTypeId = rt.RunTypeId
	LEFT JOIN dbo.SheetCategories cat
		ON s.CategoryId = cat.CategoryId
	LEFT JOIN dbo.SheetSubCategories sub
		ON s.SubCategoryId = sub.SubCategoryId
	LEFT JOIN CTE_ServicePeriods p
		ON s.ServiceId = p.ServiceId
		AND p.Ordinal = 1
	LEFT JOIN CTE_ServiceCars car_1
		ON s.ServiceId = car_1.ServiceId
		AND car_1.ORdinal = 1
		AND car_1.CarType = 1
	LEFT JOIN CTE_ServiceCars car_2
		ON s.ServiceId = car_2.ServiceId
		AND car_2.ORdinal = 1
		AND car_2.CarType = 2
	LEFT JOIN CTE_ServiceCars car_3
		ON s.ServiceId = car_3.ServiceId
		AND car_3.ORdinal = 1
		AND car_3.CarType = 3
		;
