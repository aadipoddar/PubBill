CREATE PROCEDURE [dbo].[Load_RunningTabeDetail_By_RunningBillId]
	@RunningBillId INT
AS
BEGIN
	SELECT *
	FROM dbo.RunningBillDetail
	WHERE RunningBillId = @RunningBillId;
END