namespace PubBillLibrary.Models;

public class BillModel
{
	public string ProductName { get; set; }
	public double Quantity { get; set; }
	public double Price { get; set; }
	public double Total => Quantity * Price;
	public string Instructions { get; set; }
}