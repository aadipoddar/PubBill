CREATE PROCEDURE [dbo].[Load_StockDetails_By_Date]
	@FromDate DATETIME,
	@ToDate DATETIME
AS
BEGIN
	SELECT
	*
	FROM Stock

END