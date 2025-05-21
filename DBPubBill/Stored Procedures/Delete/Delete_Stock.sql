CREATE PROCEDURE [dbo].[Delete_Stock]
	@StockId INT
AS
BEGIN
	DELETE FROM Stock WHERE Id = @StockId;
END