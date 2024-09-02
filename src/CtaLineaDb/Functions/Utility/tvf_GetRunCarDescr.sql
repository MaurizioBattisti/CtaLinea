/* *********************************************************************************
	Maurizio Battisit
	30/08/2024
	Calcola i mezzi titolai o scorta in un certo periodo
	eventualmetne considera le correnti e le ex 
	se non viene indicata la data di riferimento si considera la data odierna
 ******************************************************************************** */
 CREATE FUNCTION [dbo].[tvf_GetRunCarDescr]
(
	@CarType		Char = 'P', 
	@StartDate		Date = NULL,
	@EndDate		Date = NULL,
	@RefDate		Date = NULL,
	@ShowCurrent	bit = 1,
	@ShowEx			bit = 1
)
RETURNS @Tbl_Result TABLE
(
	RunId			uniqueidentifier PRIMARY KEY,
	CarDescr		Varchar(MAX)
)
AS
BEGIN
	SET @RefDate = COALESCE(@RefDate, GETDATE());
	SET @StartDate = COALESCE(@StartDate, [dbo].[fn_GetStartDate](@RefDate));
	SET @EndDate = COALESCE(@EndDate, [dbo].[fn_GetEndtDate](@RefDate));
	IF @RefDate < @StartDate SET @RefDate = @StartDate;
	IF @RefDate > @EndDate SET @RefDate = @EndDate;


	WITH CTE_Cars AS
	(
		SELECT DISTINCT  rp.RunId,
				rc.CarId,
				CASE WHEN rp.EndDate IS NULL OR rp.EndDate >= @RefDate THEN 1 ELSE 0 END AS CurrentCar
			FROM  dbo.RunCars rc
			INNER JOIN dbo.RunPeriods rp
				ON Rc.RunPEriodId = rp.[RunPeriodId]
			WHERE rc.CarType = @CarType
				AND (rp.StartDate IS NULL
					OR rp.StartDate <= @EndDate
					)
				AND (rp.EndDate IS NULL
					OR rp.EndDate >= @StartDate
					)
	)
	INSERT @Tbl_Result
		SELECT rc.RunId,
				STRING_AGG(c.Description, ', ' ) WITHIN GROUP (ORDER BY c.Description ASC) AS CarsDescr
			FROM CTE_Cars rc
			INNER JOIN dbo.Cars c
				ON rc.CarId = c.CarId
			WHERE rc.CurrentCar = @ShowCurrent
				OR rc.CurrentCar <> @ShowEx
			GROUP BY rc.RunId
			;

	RETURN
END