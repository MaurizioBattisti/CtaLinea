/* *******************************************************
	Maurizio Battisti
	04/05/2023
	Restituisce una lista di attività  in delta tra due settimane
	la corrente e quella successiva per tutti i consorziati o per quello indciato
	nella mail restituisce la mail del consorziato oppure una  di test indicata
*******************************************************  */
CREATE PROCEDURE [dbo].[up_AssociateActivityWeeksForMail]
	@RefDate			Date = NULL,
	@AssociateId		uniqueidentifier = NULL,
	@ForceDsetination	Varchar(MAX) = NULL
AS
bEGIN
	DECLARE @Monday int = DATEPART(dw, '20221226');
	DECLARE @Tuesday int = DATEPART(dw, '20221227');
	DECLARE @Wednesday int = DATEPART(dw, '20221228');
	DECLARE @Thursday  int = DATEPART(dw, '20221229');
	DECLARE @Friday int = DATEPART(dw, '20221230');
	DECLARE @Saturday int = DATEPART(dw, '20221224');
	DECLARE @Sunday int= DATEPART(dw, '20221225');
	
	DECLARE @Curr_Start	Date;
	DECLARE @Curr_End	Date;
	DECLARE @Next_Start	Date;
	DECLARE @NExt_End	Date;

	IF @RefDate IS NULL SET @RefDate = GETDATE();
	
	DECLARE @Delta int =
		(CASE DATEPART(dw, @RefDate)
			WHEN @Monday THEN 0
			WHEN @Tuesday THEN -1
			WHEN @Wednesday THEN -2
			WHEN @Thursday THEN -3
			WHEN @Friday THEN -4
			WHEN @Saturday THEN -5
			WHEN @Sunday THEN -6
			ELSE 0
		END)

	sET @Curr_Start =DATEADD(d, @Delta, @RefDate);
	SET @Curr_End = DATEADD(d, 6, @Curr_Start);
	SET @Next_Start =  DATEADD (d, 7, @Curr_Start);
	SET @NExt_End =  DATEADD (d, 7, @Curr_End);

	WITH  CTE_DaysBase aS
	(
		SELECT d.RunId, d.RunVariationId, c.AssociateId, c.CarId, d.WeekDay, d.Day
			FROM dbo.RunDays d
			INNER JOIN dbo.RunCars rc
				ON D.RunCarId = rc.RunCarId
			INNER JOIN dbo.Cars c
				ON rc.CarId = c.CarId
			WHERE d.Suspended = 0
				AND d.OutOfPeriod =0
				AND (@AssociateId IS NULL
					OR c.AssociateId = @AssociateId)
	), CTE_Curr AS
	(
		SELECT d.RunId, d.RunVariationId, d.ASsociateID, d.CArId , d.WeekDay, d.DAy
			FROM CTE_DaysBase d
			WHERE d.Day BETWEEN @Curr_Start AND @Curr_End
	), CTE_Next AS
	(
		SELECT d.RunId, d.RunVariationId, d.ASsociateID, d.CArId , d.WeekDay, d.Day
			FROM CTE_DaysBase d
			WHERE d.Day BETWEEN @Next_Start AND @NExt_End
	) , CTGE_All AS
	(
		SELECT c.*, 'OLD' AS Status
			FROM CTE_Curr c
			LEFT JOIN CTE_Next n
			ON  c.RunId = n.RunId
				AND c.RunVariationId = n.RunVariationId
				AND DATEADD(d, 7, c.Day) = n.Day
				AND c.AssociateId = n.AssociateId
				AND c.CarId = n.CarId
				AND c.WeekDay = n.WeekDay
			WHERE n.RunId IS NULL
		UNION ALL
		SELECT n.*, 'NEW' AS Status
			FROM CTE_Next n
			LEFT JOIN CTE_Curr c
			ON  c.RunId = n.RunId
				AND c.RunVariationId = n.RunVariationId
				AND DATEADD(d, 7, c.Day) = n.Day
				AND c.AssociateId = n.AssociateId
				AND c.CarId = n.CarId
				AND c.WeekDay = n.WeekDay
			WHERE c.RunId IS NULL

	)
	SELECT a.AssociateId,
			a.Description AS AssociateDescr,
			COALESCE(@ForceDsetination, a.Email) AS Email,
			c.Description AS CarDescr,
			r.RunName,
			ct.ContractName,
			v.LineNumber,
			v.RunNumber,
			v.StartTime,
			v.Path,
			'' AS RunDataDescription,

			d.RunId,
			d.RunVariationId,
			
			d.Day,
			d.Status
		FROM CTGE_All d
		INNER JOIN dbo.runs r
			ON d.RunId = r.RunId
		INNER JOIN dbo.Contracts ct
			ON R.ContractId = ct.ContractId
		INNER JOIN dbo.RunVariations v
			ON d.RunVariationId = v.RunVariationId
		INNER JOIN dbo.Associates a
			ON d.AssociateId = a.AssociateId
		INNER JOIN dbo.Cars c
			ON d.CarId = c.CarId
		ORDER BY a.Description, a.AssociateId, d.Status DESC, d.Day;

	RETURN 0;
END
