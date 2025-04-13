CREATE PROCEDURE [dbo].[Load_BillDetail_By_BillId]
	@BillId INT
AS
BEGIN
	SELECT *
	FROM dbo.BillDetail
	WHERE BillId = @BillId;
END