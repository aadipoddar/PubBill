CREATE PROCEDURE [dbo].[Load_Stock_By_Purchase]
	@PurchaseId INT
AS
BEGIN
	SELECT
		*
	FROM [Stock]
	WHERE PurchaseId = @PurchaseId
END