CREATE PROCEDURE [dbo].[Load_ItemDetails_By_Date]
	@FromDate DATETIME,
    @ToDate DATETIME
AS
BEGIN
	SELECT 
		p.Id AS ProductId,
		p.Name AS ProductName,
		p.Code AS ProductCode,
		pc.Name AS CategoryName,
		pg.Name AS GroupName,
		SUM(bd.Quantity) AS TotalQuantity,
		AVG(bd.Rate) AS AverageRate,
		SUM(bd.BaseTotal) AS TotalBaseAmount,
		SUM(bd.DiscAmount) AS TotalDiscountAmount,
		SUM(bd.AfterDiscount) AS TotalAfterDiscount,
		SUM(bd.CGSTAmount) AS TotalCGSTAmount,
		SUM(bd.SGSTAmount) AS TotalSGSTAmount,
		SUM(bd.IGSTAmount) AS TotalIGSTAmount,
		SUM(bd.Total) AS TotalSaleAmount
	FROM 
		BillDetail bd
	INNER JOIN 
		Bill b ON bd.BillId = b.Id
	INNER JOIN 
		Product p ON bd.ProductId = p.Id
	INNER JOIN 
		ProductCategory pc ON p.ProductCategoryId = pc.Id
	INNER JOIN 
		ProductGroup pg ON pc.ProductGroupId = pg.Id
	WHERE
		b.BillDateTime BETWEEN @FromDate AND @ToDate
	GROUP BY 
		p.Id, p.Name, p.Code, pc.Name, pg.Name
	ORDER BY 
		pg.Name, pc.Name, p.Name; 
END