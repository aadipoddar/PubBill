﻿namespace PubBillLibrary.Data;

public class DiningTableData
{
	public static async Task InsertDiningTable(DiningTableModel diningTable) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.InsertDiningTable, diningTable);

	public static async Task UpdateDiningTable(DiningTableModel diningTable) =>
			await SqlDataAccess.SaveData(StoredProcedureNames.UpdateDiningTable, diningTable);

	public static async Task<List<DiningTableModel>> LoadDiningTableByDiningArea(int DiningAreaId) =>
			await SqlDataAccess.LoadData<DiningTableModel, dynamic>(StoredProcedureNames.LoadDiningTableByDiningArea, new { DiningAreaId });
}