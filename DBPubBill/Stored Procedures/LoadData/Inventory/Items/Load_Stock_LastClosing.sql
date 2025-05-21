CREATE PROCEDURE [dbo].[Load_Stock_LastClosing]
AS
BEGIN
	SELECT
	*
	FROM Stock
	WHERE PurchaseId IS NULL
	AND TransactionDT = (SELECT MAX(TransactionDT) FROM Stock)

END