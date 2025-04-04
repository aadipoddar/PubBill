using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace PubBill.Billing;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class BillWindow : Window
{
	private readonly UserModel _user;
	private readonly LoginWindow _loginWindow;
	private readonly TableDashboard _tableDashboard;

	private static readonly ObservableCollection<BillModel> _bill =
	[
		new BillModel { ProductName = "Chips", Quantity = 3, Price = 1.5, Instructions = "" },
		new BillModel { ProductName = "Peanuts", Quantity = 1, Price = 1.0, Instructions = "" },
		new BillModel { ProductName = "Pretzels", Quantity = 2, Price = 1.5, Instructions = "" },
		new BillModel { ProductName = "Soda", Quantity = 2, Price = 1.5, Instructions = "" },
		new BillModel { ProductName = "Water", Quantity = 1, Price = 1.0, Instructions = "" },
		new BillModel { ProductName = "Juice", Quantity = 1, Price = 2.0, Instructions = "" },
		new BillModel { ProductName = "Candy", Quantity = 2, Price = 1.0, Instructions = "" },
		new BillModel { ProductName = "Gum", Quantity = 1, Price = 0.5, Instructions = "" }
	];

	public BillWindow(UserModel user, LoginWindow loginWindow, TableDashboard tableDashboard)
	{
		InitializeComponent();
		billDataGrid.ItemsSource = _bill;
		_user = user;
		_loginWindow = loginWindow;
		_tableDashboard = tableDashboard;
		RefreshTotal();
	}

	#region LoadData

	private void RefreshTotal()
	{
		billDataGrid.Items.Refresh();

		double total = 0;
		foreach (BillModel sale in _bill)
			total += sale.Total;

		totalAmountTextBox.Text = total.ToString();
	}

	#endregion

	#region Validation

	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
	{
		Regex regex = new("[^0-9]+");
		e.Handled = regex.IsMatch(e.Text);
	}

	private void decimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
	{
		Regex regex = new(@"^\d*\.?\d{0,2}$");
		e.Handled = !regex.IsMatch((sender as System.Windows.Controls.TextBox).Text + e.Text);
	}

	#endregion

	#region DataGridEvents

	private void billDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
	{
		if (billDataGrid.SelectedItem is BillModel selectedSale)
		{
			quantityTextBox.Text = selectedSale.Quantity.ToString();
			instructionsTextBox.Text = selectedSale.Instructions;
			instructionsTextBox.Focus();
		}
	}

	private void instructionsTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
	{
		if (billDataGrid is null) return;

		if (billDataGrid.SelectedItem is BillModel selectedSale)
		{
			selectedSale.Instructions = instructionsTextBox.Text;
			RefreshTotal();
		}
		else instructionsTextBox.Clear();
	}

	private void quantityTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
	{
		if (billDataGrid is null) return;

		if (billDataGrid.SelectedItem is BillModel selectedSale)
		{
			selectedSale.Quantity = double.Parse(quantityTextBox.Text);
			RefreshTotal();
		}
		else quantityTextBox.Text = "0";
	}

	private void quantityMinusButton_Click(object sender, RoutedEventArgs e) => quantityTextBox.Text = (int.Parse(quantityTextBox.Text) - 1).ToString();

	private void quantityPlusButton_Click(object sender, RoutedEventArgs e) => quantityTextBox.Text = (int.Parse(quantityTextBox.Text) + 1).ToString();

	#endregion

	private void Window_Closed(object sender, EventArgs e) => _tableDashboard.Show();
}