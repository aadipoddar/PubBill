namespace PubBillLibrary.Data;

public static class ProductGroupData
{
	public static async Task InsertProductGroup(ProductGroupModel productGroup) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertProductGroup, productGroup);
}
