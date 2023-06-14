/* **********************************************************************
	Maurizio Battisit
	04/06/2023
	somma dei pesi per mnese dei forfait mensili
********************************************************************** */
CREATE FUNCTION [dbo].[tvf_MonthForfaitWeight]
(
	@StartDate		date = NULL,
	@EndDate		date = NULL,
	@OutOfPEriod	BIT = 0,
	@Suspended		BIT = 0,
	@RplacedCars	BIT = 1,
	@WeightCapacity	bit = 1
)
RETURNS @Tbl_Result TABLE
(
	ForfaitId		int,
	MonthId			int,
	TotalWeight		real,

	PRIMARY KEY (ForfaitId, MonthId)
)
AS
BEGIN
	SET @StartDate = COALESCE(@StartDate, '20000901');
	SET @EndDate = COALESCE(@EndDate, '99990831');

	SET @EndDate = CAST(FORMAT(@EndDate, 'yyyyMM')+ '01' AS date);
	SET @EndDate = CAST(FORMAT(@EndDate, 'yyyyMM')+ '01' AS date);
	SET @EndDate = DATEADD(M, 1, @EndDate);
	SET @EndDate = DATEADD (D, -1, @EndDate);

	WITH CTE_Runs AS
	(
		SELECT f.ForfaitId, fd.RunId
			FROM dbo.MultiRunForfait f
			INNER JOIN dbo.MultiRunForfaitDetails fd
				ON f.ForfaitId = fd.ForfaitId
			WHERE f.ForfaitType = 'M'
	), CTE_RunDays AS 
	(
		SELECT DISTINCT d.*, 
				r.ForfaitId
			FROM Dbo.RunDays d
			INNER JOIN CTE_Runs r
				ON d.RunId = r.RunId
			WHERE d.CarNum = 1
				AND d.Day BETWEEN @StartDate AND @EndDate
	), CTE_Data AS
	(
		SELECT d.ForfaitId,
				d.RunId,
				d.Day,
				rv.Km * 
				CASE WHEN @WeightCapacity = 0 THEN 1
					WHEN rv.RequestedCapacity <= 0 THEN 1
					ELSE rv.RequestedCapacity
				END Weight
			FROM CTE_RunDays d
			INNER JOIN Dbo.RunVariations rv
				ON d.RunId = rv.RunId
				AND rv.StartDate IS NULL
	)
	INSERT INTO @Tbl_Result
		(ForfaitId, MonthId, TotalWeight)
		SELECT d.ForfaitId,
				dbo.fn_GetMonthId(d.Day),
				SUM(d.Weight) AS TotalWeight
			FROM CTE_Data d
			GROUP BY d.ForfaitId,
				dbo.fn_GetMonthId(d.Day);

	RETURN
END
