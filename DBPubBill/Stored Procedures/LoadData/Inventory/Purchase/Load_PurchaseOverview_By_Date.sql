CREATE PROCEDURE [dbo].[Load_PurchaseOverview_By_Date]
	@BillDate DATE
AS
BEGIN
	SELECT
		*
	FROM
		[dbo].[Purchase_Overview] AS po
	WHERE
		po.BillDate = @BillDate
END