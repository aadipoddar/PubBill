namespace PubBillLibrary.Models.Inventory.Items;

public class RawMaterialModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Code { get; set; }
	public int RawMaterialCategoryId { get; set; }
	public decimal MRP { get; set; }
	public int TaxId { get; set; }
	public bool Status { get; set; }
}