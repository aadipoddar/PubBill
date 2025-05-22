CREATE PROCEDURE [dbo].[Load_Stock_LastClosing]
AS
BEGIN
	SELECT
	*
	FROM Stock
	WHERE PurchaseId IS NULL
	AND [TransactionDate] = (SELECT MAX([TransactionDate]) FROM Stock)

END