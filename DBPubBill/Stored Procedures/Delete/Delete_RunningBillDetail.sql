CREATE PROCEDURE [dbo].[Delete_RunningBillDetail]
	@RunningBillId INT
AS
BEGIN
	DELETE FROM dbo.RunningBillDetail WHERE RunningBillId = @RunningBillId;
END