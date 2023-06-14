/* **********************************************************************
	Maurizio Battisit
	04/06/2023
	somma dei pesi per mnese dei forfait Annulai
********************************************************************** */
CREATE FUNCTION [dbo].[tvf_YearForfaitWeight]
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
	YearId			int,
	TotalWeight		real,

	PRIMARY KEY (ForfaitId, YearId)
)
AS
BEGIN
	DECLARE @StartYear int;
	DECLARE @EndYear int;

	SET @StartDate = COALESCE(@StartDate, '20000901');
	SET @EndDate = COALESCE(@EndDate, '99970831');
	
	SET @StartYear = YEAR(@StartDate);
	IF MONTH(@StartDate) < 9 SET @StartYear = @StartYear -1;
	SET @StartDate = CAST(CAST(@StartYear AS Varchar(4)) + '0901' AS date);

	SET @EndYear = YEAR(@EndDate);
	IF MONTH(@EndDate) >= 9 SET @EndYear = @EndYear + 1;
	SET @EndDate = CAST(CAST(@EndYear AS Varchar(4)) + '0901' AS date);
	SET @EndDate = DATEADD (D, -1, @EndDate);

	WITH CTE_Runs AS
	(
		SELECT f.ForfaitId, fd.RunId
			FROM dbo.MultiRunForfait f
			INNER JOIN dbo.MultiRunForfaitDetails fd
				ON f.ForfaitId = fd.ForfaitId
			WHERE f.ForfaitType = 'Y'
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
		(ForfaitId, YearId, TotalWeight)
		SELECT d.ForfaitId,
				dbo.fn_GetYearId(d.Day),
				SUM(d.Weight) AS TotalWeight
			FROM CTE_Data d
			GROUP BY d.ForfaitId,
				dbo.fn_GetYearId(d.Day);

	RETURN
END
