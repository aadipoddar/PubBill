namespace PubBillLibrary.Models.Dining;

public class DiningAreaKitchenModel
{
	public int Id { get; set; }
	public int DiningAreaId { get; set; }
	public int KitchenId { get; set; }
	public bool Status { get; set; }
}
