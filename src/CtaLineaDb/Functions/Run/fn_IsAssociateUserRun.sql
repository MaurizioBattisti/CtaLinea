/* *******************************************************************
	Maurizio Battisti
	14/06/2023
	Controlla se una corsa può essere lavorata da un utente consorziato
******************************************************************* */
CREATE FUNCTION [dbo].[fn_IsAssociateUserRun]
(
	@UserName	varchar(128),
	@RunId		uniqueidentifier
)
RETURNS BIT
AS
BEGIN
	DECLARE @Result		bit = 0;
	DECLARE @AssId		uniqueidentifier;
	SELECT @AssId = u.AssociateId
		FROM dbo.Meta_Users u
		WHERE u.UserName = @UserName;
	
	IF @AssId IS NULL SET @Result = 1;
	IF @AssId IS NOT NULL
	BEGIN
		IF EXISTS(
			SELECT 1
				FROM Dbo.RunCars rc
				INNER JOIN dbo.RunPeriods rp
					ON rc.RunPeriodId  =rp.RunPeriodId
				WHERE rc.AssociateId =@AssId
					AND rp.RunId = @RunId
				)
		BEGIN
			SET @Result = 1;
		END;
	END;
	
	RETURN @Result;
END