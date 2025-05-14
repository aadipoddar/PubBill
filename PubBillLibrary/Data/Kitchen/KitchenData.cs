﻿namespace PubBillLibrary.Data.Kitchen;

public static class KitchenData
{
	public static async Task InsertKitchen(KitchenModel kitchen) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertKitchen, kitchen);
}
