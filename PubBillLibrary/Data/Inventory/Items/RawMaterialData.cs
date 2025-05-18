namespace PubBillLibrary.Data.Inventory.Items;

public static class RawMaterialData
{
	public static async Task InsertRawMaterial(RawMaterialModel rawMaterial) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertRawMaterial, rawMaterial);

	public static async Task<List<RawMaterialModel>> LoadRawMaterialByRawMaterialCategory(int RawMaterialCategoryId) =>
		await SqlDataAccess.LoadData<RawMaterialModel, dynamic>(StoredProcedureNames.LoadRawMaterialByRawMaterialCategory, new { RawMaterialCategoryId });
}
