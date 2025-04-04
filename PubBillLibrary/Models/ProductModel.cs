namespace PubBillLibrary.Models;

public class ProductModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public decimal Prize { get; set; }
	public int ProductCategoryId { get; set; }
	public bool Status { get; set; }
}
