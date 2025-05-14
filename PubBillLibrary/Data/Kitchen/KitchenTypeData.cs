namespace PubBillLibrary.Data.Kitchen;

public static class KitchenTypeData
{
	public static async Task InsertKitchenType(KitchenTypeModel kitchenType) =>
	await SqlDataAccess.SaveData(StoredProcedureNames.InsertKitchenType, kitchenType);
}