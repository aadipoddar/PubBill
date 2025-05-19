using System.Windows;

namespace PubBill.Inventory;

/// <summary>
/// Interaction logic for InventoryDashboard.xaml
/// </summary>
public partial class InventoryDashboard : Window
{
	private readonly LoginWindow _loginWindow;

	public InventoryDashboard(LoginWindow loginWindow)
	{
		InitializeComponent();
		_loginWindow = loginWindow;

		_loginWindow.Hide();
	}

	private void recipeButton_Click(object sender, RoutedEventArgs e)
	{
		RecipeWindow recipeWindow = new();
		recipeWindow.ShowDialog();
	}

	private void purchaseButton_Click(object sender, RoutedEventArgs e)
	{

	}

	private void supplierButton_Click(object sender, RoutedEventArgs e)
	{
		SupplierWindow supplierWindow = new();
		supplierWindow.ShowDialog();
	}

	private void Window_Closed(object sender, EventArgs e) =>
		_loginWindow.Show();
}
