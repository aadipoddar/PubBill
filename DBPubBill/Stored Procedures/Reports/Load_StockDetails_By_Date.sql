CREATE PROCEDURE [dbo].[Load_StockDetails_By_Date]
	@FromDate DATE,
	@ToDate DATE
AS
BEGIN
SELECT s.RawMaterialId,
       r.[Name] RawMaterialName,
       r.Code RawMaterialCode,
       ISNULL
       (
          (SELECT SUM (Quantity)
           FROM Stock
           WHERE     RawMaterialId = s.RawMaterialId
                 AND TransactionDate < @FromDate),
          0) AS OpeningStock,
       ISNULL
       (
          (SELECT SUM (Quantity)
           FROM Stock
           WHERE     RawMaterialId = s.RawMaterialId
                 AND TransactionDate >= @FromDate
                 AND TransactionDate < @ToDate
                 AND PurchaseId IS NOT NULL),
          0) AS PurchaseStock,
       ISNULL
       (
          (SELECT SUM (Quantity)
           FROM Stock
           WHERE     RawMaterialId = s.RawMaterialId
                 AND TransactionDate >= @FromDate
                 AND TransactionDate < @ToDate
                 AND PurchaseId IS NULL),
          0) AS SaleStock,
       ISNULL
       (
          (SELECT SUM (Quantity)
           FROM Stock
           WHERE     RawMaterialId = s.RawMaterialId
                 AND TransactionDate >= @FromDate
                 AND TransactionDate < @ToDate),
          0) AS MonthlyStock,
       (  ISNULL
          (
             (SELECT SUM (Quantity)
              FROM Stock
              WHERE     RawMaterialId = s.RawMaterialId
                    AND TransactionDate < @FromDate),
             0)
        + ISNULL
          (
             (SELECT SUM (Quantity)
              FROM Stock
              WHERE     RawMaterialId = s.RawMaterialId
                    AND TransactionDate >= @FromDate
                    AND TransactionDate < @ToDate),
             0)) AS ClosingStock
FROM Stock s LEFT JOIN dbo.RawMaterial r ON r.Id = s.RawMaterialId
GROUP BY s.RawMaterialId, r.[Name], r.Code;
END