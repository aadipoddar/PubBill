namespace PubBillLibrary.Models.Kitchen;

public class KitchenModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int KitchenTypeId { get; set; }
	public string PrinterName { get; set; }
	public bool Status { get; set; }
}
