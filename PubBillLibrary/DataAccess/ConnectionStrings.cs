namespace PubBillLibrary.DataAccess;

public static class ConnectionStrings
{
	public static string Azure => $"Server=tcp:goldenpark.database.windows.net,1433;Initial Catalog={Secrets.DatabaseName};Persist Security Info=False;User ID={Secrets.DatabaseUserId};Password={Secrets.DatabasePassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
	public static string Local => $"Data Source=AADILAPI;Initial Catalog={Secrets.DatabaseName};Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";

	public static string PubEntryAzure => $"Server=tcp:goldenpark.database.windows.net,1433;Initial Catalog={Secrets.PubEntryDatabaseName};Persist Security Info=False;User ID={Secrets.DatabaseUserId};Password={Secrets.DatabasePassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
	public static string PubEntryLocal => $"Data Source=AADILAPI;Initial Catalog={Secrets.PubEntryDatabaseName};Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False";
}
