CREATE VIEW [dbo].[Product_Tax]
	AS
		SELECT
		    p.[Id],
		    p.[Name],
		    p.[Code],
		    p.[ProductCategoryId],
		    p.[Rate],
			t.Code AS TaxCode,
		    t.CGST AS CGSTPercent,
		    t.SGST AS SGSTPercent,
		    t.IGST AS IGSTPercent,
		    CAST((p.[Rate] * t.CGST / 100) AS money) AS CGSTAmount,
		    CAST((p.[Rate] * t.SGST / 100) AS money) AS SGSTAmount,
		    CAST((p.[Rate] * t.IGST / 100) AS money) AS IGSTAmount,
		    CAST((p.[Rate] * (t.CGST + t.SGST + t.IGST) / 100) AS money) AS TotalTax,
		    CAST(p.[Rate] + (p.[Rate] * (t.CGST + t.SGST + t.IGST) / 100) AS money) AS Total,
		    p.[Status]
		FROM
		    Product p
		JOIN Tax t ON p.TaxId = t.Id