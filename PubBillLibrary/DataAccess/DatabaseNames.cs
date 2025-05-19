namespace PubBillLibrary.DataAccess;

public static class TableNames
{
	public static string Location => "Location";
	public static string Person => "Person";
	public static string User => "User";
	public static string PaymentMode => "PaymentMode";
	public static string Settings => "Settings";

	public static string DiningArea => "DiningArea";
	public static string DiningTable => "DiningTable";
	public static string DiningAreaKitchen => "DiningAreaKitchen";

	public static string ProductGroup => "ProductGroup";
	public static string ProductCategory => "ProductCategory";
	public static string Product => "Product";
	public static string Tax => "Tax";

	public static string Bill => "Bill";
	public static string BillDetail => "BillDetail";
	public static string RunningBill => "RunningBill";
	public static string RunningBillDetail => "RunningBillDetail";
	public static string BillPaymentDetail => "BillPaymentDetail";

	public static string KOTBillDetail => "KOTBillDetail";
	public static string Kitchen => "Kitchen";
	public static string KitchenType => "KitchenType";


	public static string RawMaterial => "RawMaterial";
	public static string RawMaterialCategory => "RawMaterialCategory";
	public static string Recipe => "Recipe";
	public static string RecipeDetail => "RecipeDetail";

	public static string Purchase => "Purchase";
	public static string PurchaseDetail => "PurchaseDetail";
	public static string PurchasePaymentDetail => "PurchasePaymentDetail";
	public static string Supplier => "Supplier";
}

public static class StoredProcedureNames
{
	public static string InsertLocation => "Insert_Location";
	public static string InsertPerson => "Insert_Person";
	public static string InsertUser => "Insert_User";
	public static string InsertPaymentMode => "Insert_PaymentMode";

	public static string InsertDiningArea => "Insert_DiningArea";
	public static string InsertDiningAreaKitchen => "Insert_DiningAreaKitchen";
	public static string InsertDiningTable => "Insert_DiningTable";

	public static string InsertProductGroup => "Insert_ProductGroup";
	public static string InsertProductCategory => "Insert_ProductCategory";
	public static string InsertProduct => "Insert_Product";
	public static string InsertTax => "Insert_Tax";

	public static string InsertBill => "Insert_Bill";
	public static string InsertBillDetail => "Insert_BillDetail";
	public static string InsertRunningBill => "Insert_RunningBill";
	public static string InsertRunningBillDetail => "Insert_RunningBillDetail";
	public static string InsertBillPaymentDetail => "Insert_BillPaymentDetail";

	public static string InsertKOTBillDetail => "Insert_KOTBillDetail";
	public static string InsertKitchen => "Insert_Kitchen";
	public static string InsertKitchenType => "Insert_KitchenType";

	public static string InsertRawMaterial => "Insert_RawMaterial";
	public static string InsertRawMaterialCategory => "Insert_RawMaterialCategory";
	public static string InsertRecipe => "Insert_Recipe";
	public static string InsertRecipeDetail => "Insert_RecipeDetail";

	public static string InsertPurchase => "Insert_Purchase";
	public static string InsertPurchaseDetail => "Insert_PurchaseDetail";
	public static string InsertPurchasePaymentDetail => "Insert_PurchasePaymentDetail";
	public static string InsertSupplier => "Insert_Supplier";


	public static string DeleteRunningBillDetail => "Delete_RunningBillDetail";

	public static string LoadSettingsByKey => "Load_Settings_By_Key";
	public static string UpdateSettings => "Update_Settings";
	public static string ResetSettings => "Reset_Settings";


	public static string LoadTableData => "Load_TableData";
	public static string LoadTableDataById => "Load_TableData_By_Id";
	public static string LoadTableDataByStatus => "Load_TableData_By_Status";
	public static string LoadUserByPassword => "Load_User_By_Password";

	public static string LoadPersonByNumber => "Load_Person_By_Number";

	public static string LoadDiningAreaByLocation => "Load_DiningArea_By_Location";
	public static string LoadDiningTableByDiningArea => "Load_DiningTable_By_DiningArea";

	public static string LoadProductByProductCategory => "Load_Product_By_ProductCategory";
	public static string LoadProductCategoryByProductGroup => "Load_ProductCategory_By_ProductGroup";

	public static string LoadRunningTabeDetailByRunningBillId => "Load_RunningTabeDetail_By_RunningBillId";
	public static string LoadKOTBillDetailByRunningBillId => "Load_KOTBillDetail_By_RunningBillId";
	public static string LoadBillDetailByBillId => "Load_BillDetail_By_BillId";
	public static string LoadBillPaymentDetailByBillId => "Load_BillPaymentDetail_By_BillId";
	public static string LoadBillDetailsByDateLocationId => "Load_BillDetails_By_Date_LocationId";


	public static string LoadRawMaterialByRawMaterialCategory => "Load_RawMaterial_By_RawMaterialCategory";

	public static string LoadRecipeByProduct => "Load_Recipe_By_Product";
	public static string LoadRecipeDetailByRecipe => "Load_RecipeDetail_By_Recipe";
}

public static class ViewNames
{
	public static string ProductTax => "Product_Tax";
	public static string BillOverview => "Bill_Overview";
}

public static class PubEntryStoredProcedures
{
	public static string LaodTransactionByLocationPerson => "Laod_Transaction_By_Location_Person";
	public static string LoadAdvanceByTransactionId => "Load_Advance_By_TransactionId";
	public static string LoadAdvanceDetailByAdvanceId => "Load_AdvanceDetail_By_AdvanceId";
}