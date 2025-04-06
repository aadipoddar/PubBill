CREATE PROCEDURE [dbo].[Delete_RunningBill]
	@Id INT
AS
BEGIN
	DELETE FROM RunningBill WHERE Id = @Id;
END