CREATE PROCEDURE [dbo].[Delete_KOTBillDetail]
	@RunningBillId INT
AS
BEGIN
	DELETE FROM dbo.KOTBillDetail WHERE RunningBillId = @RunningBillId;
END