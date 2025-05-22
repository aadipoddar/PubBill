namespace PubBillLibrary.Models.Products;

public class ProductModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Code { get; set; }
	public int ProductCategoryId { get; set; }
	public decimal Rate { get; set; }
	public int TaxId { get; set; }
	public int KitchenTypeId { get; set; }
	public bool Status { get; set; }
}

public class ProductTaxModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Code { get; set; }
	public int ProductCategoryId { get; set; }
	public decimal Rate { get; set; }
	public string TaxCode { get; set; }
	public decimal CGSTPercent { get; set; }
	public decimal SGSTPercent { get; set; }
	public decimal IGSTPercent { get; set; }
	public decimal CGSTAmount { get; set; }
	public decimal SGSTAmount { get; set; }
	public decimal IGSTAmount { get; set; }
	public decimal TotalTax { get; set; }
	public decimal Total { get; set; }
	public bool Status { get; set; }
}

public class ItemDetailModel
{
	public int ProductId { get; set; }
	public string ProductName { get; set; }
	public string ProductCode { get; set; }
	public string CategoryName { get; set; }
	public string GroupName { get; set; }
	public decimal TotalQuantity { get; set; }
	public decimal AverageRate { get; set; }
	public decimal TotalBaseAmount { get; set; }
	public decimal TotalDiscountAmount { get; set; }
	public decimal TotalAfterDiscount { get; set; }
	public decimal TotalCGSTAmount { get; set; }
	public decimal TotalSGSTAmount { get; set; }
	public decimal TotalIGSTAmount { get; set; }
	public decimal TotalSaleAmount { get; set; }
}