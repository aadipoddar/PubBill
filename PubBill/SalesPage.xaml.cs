using System.Collections.ObjectModel;
using System.Windows;

namespace PubBill;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class SalesPage : Window
{
	private ObservableCollection<Sale> sales = new ObservableCollection<Sale>
	{
		new Sale { ProductName = "Chips", Quantity = 3, Price = 1.5 },
		new Sale { ProductName = "Peanuts", Quantity = 1, Price = 1.0 },
		new Sale { ProductName = "Pretzels", Quantity = 2, Price = 1.5 },
		new Sale { ProductName = "Soda", Quantity = 2, Price = 1.5 },
		new Sale { ProductName = "Water", Quantity = 1, Price = 1.0 },
		new Sale { ProductName = "Juice", Quantity = 1, Price = 2.0 },
		new Sale { ProductName = "Candy", Quantity = 2, Price = 1.0 },
		new Sale { ProductName = "Gum", Quantity = 1, Price = 0.5 }
	};

	public SalesPage()
	{
		InitializeComponent();
		billDataGrid.ItemsSource = sales;
	}
}

public class Sale
{
	public string ProductName { get; set; }
	public int Quantity { get; set; }
	public double Price { get; set; }
	public double Total => Quantity * Price;
}