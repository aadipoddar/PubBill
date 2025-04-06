namespace PubBillLibrary.Models;

public class DiningTableModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int DiningAreaId { get; set; }
	public bool Running { get; set; }
	public bool Status { get; set; }
}