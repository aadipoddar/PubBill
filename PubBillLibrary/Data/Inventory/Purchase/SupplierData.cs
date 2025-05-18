﻿namespace PubBillLibrary.Data.Inventory.Purchase;

public static class SupplierData
{
	public static async Task InsertSupplier(SupplierModel supplier) =>
		await SqlDataAccess.SaveData(StoredProcedureNames.InsertSupplier, supplier);
}