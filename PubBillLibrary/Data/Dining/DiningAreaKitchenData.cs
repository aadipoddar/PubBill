namespace PubBillLibrary.Data.Dining;

public static class DiningAreaKitchenData
{
	public static async Task InsertDiningAreaKitchen(DiningAreaKitchenModel diningAreaKitchen) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertDiningAreaKitchen, diningAreaKitchen);
}
