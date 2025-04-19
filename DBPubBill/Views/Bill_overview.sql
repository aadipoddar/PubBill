CREATE VIEW [dbo].[Bill_Overview]
	AS
SELECT 
    b.Id AS BillId,
    b.LocationId AS LocationId,
    l.Name AS LocationName,
    b.DiningAreaId,
    da.Name AS DiningAreaName,
    b.DiningTableId,
    dt.Name AS DiningTableName,
    b.UserId,
    u.Name AS UserName,
    b.BillDateTime,
    b.PersonId,
    p.Name AS PersonName,
    p.Number AS PersonNumber,
    p.Loyalty AS PersonLoyalty,
    b.TotalPeople,
    b.DiscPercent AS DiscountPercent,
    b.DiscReason AS DiscountReason,
    b.ServicePercent,
    b.Remarks,
    COUNT(DISTINCT bd.ProductId) AS TotalProducts,
    SUM(bd.Quantity) AS TotalQuantity,
    (SUM(bd.Quantity * bd.Rate * bd.SGST / 100) / SUM(bd.Quantity * bd.Rate) * 100) AS SGSTPercent,
    (SUM(bd.Quantity * bd.Rate * bd.CGST / 100) / SUM(bd.Quantity * bd.Rate) * 100) AS CGSTPercent,
    (SUM(bd.Quantity * bd.Rate * bd.IGST / 100) / SUM(bd.Quantity * bd.Rate) * 100) AS IGSTPercent,
    SUM(bd.Quantity * bd.Rate * bd.SGST / 100) AS SGSTAmount,
    SUM(bd.Quantity * bd.Rate * bd.CGST / 100) AS CGSTAmount,
    SUM(bd.Quantity * bd.Rate * bd.IGST / 100) AS IGSTAmount,
    SUM(bd.Quantity * bd.Rate * (b.DiscPercent / 100)) AS DiscountAmount,
    SUM(bd.Quantity * bd.Rate * ((bd.CGST + bd.SGST + bd.IGST) / 100)) AS TotalTaxAmount,
    SUM(((bd.Quantity * bd.Rate) +
        (bd.Quantity * bd.Rate * ((bd.CGST + bd.SGST + bd.IGST) / 100)) -
        (bd.Quantity * bd.Rate * (b.DiscPercent / 100))) * (b.ServicePercent / 100)) AS ServiceAmount,
    SUM(bd.Quantity * bd.Rate) AS BaseTotal,
    SUM(bd.Quantity * bd.Rate * (1 - b.DiscPercent / 100)) AS AfterDiscount,
    SUM((bd.Quantity * bd.Rate) +
        (bd.Quantity * bd.Rate * ((bd.CGST + bd.SGST + bd.IGST) / 100)) -
        (bd.Quantity * bd.Rate * (b.DiscPercent / 100))) AS AfterTax,
    SUM(((bd.Quantity * bd.Rate) +
        (bd.Quantity * bd.Rate * ((bd.CGST + bd.SGST + bd.IGST) / 100)) -
        (bd.Quantity * bd.Rate * (b.DiscPercent / 100))) * (1 + b.ServicePercent / 100)) AS AfterService,
    SUM(b.EntryPaid) AS EntryPaid,
    SUM(((bd.Quantity * bd.Rate) +
        (bd.Quantity * bd.Rate * ((bd.CGST + bd.SGST + bd.IGST) / 100)) -
        (bd.Quantity * bd.Rate * (b.DiscPercent / 100))) * (1 + b.ServicePercent / 100)) -
        SUM(b.EntryPaid) AS FinalAmount,

    (SELECT Amount FROM [dbo].[BillPaymentDetail] bpd WHERE bpd.BillId = b.Id AND bpd.PaymentModeId = 1 AND bpd.Status = 1) AS [Cash],
    (SELECT Amount FROM [dbo].[BillPaymentDetail] bpd WHERE bpd.BillId = b.Id AND bpd.PaymentModeId = 2 AND bpd.Status = 1) AS [Card],
    (SELECT Amount FROM [dbo].[BillPaymentDetail] bpd WHERE bpd.BillId = b.Id AND bpd.PaymentModeId = 3 AND bpd.Status = 1) AS [UPI]
FROM 
    [dbo].[Bill] b
INNER JOIN 
    [dbo].[BillDetail] bd ON b.Id = bd.BillId
INNER JOIN
    [dbo].[Location] l ON b.LocationId = l.Id
INNER JOIN
    [dbo].[DiningArea] da ON b.DiningAreaId = da.Id
INNER JOIN
    [dbo].[DiningTable] dt ON b.DiningTableId = dt.Id
INNER JOIN
    [dbo].[User] u ON b.UserId = u.Id
LEFT JOIN
    [dbo].[Person] p ON b.PersonId = p.Id
WHERE
    bd.Cancelled = 0
    AND bd.Status = 1
GROUP BY 
    b.Id, 
    b.LocationId,
    b.DiningAreaId,
    b.DiningTableId,
    b.UserId,
    l.Name, 
    da.Name,
    dt.Name,
    u.Name, 
    b.BillDateTime, 
    b.PersonId,
    p.Name,
    p.Number,
    p.Loyalty,
    b.TotalPeople,
    b.DiscPercent, 
    b.DiscReason,
    b.ServicePercent,
    b.Remarks;