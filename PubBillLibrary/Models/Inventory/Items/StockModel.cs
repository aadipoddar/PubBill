namespace PubBillLibrary.Models.Inventory.Items;

public class StockModel
{
	public int Id { get; set; }
	public int RawMaterialId { get; set; }
	public decimal Quantity { get; set; }
	public string Type { get; set; }
	public int? PurchaseId { get; set; }
	public DateOnly? TransactionDate { get; set; }
	public bool Status { get; set; }
}
