﻿namespace PubBillLibrary.DataAccess;

public static class TableNames
{
	public static string Location => "Location";
	public static string Person => "Person";
	public static string User => "User";
	public static string DiningArea => "DiningArea";
	public static string DiningTable => "DiningTable";
	public static string ProductGroup => "ProductGroup";
	public static string ProductCategory => "ProductCategory";
	public static string Product => "Product";
	public static string PaymentMode => "PaymentMode";
	public static string Bill => "Bill";
	public static string BillDetail => "BillDetail";
	public static string RunningBill => "RunningBill";
	public static string RunningBillDetail => "RunningBillDetail";
	public static string KOTBillDetail => "KOTBillDetail";
	public static string BillPaymentDetail => "BillPaymentDetail";
}

public static class ViewNames
{
}

public static class StoredProcedureNames
{
	public static string InsertLocation => "Insert_Location";
	public static string InsertPerson => "Insert_Person";
	public static string InsertUser => "Insert_User";
	public static string InsertDiningArea => "Insert_DiningArea";
	public static string InsertDiningTable => "Insert_DiningTable";
	public static string InsertProductGroup => "Insert_ProductGroup";
	public static string InsertProductCategory => "Insert_ProductCategory";
	public static string InsertProduct => "Insert_Product";
	public static string InsertPaymentMode => "Insert_PaymentMode";
	public static string InsertBill => "Insert_Bill";
	public static string InsertBillDetail => "Insert_BillDetail";
	public static string InsertRunningBill => "Insert_RunningBill";
	public static string InsertRunningBillDetail => "Insert_RunningBillDetail";
	public static string InsertKOTBillDetail => "Insert_KOTBillDetail";
	public static string InsertBillPaymentDetail => "Insert_BillPaymentDetail";

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

	public static string DeleteRunningBillDetail => "Delete_RunningBillDetail";
}
