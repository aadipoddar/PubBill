namespace PubBillLibrary.Models.Products;

public class ProductCategoryModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int ProductGroupId { get; set; }
	public bool Status { get; set; }
}
