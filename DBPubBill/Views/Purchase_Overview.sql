CREATE VIEW [dbo].[Purchase_Overview]
	AS
SELECT
	p.Id,
	p.BillNo,
	p.BillDate,
	p.Remarks,
	p.Status,
	p.SupplierId,
	s.Name AS SupplierName,
	s.Phone AS SupplierPhone,
	s.Code AS SupplierCode,
	s.Address AS SupplierAddress,
	s.GSTNo AS SupplierGSTNo,
	s.Email AS SupplierEmail,
	s.Status AS SupplierStatus,
	p.CDPercent AS CashDiscountPercent,
	p.CDAmount AS CashDiscountAmount,

	COUNT(DISTINCT pd.RawMaterialId) AS TotalRawMaterials,
	SUM(pd.Quantity) AS TotalQuantity,

	AVG(pd.SGSTPercent) AS SGSTPercent,
	AVG(pd.CGSTPercent) AS CGSTPercent,
	AVG(pd.IGSTPercent) AS IGSTPercent,

	SUM(pd.SGSTAmount) AS SGSTAmount,
	SUM(pd.CGSTAmount) AS CGSTAmount,
	SUM(pd.IGSTAmount) AS IGSTAmount,

	SUM(pd.DiscAmount / pd.BaseTotal * 100) AS DiscountPercent,
	SUM(pd.DiscAmount) AS DiscountAmount,
	SUM(pd.SGSTAmount + pd.CGSTAmount + pd.IGSTAmount) AS TotalTaxAmount,

	SUM(pd.BaseTotal) AS BaseTotal,
	SUM(pd.AfterDiscount) AS AfterDiscount,
	SUM(pd.Total) AS AfterTax,
	SUM(pd.Total - p.CDAmount) AS FinalAmount
FROM
	[dbo].[Purchase] p
INNER JOIN
	[dbo].[PurchaseDetail] pd ON p.Id = pd.PurchaseId
INNER JOIN
	[dbo].[Supplier] s ON p.SupplierId = s.Id
WHERE
	pd.Status = 1
GROUP BY
	p.Id,
	p.BillNo,
	p.BillDate,
	p.Remarks,
	p.Status,
	p.SupplierId,
	s.Name,
	s.Phone,
	s.Code,
	s.Address,
	s.GSTNo,
	s.Email,
	s.Status,
	p.CDPercent,
	p.CDAmount