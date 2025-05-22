namespace PubBillLibrary.Models.Inventory.Items;

public class StockModel
{
	public int Id { get; set; }
	public int RawMaterialId { get; set; }
	public decimal Quantity { get; set; }
	public string Type { get; set; }
	public int? PurchaseId { get; set; }
	public DateOnly TransactionDate { get; set; }
}

public enum StockType
{
	Purchase,
	PurchaseReturn,
	Sale
}

public class StockDateModel
{
	public int RawMaterialId { get; set; }
	public string RawMaterialName { get; set; }
	public string RawMaterialCode { get; set; }
	public decimal OpeningStock { get; set; }
	public decimal PurchaseStock { get; set; }
	public decimal SaleStock { get; set; }
	public decimal MonthlyStock { get; set; }
	public decimal ClosingStock { get; set; }
}