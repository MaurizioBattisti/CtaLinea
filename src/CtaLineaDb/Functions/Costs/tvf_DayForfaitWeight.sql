/* **********************************************************************
	Maurizio Battisit
	03/06/2023
	somma dei pesi per giorno dei forfait giornalieri
********************************************************************** */
CREATE FUNCTION [dbo].[tvf_DayForfaitWeight]
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
	Day				Date,
	TotalWeight		real,

	PRIMARY KEY (ForfaitId, Day)
)
AS
BEGIN
	WITH CTE_Runs AS
	(
		SELECT f.ForfaitId, fd.RunId
			FROM dbo.MultiRunForfait f
			INNER JOIN dbo.MultiRunForfaitDetails fd
				ON f.ForfaitId = fd.ForfaitId
			WHERE f.ForfaitType = 'D'
	), CTE_RunDays AS 
	(
		SELECT DISTINCT d.*, 
				/*
				CASE @RplacedCars 
					WHEN 1 THEN rc.AssociateId
					WHEN 0 THEN o_rc.AssociateId
				END AS AssociateId,
				CASE @RplacedCars 
					WHEN 1 THEN rc.CarId
					WHEN 0 THEN o_rc.CarId
				END AS CarId, 
				*/
				r.ForfaitId
			FROM Dbo.RunDays d
			INNER JOIN CTE_Runs r
				ON d.RunId = r.RunId
			/*
			LEFT JOIN dbo.RunCars rc
				ON d.RunCarId = rc.RunCarId
			LEFT JOIN dbo.RunCars o_rc
				ON d.OriginalRunCarId = rc.RunCarId
			*/
			WHERE d.CarNum = 1
				AND (@StartDate IS NULL
					OR d.Day >= @StartDate)
				AND (@EndDate IS NULL
					OR d.Day <= @EndDate)
			/*
				AND (d.Suspended = 0
					OR d.Suspended = @Suspended)
				AND (d.OutOfPeriod = 0
					OR d.OutOfPeriod =OutOfPEriod
			*/
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
		(ForfaitId, Day, TotalWeight)
		SELECT d.ForfaitId,
				d.Day,
				SUM(d.Weight) AS TotalWeight
			FROM CTE_Data d
			GROUP BY d.ForfaitId,
				d.Day;

	RETURN
END
