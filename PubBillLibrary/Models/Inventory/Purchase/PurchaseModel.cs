namespace PubBillLibrary.Models.Inventory.Purchase;

public class PurchaseModel
{
	public int Id { get; set; }
	public string BillNo { get; set; }
	public int SupplierId { get; set; }
	public DateOnly BillDate { get; set; }
	public string Remarks { get; set; }
	public bool Status { get; set; }
}