namespace PubBillLibrary.Models.Inventory.Purchase;

public class PurchaseModel
{
	public int Id { get; set; }
	public string BillNo { get; set; }
	public int SupplierId { get; set; }
	public DateOnly BillDate { get; set; }
	public decimal CDPercent { get; set; }
	public decimal CDAmount { get; set; }
	public string Remarks { get; set; }
	public bool Status { get; set; }
}

public class PurchaseOverviewModel
{
	public int Id { get; set; }
	public string BillNo { get; set; }
	public DateOnly BillDate { get; set; }
	public string Remarks { get; set; }
	public bool Status { get; set; }
	public int SupplierId { get; set; }
	public string SupplierName { get; set; }
	public string SupplierPhone { get; set; }
	public string SupplierCode { get; set; }
	public string SupplierAddress { get; set; }
	public string SupplierGSTNo { get; set; }
	public string SupplierEmail { get; set; }
	public bool SupplierStatus { get; set; }
	public decimal CashDiscountPercent { get; set; }
	public decimal CashDiscountAmount { get; set; }
	public decimal TotalRawMaterials { get; set; }
	public decimal TotalQuantity { get; set; }
	public decimal SGSTPercent { get; set; }
	public decimal CGSTPercent { get; set; }
	public decimal IGSTPercent { get; set; }
	public decimal SGSTAmount { get; set; }
	public decimal CGSTAmount { get; set; }
	public decimal IGSTAmount { get; set; }
	public decimal DiscountPercent { get; set; }
	public decimal DiscountAmount { get; set; }
	public decimal TotalTaxAmount { get; set; }
	public decimal BaseTotal { get; set; }
	public decimal AfterDiscount { get; set; }
	public decimal AfterTax { get; set; }
	public decimal FinalAmount { get; set; }
}