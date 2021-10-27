/* ******************************************************************************
*	Authro			Maurizio Battisti
*	Date			21/05/2021
*	Description		vista con tutti i dati dei mezzi di un  servizio
****************************************************************************** */
CREATE VIEW [dbo].[vw_CtaServiceCars]
AS
SELECT sc.*,
		c.RegNumber,
		c.Description AS CarDescription,
		c.NrSittings,
		c.ChassisNumber,
		c.FirstRegistration,
		c.BsCarId,
		c.PrimaryCar,
		c.SpareCar,
		c.Active AS CarActive,
		a.Description AS AssociateDescription,
		a.BsCustomerCode,
		a.BsSupplierCode,
		a.Email,
		a.Active AS AssociateActive
	FROM dbo.CtaServiceCars sc
	INNER JOIN dbo.Cars c
		ON sc.CarId = c.CarId
	INNER JOIN Dbo.Associates a
		ON c.AssociateId = a.AssociateId
		;
