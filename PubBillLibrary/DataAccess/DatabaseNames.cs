namespace PubBillLibrary.DataAccess;

public static class TableNames
{
	public static string Location => "Location";
	public static string Person => "Person";
	public static string User => "User";
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

	public static string UpdateLocation => "Update_Location";
	public static string UpdatePerson => "Update_Person";
	public static string UpdateUser => "Update_User";
	public static string UpdateDiningArea => "Update_DiningArea";
	public static string UpdateDiningTable => "Update_DiningTable";

	public static string LoadTableData => "Load_TableData";
	public static string LoadTableDataById => "Load_TableData_By_Id";
	public static string LoadTableDataByStatus => "Load_TableData_By_Status";
	public static string LoadUserByPassword => "Load_User_By_Password";

	public static string LoadPersonByNumber => "Load_Person_By_Number";
	public static string LoadDiningAreaByLocation => "Load_DiningArea_By_Location";
	public static string LoadDiningTableByDiningArea => "Load_DiningTable_By_DiningArea";
}
