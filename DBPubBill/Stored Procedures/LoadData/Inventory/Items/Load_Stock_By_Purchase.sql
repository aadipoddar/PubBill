CREATE PROCEDURE [dbo].[Load_Stock_By_Purchase]
	@PurchaseId INT
AS
BEGIN
	SELECT
		*
	FROM [Stock]
	WHERE PurchaseId = @PurchaseId
		AND Status = 1
END