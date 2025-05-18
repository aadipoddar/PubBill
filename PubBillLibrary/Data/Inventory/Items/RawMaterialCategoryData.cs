namespace PubBillLibrary.Data.Inventory.Items;

public static class RawMaterialCategoryData
{
	public static async Task InsertRawMaterialCategory(RawMaterialCategoryModel rawMaterialCategoryModel) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertRawMaterialCategory, rawMaterialCategoryModel);
}
