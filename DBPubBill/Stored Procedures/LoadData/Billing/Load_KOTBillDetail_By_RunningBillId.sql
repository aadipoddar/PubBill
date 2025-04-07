CREATE PROCEDURE [dbo].[Load_KOTBillDetail_By_RunningBillId]
	@RunningBillId INT
AS
BEGIN
	SELECT *
	FROM dbo.KOTBillDetail
	WHERE RunningBillId = @RunningBillId;
END