using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

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

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		await LoadProductGroupComboBox();
	}

	#region LoadProducts

	private async Task LoadProductGroupComboBox()
	{
		groupComboBox.ItemsSource = await CommonData.LoadTableData<ProductGroupModel>(TableNames.ProductGroup);
		groupComboBox.DisplayMemberPath = nameof(ProductGroupModel.Name);
		groupComboBox.SelectedValuePath = nameof(ProductGroupModel.Id);
		groupComboBox.SelectedIndex = 0;
	}

	private async void groupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (groupComboBox.SelectedValue is int selectedGroupId)
		{
			categoryListBox.ItemsSource = await ProductCategoryData.LoadProductCategoryByProductGroup(selectedGroupId);
			categoryListBox.DisplayMemberPath = nameof(ProductCategoryModel.Name);
			categoryListBox.SelectedValuePath = nameof(ProductCategoryModel.Id);
			categoryListBox.SelectedIndex = 0;
		}
	}

	private async void categoryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (categoryListBox.SelectedValue is int)
		{
			searchProductNameTextBox.Clear();
			await CreateProductButtons();
		}
	}

	private async void searchProductNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (!string.IsNullOrEmpty(searchProductNameTextBox.Text))
		{
			categoryListBox.SelectedValue = null;
			await CreateProductButtons();
		}
	}

	private async Task CreateProductButtons()
	{
		itemsContol.Items.Clear();

		List<ProductModel> products = [];

		if (string.IsNullOrEmpty(searchProductNameTextBox.Text))
			products = await ProductData.LoadProductByProductCategory((int)categoryListBox.SelectedValue);
		else
		{
			products = await CommonData.LoadTableDataByStatus<ProductModel>(TableNames.Product);
			products = [.. products.Where(p => p.Name.Contains(searchProductNameTextBox.Text, StringComparison.CurrentCultureIgnoreCase))];
		}

		foreach (var product in products)
		{
			var button = CreateComponents.BuildProductButton(product);
			itemsContol.Items.Add(button);
		}
	}

	#endregion

	private void RefreshTotal()
	{
		billDataGrid.Items.Refresh();

		double total = 0;
		foreach (BillModel sale in _bill)
			total += sale.Total;

		totalAmountTextBox.Text = total.ToString();
	}

	#region Validation

	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
	{
		Regex regex = new("[^0-9]+");
		e.Handled = regex.IsMatch(e.Text);
	}

	private void decimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
	{
		Regex regex = new(@"^\d*\.?\d{0,2}$");
		e.Handled = !regex.IsMatch((sender as TextBox).Text + e.Text);
	}

	#endregion

	#region DataGridEvents

	private void billDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (billDataGrid.SelectedItem is BillModel selectedSale)
		{
			quantityTextBox.Text = selectedSale.Quantity.ToString();
			instructionsTextBox.Text = selectedSale.Instructions;
			instructionsTextBox.Focus();
		}
	}

	private void instructionsTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (billDataGrid is null) return;

		if (billDataGrid.SelectedItem is BillModel selectedSale)
		{
			selectedSale.Instructions = instructionsTextBox.Text;
			RefreshTotal();
		}
		else instructionsTextBox.Clear();
	}

	private void quantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
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