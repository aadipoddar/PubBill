namespace PubBillLibrary.Models;

public class UserModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public short Password { get; set; }
	public int LocationId { get; set; }
	public bool Status { get; set; }
	public bool Bill { get; set; }
	public bool KOT { get; set; }
	public bool Inventory { get; set; }
	public bool Admin { get; set; }
}

public class UserLocationModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public short Password { get; set; }
	public int LocationId { get; set; }
	public string LocationName { get; set; }
	public bool Status { get; set; }
	public bool Bill { get; set; }
	public bool KOT { get; set; }
	public bool Inventory { get; set; }
	public bool Admin { get; set; }
}