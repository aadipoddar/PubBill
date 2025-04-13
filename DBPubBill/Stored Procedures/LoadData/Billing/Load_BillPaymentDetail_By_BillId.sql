CREATE PROCEDURE [dbo].[Load_BillPaymentDetail_By_BillId]
	@BIllId INT
AS
BEGIN
	SELECT *
	FROM dbo.BillPaymentDetail
	WHERE BillId = @BillId
END