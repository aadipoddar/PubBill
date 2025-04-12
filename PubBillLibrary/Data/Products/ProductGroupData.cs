namespace PubBillLibrary.Data.Products;

public static class ProductGroupData
{
	public static async Task InsertProductGroup(ProductGroupModel productGroup) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertProductGroup, productGroup);
}
