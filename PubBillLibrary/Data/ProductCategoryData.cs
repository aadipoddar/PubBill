namespace PubBillLibrary.Data;

public static class ProductCategoryData
{
	public static async Task InsertProductCategory(ProductCategoryModel productCategory) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertProductCategory, productCategory);

	public static async Task<List<ProductCategoryModel>> LoadProductCategoryByProductGroup(int ProductGroupId) =>
			await SqlDataAccess.LoadData<ProductCategoryModel, dynamic>(StoredProcedureNames.LoadProductCategoryByProductGroup, new { ProductGroupId });
}
