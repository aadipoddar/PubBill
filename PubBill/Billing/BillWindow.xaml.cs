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
	private readonly DiningTableModel _diningTableModel;
	private readonly DiningAreaModel _diningAreaModel;

	private static readonly ObservableCollection<CartModel> _cart = [];

	public BillWindow(UserModel user, LoginWindow loginWindow, TableDashboard tableDashboard, DiningTableModel diningTableModel, DiningAreaModel diningAreaModel)
	{
		InitializeComponent();

		_cart.Clear();
		cartDataGrid.ItemsSource = _cart;
		_user = user;
		_loginWindow = loginWindow;
		_tableDashboard = tableDashboard;
		_diningTableModel = diningTableModel;
		_diningAreaModel = diningAreaModel;

		RefreshTotal();
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		diningAreaTextBox.Text = _diningAreaModel.Name;
		diningTableTextBox.Text = _diningTableModel.Name;

		paymentModeComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<PaymentModeModel>(TableNames.PaymentMode);
		paymentModeComboBox.DisplayMemberPath = nameof(PaymentModeModel.Name);
		paymentModeComboBox.SelectedValuePath = nameof(PaymentModeModel.Id);
		paymentModeComboBox.SelectedIndex = 0;

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

			button.Click += (sender, e) =>
			{
				if (cartDataGrid.ItemsSource is ObservableCollection<CartModel> cart)
				{
					var existingCart = cart.FirstOrDefault(c => c.ProductId == product.Id);
					if (existingCart is not null) existingCart.Quantity++;
					else cart.Add(new CartModel
					{
						ProductId = product.Id,
						ProductName = product.Name,
						Quantity = 1,
						Rate = product.Rate,
						Instruction = string.Empty
					});
				}
				RefreshTotal();
			};

			itemsContol.Items.Add(button);
		}
	}

	#endregion

	private async void personNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		var foundPerson = await PersonData.LoadPersonByNumber(personNumberTextBox.Text);

		if (foundPerson is not null)
		{
			personNameTextBox.Text = foundPerson.Name;
			personNameTextBox.IsReadOnly = true;
			loyaltyCheckBox.IsChecked = foundPerson.Loyalty;
		}
		else
		{
			personNameTextBox.Clear();
			personNameTextBox.IsReadOnly = false;
			loyaltyCheckBox.IsChecked = false;
		}
	}

	private void RefreshTotal()
	{
		cartDataGrid.Items.Refresh();

		decimal total = 0;
		foreach (CartModel cart in _cart)
			total += cart.Total;

		foreach (var column in cartDataGrid.Columns)
		{
			if (column is DataGridTextColumn textColumn)
			{
				textColumn.ElementStyle = new Style(typeof(TextBlock))
				{
					Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right) }
				};
			}
		}

		totalAmountTextBox.Text = total.ToString();
	}

	#region DataGridEvents

	private void cartDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (cartDataGrid.SelectedItem is CartModel selectedSale)
		{
			quantityTextBox.Text = selectedSale.Quantity.ToString();
			instructionsTextBox.Text = selectedSale.Instruction;
			instructionsTextBox.Focus();
		}
	}

	private void instructionsTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (cartDataGrid is null) return;

		if (cartDataGrid.SelectedItem is CartModel selectedSale)
		{
			selectedSale.Instruction = instructionsTextBox.Text;
			RefreshTotal();
		}
		else instructionsTextBox.Clear();
	}

	private void quantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (cartDataGrid is null) return;

		if (cartDataGrid.SelectedItem is CartModel selectedSale)
		{
			selectedSale.Quantity = int.Parse(quantityTextBox.Text);
			RefreshTotal();
		}
		else quantityTextBox.Text = "0";
	}

	private void quantityMinusButton_Click(object sender, RoutedEventArgs e) => quantityTextBox.Text = (int.Parse(quantityTextBox.Text) - 1).ToString();

	private void quantityPlusButton_Click(object sender, RoutedEventArgs e) => quantityTextBox.Text = (int.Parse(quantityTextBox.Text) + 1).ToString();

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
		e.Handled = !regex.IsMatch((sender as TextBox).Text + e.Text);
	}

	private bool ValidateFields()
	{
		if (string.IsNullOrEmpty(personNumberTextBox.Text))
		{
			MessageBox.Show("Please enter a valid number.", "Invalid Number", MessageBoxButton.OK, MessageBoxImage.Warning);
			personNumberTextBox.Focus();
			return false;
		}

		if (string.IsNullOrEmpty(personNameTextBox.Text))
		{
			MessageBox.Show("Please enter a valid name.", "Invalid Name", MessageBoxButton.OK, MessageBoxImage.Warning);
			personNameTextBox.Focus();
			return false;
		}

		if (cartDataGrid.Items.Count == 0)
		{
			MessageBox.Show("Please add at least one product to the cart.", "Empty Cart", MessageBoxButton.OK, MessageBoxImage.Warning);
			return false;
		}
		return true;
	}

	#endregion

	#region Saving

	private async void billButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateFields()) return;

		int personId = await InsertPerson();
		if (personId == 0)
		{
			MessageBox.Show("Failed to insert person data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		int billId = await InsertBill(personId);
		if (billId == 0)
		{
			MessageBox.Show("Failed to insert bill data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		await InsertBillDetails(billId);
		Close();
	}

	private async Task<int> InsertPerson()
	{
		PersonModel person = new()
		{
			Id = 0,
			Name = personNameTextBox.Text,
			Number = personNumberTextBox.Text,
			Loyalty = (bool)loyaltyCheckBox.IsChecked
		};

		if (personNameTextBox.IsReadOnly)
			person.Id = (await PersonData.LoadPersonByNumber(person.Number)).Id;

		return await PersonData.InsertPerson(person);
	}

	private async Task<int> InsertBill(int personId)
	{
		BillModel bill = new()
		{
			Id = 0,
			UserId = _user.Id,
			LocationId = _user.LocationId,
			DiningAreaId = _diningAreaModel.Id,
			DiningTableId = _diningTableModel.Id,
			PersonId = personId,
			TotalPeople = int.Parse(totalPeopleTextBox.Text),
			AdjAmount = decimal.Parse(adjAmountTextBox.Text),
			AdjReason = adjReasonTextBox.Text,
			Remarks = remarkTextBox.Text,
			Total = decimal.Parse(totalAmountTextBox.Text),
			PaymentModeId = (int)paymentModeComboBox.SelectedValue,
			BillDateTime = DateTime.Now
		};
		if (string.IsNullOrEmpty(bill.AdjReason)) bill.AdjReason = string.Empty;
		if (string.IsNullOrEmpty(bill.Remarks)) bill.Remarks = string.Empty;
		if (string.IsNullOrEmpty(bill.AdjAmount.ToString())) bill.AdjAmount = 0;
		if (string.IsNullOrEmpty(bill.TotalPeople.ToString())) bill.TotalPeople = 0;

		return await BillData.InsertBill(bill);
	}

	private static async Task InsertBillDetails(int billId)
	{
		foreach (CartModel cart in _cart)
			await BillData.InsertBillDetail(new BillDetailModel
			{
				Id = 0,
				BillId = billId,
				ProductId = cart.ProductId,
				Quantity = cart.Quantity,
				Rate = cart.Rate,
				Instruction = cart.Instruction
			});
	}

	#endregion

	private void Window_Closed(object sender, EventArgs e) => _tableDashboard.Show();
}