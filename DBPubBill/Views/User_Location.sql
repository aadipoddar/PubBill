CREATE VIEW [dbo].[User_Location]
	AS
	SELECT
		ut.Id,
		ut.Name,
		ut.Password,
		lt.Id LocationId,
		lt.Name LocationName,
		ut.Status,
		ut.Bill,
		ut.KOT,
		ut.Inventory,
		ut.Admin
	FROM [User] ut
	LEFT JOIN Location lt ON ut.LocationId = lt.Id