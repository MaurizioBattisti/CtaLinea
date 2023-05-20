/* ***********************************************************************************
	Maurizio Battisti
	14/05/2023
	Lista degli utenti senza password ma con i ruoli
************************************************************************************* */
CREATE VIEW [dbo].[vw_Users]
AS
WITH CTE_Roles AS
(
	SELECT r.UserName,
			STRING_AGG(r.RoleId, ',') WITHIN GROUP (ORDER BY r.RoleId) AS Roles
		FROM Dbo.Meta_Roles r
		GROUP BY r.UserName
)
SELECT u.UserName,
		u.Description,
		u.Email,
		u.Expiration,
		u.MustChangePassword,
		u.AssociateId,
		a.Description AS AssociateDescription,
		u.Interactive,
		r.Roles
	FROM Dbo.Meta_Users u
	LEFT JOIN CTE_Roles r
		ON u.UserName = r.UserName
	LEFT JOIN Dbo.Associates a
		ON u.AssociateId  = a.AssociateId
	;
