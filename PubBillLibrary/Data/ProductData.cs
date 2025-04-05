namespace PubBillLibrary.Data;

public static class ProductData
{
	public static async Task InsertProduct(ProductModel product) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertProduct, product);

	public static async Task<List<ProductModel>> LoadProductByProductCategory(int ProductCategoryId) =>
			await SqlDataAccess.LoadData<ProductModel, dynamic>(StoredProcedureNames.LoadProductByProductCategory, new { ProductCategoryId });
}
