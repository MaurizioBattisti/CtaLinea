USE CtaLineaDb
GO

-- DECLARE @StartSChoolUYear		AS INT = 20192020;


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
COMMIT;