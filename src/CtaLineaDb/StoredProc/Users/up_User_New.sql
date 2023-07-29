/* ***************************************************************************
	Maurizio Battisti
	15/05/2023
	Inserisce un nuovo utente
*************************************************************************** */
CREATE PROCEDURE [dbo].[up_User_New]
	@UserName			varchar(128),
	@PasswordHash		varchar(MAX),
	@Description		varchar(1024) = NULL,
	@Email				varchar(1024) = NULL,
	@Expiration			date = NULL,
	@MustChangePassword	bit = 1,
	@Interactive		bit = 1,
	@AssociateId		uniqueidentifier = NULL,
	@Roles				varchar(MAX) = NULL
AS
BEGIN
	SET DATEFIRST 1; -- this sets Monday to the first day of the week for the current connection.

	DECLARE @Tbl_Roles AS TABLE 
	(
		RoleId		varchar(32) PRIMARY KEY
	);
	IF @Roles IS NOT NULL 
		AND  @Interactive = 1
	BEGIN
		INSERT INTO @Tbl_Roles (RoleId)
		SELECT DISTINCT TRIM(value) FROM STRING_SPLIT(@Roles, ',');
	END;
	ELSE IF @Interactive = 1
	BEGIN
		-- solleva un errore
		RAISERROR ('Non sono stati indicati i ruoli dell''utente', 16, 1);
		RETURN -1;
	END;

	IF  @Interactive = 0
	BEGIN
		DELETE FROM @Tbl_Roles;
		INSERT INTO @Tbl_Roles 
			(RoleId)
			VALUES ('TASK');
	END;

	BEGIN TRAN;

	INSERT INTO [dbo].[Meta_Users]
			(UserName, 
			PasswordHash, Description,
			Email, Expiration,
			MustChangePassword, Interactive,
			AssociateId)
		VALUES (@UserName, 
			@PasswordHash, @Description,
			@Email, @Expiration,
			@MustChangePassword, @Interactive,
			@AssociateId);

	INSERT INTO [dbo].[Meta_Roles]
		([UserName], [RoleId])
		SELECT  @UserName, r.RoleId
			FROM @Tbl_Roles r;
	COMMIT;

	RETURN 0;
END
