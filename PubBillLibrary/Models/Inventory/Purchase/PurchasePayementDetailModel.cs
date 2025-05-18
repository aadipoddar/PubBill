namespace PubBillLibrary.Models.Inventory.Purchase;

public class PurchasePayementDetailModel
{
	public int Id { get; set; }
	public int PurchaseId { get; set; }
	public int PaymentModeId { get; set; }
	public decimal Amount { get; set; }
	public bool Status { get; set; }
}