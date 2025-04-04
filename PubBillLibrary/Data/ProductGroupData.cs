namespace PubBillLibrary.Data;

public class ProductGroupData
{
	public static async Task InsertProductGroup(ProductGroupModel productGroup) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertProductGroup, productGroup);

	public static async Task UpdateProductGroup(ProductGroupModel productGroup) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.UpdateProductGroup, productGroup);
}
