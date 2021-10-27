USE CtaLineaDb
GO


DECLARE @StartSChoolUYear		AS INT = 20192020;
DECLARE @dt_StartCAlendarDates  AS DATE = '20190901';


BEGIN TRAN;

/* Punti i Raccolta *******************************************************/
-- importa i punti di raccolta
WITH CTE_Data AS (
	SELECT *
		FROM Viaggi_2.dbo.CollectionPoints AS d
		WHERE d.CollectionPointType = 'TT'
			-- OR d.Source = 'TTE'
)
MERGE CtaLineaDb.[dbo].[CollectionPoints] AS t
USING CTE_Data as s
	ON t.CollectionPointId = s.CollectionPointId
WHEN NOT MATCHED  THEN
	INSERT (CollectionPointId, Description,
	Address, City, ZipCode,
	Latitude, Longitude,
	CollectionPointType)
	VALUES (s.CollectionPointId, s.CollectionPointDescription,
		s.Address, s.City, s.ZipCode,
		s.Latitude, s.Longitude,
		s.CollectionPointType)
WHEN MATCHED THEN
	UPDATE SET Description = s.CollectionPointDescription,
		Address = s.Address,
		City = s.City,
		ZipCode = s.ZipCode,
		Latitude = s.Latitude,
		Longitude = s.Longitude,
		CollectionPointType = s.CollectionPointType
WHEN NOT MATCHED  BY SOURCE  THEN
	DELETE;

/* Calendari *******************************************************/
-- import calendari
MERGE CtaLineaDb.dbo.Calendars AS t
USING Viaggi_2.dbo.Calendar AS s
	ON t.CalendarId = s.CalendarID
WHEN NOT MATCHED  THEN
	INSERT (CalendarId, Description, CalendarType)
	VALUES (s.CalendarID, s.Description, s.CalendarType)
WHEN MATCHED THEN
	UPDATE sET Description = s.Description,
			CalendarType = s.CalendarType
WHEN NOT MATCHED  BY SOURCE  THEN
	DELETE;

-- giorni dei calendari
WITH CTE_Data AS
(
	SELECT *
		FROM Viaggi_2.dbo.CalendarHoliday AS h
		WHERE h.HolidayDate >= @dt_StartCAlendarDates
)
MERGE CtaLineaDb.dbo.CalendarDays AS t
USING CTE_Data AS s
	ON t.CalendarId = s.CalendarID
	AND t.Date = s.HolidayDate
WHEN NOT MATCHED  THEN
	INSERT (CalendarId, Date)
	VALUES (s.CalendarID, s.HolidayDate)
WHEN NOT MATCHED  BY SOURCE  THEN
	DELETE;

-- range di validità dei calendari
WITH CTE_Data AS (
	SELECT *
		FROM Viaggi_2.dbo.CalendaSchoolYearRanges d
		WHERE D.SchoolYear >= @StartSChoolUYear
)
MERGE CtaLineaDb.dbo.CalendaSchoolYearRanges AS t
USING CTE_Data AS s
	ON t.CalendarId = s.CalendarID
	AND t.SchoolYear = s.SChoolYear
WHEN NOT MATCHED  THEN
	INSERT (CalendarId, SchoolYear, StartDate, EndDate)
	VALUES (s.CalendarID, s.SchoolYear, s.StartDate, s.EndDate)
WHEN MATCHED THEN
	UPDATE sET StartDate = s.StartDate,
			EndDate = s.EndDate
WHEN NOT MATCHED  BY SOURCE  THEN
	DELETE;


/* Ditte mezzi autisti *******************************************************/
--Ditte
WITH CTE_Data AS (
	SELECT s.*,
			COALESCE (s.eMail, s.OtherEMail, s.CertifiedEMail) AS MailToUse
		FROM Viaggi_2.dbo.Associate AS s
)
MERGE CtaLineaDb.dbo.Associates AS t
USING CTE_Data AS s
	ON t.AssociateId = s.AssociateID
WHEN NOT MATCHED  THEN
	INSERT (AssociateId, Description, BsSupplierCode, BsCustomerCode, Active, Email)
	VALUES (s.AssociateID, s.Description, s.BS_AsociateID, NULL,  s.Active, s.MailToUse)
WHEN MATCHED THEN
	UPDATE sET Description = s.Description,
			BsSupplierCode = s.BS_AsociateID,
			BsCustomerCode = NULL,
			Active = s.Active,
			Email = s.MailToUse
WHEN NOT MATCHED  BY SOURCE  THEN
	DELETE;

-- Mezzi
WITH CTE_Data AS
(
	SELECT d.*,
			(CASE
				WHEN d.Use_TT_Line = 1 OR d.Use_City_line = 1 OR d.Use_Line2 = 1
				THEN 1
				ELSE 0
			END) AS Primary_Car,
			(CASE 
				WHEN d.Use_TT_LineSpare = 1 OR Use_City_lineSoare = 1 OR d.Use_Line2_Spare = 1
				THEN 1
				ELSE 0
			END) AS Spare_Car
		FROM Viaggi_2.dbo.Car AS d
)
MERGE  CtaLineaDb.dbo.Cars AS t
USING CTE_Data AS s
	ON t.CarId = s.CarID
WHEN NOT MATCHED  THEN
	INSERT (CarId, AssociateId,
		Description, NrSittings,RegNumber,
		BsCarId,
		ChassisNumber, FirstRegistration,DiscontinuationDate, 
		PrimaryCar, SpareCar,
		Active)
	VALUES (s.CarID, s.AssociateId, 
		s.Description, s.Sittings, s.RegNumber,
		s.Bs_CarID,
		NULL, NULL, s.DismissionDate,
		s.Primary_Car, s.Spare_Car,
		s.Active)
WHEN MATCHED THEN
	UPDATE sET AssociateId = s.AssociateID,
			Description = s.Description,
			NrSittings = s.Sittings,
			RegNumber = s.RegNumber,
			BsCarId = s.Bs_CarID,
			ChassisNumber = NULL,
			FirstRegistration = NULL,
			DiscontinuationDate = s.DismissionDate,
			PrimaryCar = s.Primary_Car,
			SpareCar = s.Spare_Car,
			Active = s.Active
WHEN NOT MATCHED  BY SOURCE  THEN
	DELETE;

-- Autisti
MERGE CtaLineaDb.dbo.Drivers t
USING Viaggi_2.dbo.Driver AS s
	ON t.DriverId = s.DriverID
WHEN NOT MATCHED  THEN
	INSERT (DriverID, AssociateId,
		LastName, FirstName,
		BsDriverId,
		LicenseNumber, LicenceCategory,
		 DismissionDate,
		Active)
	VALUES (s.DriverID, s.AssociateId, 
		s.LastName, s.FirstName,
		s.BS_DriverID,
		s.LicenseNumber, s.LicenceCategory,
		s.DismissionDate,
		s.Active)
WHEN MATCHED THEN
	UPDATE sET AssociateId = s.AssociateID,
			LastName = s.LastName,
			FirstName = s.FirstName,
			BsDriverId = s.BS_DriverID,
			LicenseNumber = s.LicenseNumber,
			LicenceCategory = s.LicenceCategory,
			DismissionDate = s.DismissionDate,
			Active = s.Active
WHEN NOT MATCHED  BY SOURCE  THEN
	DELETE;

/* Categorie *************************************************************************************/
-- categorie
DECLARE @Tbl_Cat AS TABLE (
	SheetCategoryID		varchar(10) PRIMARY KEY
);
INSERT INTO @Tbl_Cat
	SELECT c.SheetCategoryID
		FROM Viaggi_2.dbo.SheetCategory AS c
		WHERE c.SheetCategoryID <> 'S';

WITH CTE_Data AS (
	SELECT c.*
		FROM Viaggi_2.dbo.SheetCategory AS c
		INNER JOIN @Tbl_Cat AS c1
			ON c.SheetCategoryID = c1.SheetCategoryID
)
MERGE CtaLineaDb.dbo.SheetCategories AS t
USING  CTE_Data AS s
	ON t.CategoryId = s.SheetCategoryID
WHEN NOT MATCHED  THEN
	INSERT (CategoryId, Description)
	VALUES (s.SheetCategoryID, Description)
WHEN MATCHED THEN
	UPDATE SET Description= s.Description
WHEN NOT MATCHED  BY SOURCE  THEN
	DELETE;

-- sottocatgegorie
WITH CTE_Data AS (
	SELECT c.*
		FROM Viaggi_2.dbo.SheetSubCategory AS c
		INNER JOIN @Tbl_Cat AS c1
			ON c.SheetCategoryID = c1.SheetCategoryID
)
MERGE CtaLineaDb.dbo.SheetSubCategories AS t
USING  CTE_Data AS s
	ON t.SubCategoryId = s.SubCategoryID
WHEN NOT MATCHED  THEN
	INSERT (SubCategoryId, CategoryId, Description)
	VALUES (s.SubCategoryID, s.SheetCategoryID, Description)
WHEN MATCHED THEN
	UPDATE SET CategoryId = s.SheetCategoryID,
			Description= s.Description
WHEN NOT MATCHED  BY SOURCE  THEN
	DELETE;



COMMIT;