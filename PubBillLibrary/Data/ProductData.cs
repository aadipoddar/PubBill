namespace PubBillLibrary.Data;

public class ProductData
{
	public static async Task InsertProduct(ProductModel product) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertProduct, product);

	public static async Task UpdateProduct(ProductModel product) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.UpdateProduct, product);

	public static async Task<List<ProductModel>> LoadProductByProductCategory(int ProductCategoryId) =>
			await SqlDataAccess.LoadData<ProductModel, dynamic>(StoredProcedureNames.LoadProductByProductCategory, new { ProductCategoryId });
}
