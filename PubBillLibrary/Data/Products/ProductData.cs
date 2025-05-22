namespace PubBillLibrary.Data.Products;

public static class ProductData
{
	public static async Task InsertProduct(ProductModel product) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertProduct, product);

	public static async Task<List<ProductModel>> LoadProductByProductCategory(int ProductCategoryId) =>
			await SqlDataAccess.LoadData<ProductModel, dynamic>(StoredProcedureNames.LoadProductByProductCategory, new { ProductCategoryId });

	public static async Task<List<ItemDetailModel>> LoadItemDetailsByDate(DateTime FromDate, DateTime ToDate) =>
			await SqlDataAccess.LoadData<ItemDetailModel, dynamic>(StoredProcedureNames.LoadItemDetailsByDate, new { FromDate, ToDate });
}
