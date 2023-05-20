/* ***************************************************************************
	Maurizio Battisti
	15/05/2023
	Reimposta la password di un utente
*************************************************************************** */
CREATE PROCEDURE [dbo].[up_User_ResetPassword]
	@UserName		varchar(128),
	@PasswordHash	varchar(MAX),
	@SetMustChange	bit = 0
AS
BEGIN
	UPDATE [dbo].[Meta_Users]
		SET PasswordHash =@PasswordHash,
			MustChangePassword = @SetMustChange
		WHERE UserName = @UserName;

	RETURN 0;
END
