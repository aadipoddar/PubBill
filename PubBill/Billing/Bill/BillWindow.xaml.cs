using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PubBill.Billing.Bill;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class BillWindow : Window
{
	#region Fields
	private readonly UserModel _user;
	private readonly TableDashboard _tableDashboard;
	private readonly DiningTableModel _diningTableModel;
	private readonly DiningAreaModel _diningAreaModel;
	private readonly RunningBillModel _runningBillModel;

	private static readonly ObservableCollection<CartModel> _allCart = [];
	private static readonly ObservableCollection<CartModel> _kotCart = [];
	#endregion

	#region Constructors
	public BillWindow(UserModel user, TableDashboard tableDashboard, DiningTableModel diningTableModel, DiningAreaModel diningAreaModel)
		: this(user, tableDashboard, diningTableModel, diningAreaModel, null)
	{
	}

	public BillWindow(UserModel user, TableDashboard tableDashboard, DiningTableModel diningTableModel, DiningAreaModel diningAreaModel, RunningBillModel runningBillModel)
	{
		InitializeComponent();

		_allCart.Clear();
		_kotCart.Clear();
		cartDataGrid.ItemsSource = _allCart;
		kotCartDataGrid.ItemsSource = _kotCart;
		_user = user;
		_tableDashboard = tableDashboard;
		_diningTableModel = diningTableModel;
		_diningAreaModel = diningAreaModel;
		_runningBillModel = runningBillModel;

		RefreshTotal();
	}
	#endregion

	#region Initialization
	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		InitializeTextFields();
		await LoadProductGroupComboBox();
		await LoadComponentsFromRunningBill();
	}

	private void InitializeTextFields()
	{
		diningAreaTextBox.Text = _diningAreaModel.Name;
		diningTableTextBox.Text = _diningTableModel.Name;
		runningTimeTextBox.Text = "0";
	}

	private async Task LoadComponentsFromRunningBill()
	{
		if (_runningBillModel is null) return;

		UpdateRunningTime();
		await LoadPersonData();
		await UpdateAdjustmentFields();
		await LoadCartItems();

		RefreshTotal();
	}

	private void UpdateRunningTime()
	{
		var runningTime = DateTime.Now - _runningBillModel.BillStartDateTime;
		runningTimeTextBox.Text = runningTime.ToString("hh\\:mm");
	}

	private async Task LoadPersonData()
	{
		var person = await CommonData.LoadTableDataById<PersonModel>(TableNames.Person, _runningBillModel.PersonId);
		if (person is not null)
		{
			personNameTextBox.Text = person.Name;
			personNumberTextBox.Text = person.Number;
			loyaltyCheckBox.IsChecked = person.Loyalty;
		}
	}

	private async Task UpdateAdjustmentFields()
	{
		var runningBillDetails = await RunningBillData.LoadRunningBillDetailByRunningBillId(_runningBillModel.Id);
		var adjAmount = _runningBillModel.AdjAmount;
		var total = runningBillDetails.Where(item => !item.Cancelled).Sum(item => item.Quantity * item.Rate);
		var percent = adjAmount / total * 100;
		total -= adjAmount;

		totalPeopleTextBox.Text = _runningBillModel.TotalPeople.ToString();
		adjAmountTextBox.Text = adjAmount.ToString();
		adjPercentTextBox.Text = percent.ToString();
		adjReasonTextBox.Text = _runningBillModel.AdjReason;
		remarkTextBox.Text = _runningBillModel.Remarks;
		totalAmountTextBox.Text = total.ToString();
	}

	private async Task LoadCartItems()
	{
		_allCart.Clear();

		var runningTableDetails = await RunningBillData.LoadRunningBillDetailByRunningBillId(_runningBillModel.Id);
		foreach (var item in runningTableDetails)
		{
			var product = await CommonData.LoadTableDataById<ProductModel>(TableNames.Product, item.ProductId);
			if (product is not null)
			{
				_allCart.Add(new CartModel
				{
					ProductId = product.Id,
					ProductName = product.Name,
					Quantity = item.Quantity,
					Rate = item.Rate,
					Instruction = item.Instruction,
					Cancelled = item.Cancelled
				});
			}
		}
	}
	#endregion

	#region Product Loading
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
			button.Click += (sender, e) => AddProductToKotCart(product);
			itemsContol.Items.Add(button);
		}
	}

	private async void searchCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (string.IsNullOrEmpty(searchCodeTextBox.Text)) return;

		var products = await CommonData.LoadTableDataByStatus<ProductModel>(TableNames.Product);
		var foundProduct = products.FirstOrDefault(p => p.Code == searchCodeTextBox.Text);

		if (foundProduct is not null)
		{
			AddProductToKotCart(foundProduct);
			searchCodeTextBox.Clear();
		}
	}

	private void AddProductToKotCart(ProductModel product)
	{
		var existingProduct = _kotCart.FirstOrDefault(c => c.ProductId == product.Id && c.Cancelled == false);
		if (existingProduct is not null)
			existingProduct.Quantity++;
		else
			_kotCart.Add(new CartModel
			{
				ProductId = product.Id,
				ProductName = product.Name,
				Quantity = 1,
				Rate = product.Rate,
				Instruction = string.Empty
			});
		RefreshTotal();
	}
	#endregion

	#region Person Management
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
	#endregion

	#region DataGrid Events
	private void cartDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
		UpdateSelectionFields(cartDataGrid.SelectedItem as CartModel);

	private void kotCartDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
		UpdateSelectionFields(kotCartDataGrid.SelectedItem as CartModel);

	private void UpdateSelectionFields(CartModel selectedSale)
	{
		if (selectedSale != null)
		{
			quantityTextBox.Text = selectedSale.Quantity.ToString();
			instructionsTextBox.Text = selectedSale.Instruction;
			cancelledCheckBox.IsChecked = selectedSale.Cancelled;
			instructionsTextBox.Focus();
		}
	}

	private void instructionsTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (instructionsTextBox is null || allCartTabItem is null || kotCartTabItem is null) return;

		CartModel selectedSale = null;

		if (cartTabControl.SelectedIndex == 0 && cartDataGrid?.SelectedItem is CartModel sale1)
			selectedSale = sale1;
		else if (cartTabControl.SelectedIndex == 1 && kotCartDataGrid?.SelectedItem is CartModel sale2)
			selectedSale = sale2;

		if (selectedSale != null)
			selectedSale.Instruction = instructionsTextBox.Text;
		else
			instructionsTextBox.Clear();

		RefreshTotal();
	}

	private void AddProductToKotCart(CartModel cart)
	{
		var existingProduct = _kotCart.FirstOrDefault(c => c.ProductId == cart.ProductId && c.Cancelled == cart.Cancelled);
		if (existingProduct is not null)
			existingProduct.Quantity += cart.Quantity;
		else
			_kotCart.Add(new CartModel
			{
				ProductId = cart.ProductId,
				ProductName = cart.ProductName,
				Quantity = cart.Quantity,
				Rate = cart.Rate,
				Instruction = cart.Instruction,
				Cancelled = cart.Cancelled
			});
		RefreshTotal();
	}

	private void quantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (quantityTextBox is null || allCartTabItem is null || kotCartTabItem is null) return;

		if (cartTabControl.SelectedIndex == 0 && cartDataGrid?.SelectedItem is CartModel sale)
		{
			int newQuantity = int.Parse(quantityTextBox.Text);
			int oldQuantity = sale.Quantity;
			int quantityChange = newQuantity - oldQuantity;

			if (newQuantity > oldQuantity)
				AddProductToKotCart(new CartModel
				{
					ProductId = sale.ProductId,
					ProductName = sale.ProductName,
					Quantity = quantityChange,
					Rate = sale.Rate,
					Instruction = sale.Instruction
				});

			else if (newQuantity < oldQuantity)
				_kotCart.Add(new CartModel
				{
					ProductId = sale.ProductId,
					ProductName = sale.ProductName,
					Quantity = quantityChange,
					Rate = sale.Rate,
					Instruction = sale.Instruction,
					Cancelled = true
				});

			if (newQuantity == 0)
				_allCart.Remove(sale);
		}

		else if (cartTabControl.SelectedIndex == 1 && kotCartDataGrid?.SelectedItem is CartModel kotSale)
		{
			int quantity = int.Parse(quantityTextBox.Text);
			if (quantity < 0) quantity = 0;

			if (quantity == 0) _kotCart.Remove(kotSale);
			else kotSale.Quantity = quantity;
		}

		else quantityTextBox.Text = "0";

		RefreshTotal();
	}

	private void quantityMinusButton_Click(object sender, RoutedEventArgs e) =>
		UpdateQuantity(-1);

	private void quantityPlusButton_Click(object sender, RoutedEventArgs e) =>
		UpdateQuantity(1);

	private void UpdateQuantity(int change)
	{
		if (int.TryParse(quantityTextBox.Text, out int currentQty))
			quantityTextBox.Text = Math.Max(0, currentQty + change).ToString();
	}

	private void cancelledCheckBox_Checked(object sender, RoutedEventArgs e)
	{
		if (cartTabControl.SelectedIndex == 0 && cartDataGrid?.SelectedItem is CartModel sale)
		{
			if (sale.Cancelled) return;
			sale.Cancelled = true;
			_allCart.Remove(sale);
			_kotCart.Add(sale);
		}

		else if (cartTabControl.SelectedIndex == 1 && kotCartDataGrid?.SelectedItem is CartModel kotSale)
		{
			if (kotSale.Cancelled) return;
			_kotCart.Remove(kotSale);
			kotSale.Cancelled = true;
			AddProductToKotCart(kotSale);
		}

		RefreshTotal();
	}

	private void cancelledCheckBox_Unchecked(object sender, RoutedEventArgs e)
	{
		if (cartTabControl.SelectedIndex == 0 && cartDataGrid?.SelectedItem is CartModel sale)
		{
			if (!sale.Cancelled) return;
			sale.Cancelled = false;
		}

		else if (cartTabControl.SelectedIndex == 1 && kotCartDataGrid?.SelectedItem is CartModel kotSale)
		{
			if (!kotSale.Cancelled) return;
			kotSale.Cancelled = false;
			_kotCart.Remove(kotSale);
			AddProductToKotCart(kotSale);
		}

		RefreshTotal();
	}
	#endregion

	#region Price Calculations
	private decimal _originalTotal;
	private bool _isUpdating = false;

	private void RefreshTotal()
	{
		cartDataGrid.Items.Refresh();
		kotCartDataGrid.Items.Refresh();

		_originalTotal = 0;
		_originalTotal += _allCart.Where(cart => !cart.Cancelled).Sum(cart => cart.Total);
		_originalTotal += _kotCart.Where(cart => !cart.Cancelled).Sum(cart => cart.Total);

		// Apply right alignment to all text columns
		foreach (var column in cartDataGrid.Columns)
			if (column is DataGridTextColumn textColumn)
				textColumn.ElementStyle = new Style(typeof(TextBlock))
				{
					Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right) }
				};

		RecalculateTotals();
	}

	private void RecalculateTotals()
	{
		decimal adjPercent = 0;
		if (decimal.TryParse(adjPercentTextBox.Text, out decimal parsedPercent))
		{
			adjPercent = Math.Clamp(parsedPercent, 0, 100);
			if (parsedPercent != adjPercent)
				adjPercentTextBox.Text = adjPercent.ToString("N2");
		}
		else
		{
			adjPercentTextBox.Text = "0.00";
		}

		decimal adjAmount = _originalTotal * (adjPercent / 100);
		decimal finalTotal = _originalTotal - adjAmount;

		adjAmountTextBox.Text = adjAmount.ToString("N2");
		totalAmountTextBox.Text = finalTotal.ToString("N2");
	}

	private void adjPercentTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (_isUpdating || adjPercentTextBox is null || adjAmountTextBox is null || totalAmountTextBox is null) return;

		_isUpdating = true;

		if (!decimal.TryParse(adjPercentTextBox.Text, out decimal percent))
		{
			adjPercentTextBox.Text = "0.00";
			percent = 0;
		}

		percent = Math.Clamp(percent, 0, 100);
		if (percent != decimal.Parse(adjPercentTextBox.Text))
			adjPercentTextBox.Text = percent.ToString("N2");

		decimal adjAmount = _originalTotal * (percent / 100);
		decimal finalTotal = _originalTotal - adjAmount;

		adjAmountTextBox.Text = adjAmount.ToString("N2");
		totalAmountTextBox.Text = finalTotal.ToString("N2");

		_isUpdating = false;
	}

	private void adjAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (_isUpdating || adjAmountTextBox is null || adjPercentTextBox is null || totalAmountTextBox is null) return;

		_isUpdating = true;

		if (!decimal.TryParse(adjAmountTextBox.Text, out decimal amount))
		{
			adjAmountTextBox.Text = "0.00";
			amount = 0;
		}

		amount = Math.Clamp(amount, 0, _originalTotal);
		if (amount != decimal.Parse(adjAmountTextBox.Text))
			adjAmountTextBox.Text = amount.ToString("N2");

		decimal adjPercent = _originalTotal != 0 ? (amount / _originalTotal) * 100 : 0;
		adjPercentTextBox.Text = adjPercent.ToString("N2");

		totalAmountTextBox.Text = (_originalTotal - amount).ToString("N2");

		_isUpdating = false;
	}
	#endregion

	#region Validation
	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		e.Handled = !int.TryParse(e.Text, out _);

	private void decimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		e.Handled = !decimal.TryParse(e.Text, out _);

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

		return true;
	}
	#endregion

	#region KOT Processing
	private async void kotButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateFields()) return;

		if (kotCartDataGrid.Items.Count == 0)
		{
			MessageBox.Show("Please add at least one product to the cart", "Empty Cart", MessageBoxButton.OK, MessageBoxImage.Warning);
			return;
		}

		int personId = await InsertPerson();
		if (personId == 0)
		{
			MessageBox.Show("Failed to insert person data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		int runningBillId = await InsertRunningBill(personId);
		if (runningBillId == 0)
		{
			MessageBox.Show("Failed to insert running bill data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		await InsertKOTBillDetails(runningBillId);
		await InsertRunningBillDetails(runningBillId);
		Close();
	}

	private async Task<int> InsertRunningBill(int personId)
	{
		RunningBillModel runningBill = new()
		{
			Id = _runningBillModel?.Id ?? 0,
			UserId = _user.Id,
			LocationId = _user.LocationId,
			DiningAreaId = _diningAreaModel.Id,
			DiningTableId = _diningTableModel.Id,
			PersonId = personId,
			TotalPeople = int.TryParse(totalPeopleTextBox.Text, out int people) ? people : 0,
			AdjAmount = decimal.TryParse(adjAmountTextBox.Text, out decimal adj) ? adj : 0,
			AdjReason = adjReasonTextBox.Text,
			Remarks = remarkTextBox.Text,
			BillStartDateTime = _runningBillModel?.BillStartDateTime ?? DateTime.Now,
			Status = true
		};

		return await RunningBillData.InsertRunningBill(runningBill);
	}

	private static async Task InsertKOTBillDetails(int runningBillId)
	{
		foreach (CartModel cart in _kotCart)
			await KOTData.InsertKOTBillDetail(new KOTBillDetailModel
			{
				Id = 0,
				RunningBillId = runningBillId,
				ProductId = cart.ProductId,
				Quantity = cart.Quantity,
				Instruction = cart.Instruction,
				Status = true,
				Cancelled = cart.Cancelled
			});
	}

	private async Task InsertRunningBillDetails(int runningBillId)
	{
		if (_runningBillModel is not null)
			await RunningBillData.DeleteRunningBillDetail(_runningBillModel.Id);

		MergeCartItems();

		foreach (CartModel cart in _allCart)
			await RunningBillData.InsertRunningBillDetail(new RunningBillDetailModel
			{
				Id = 0,
				RunningBillId = runningBillId,
				ProductId = cart.ProductId,
				Quantity = cart.Quantity,
				Rate = cart.Rate,
				Instruction = cart.Instruction,
				Cancelled = cart.Cancelled
			});
	}

	private static void MergeCartItems()
	{
		foreach (var kotCart in _kotCart)
		{
			var allCart = _allCart.FirstOrDefault(c => c.ProductId == kotCart.ProductId && c.Cancelled == kotCart.Cancelled);
			if (allCart != null)
			{
				allCart.Quantity += kotCart.Quantity;
				allCart.Rate = kotCart.Rate;
				allCart.Instruction = kotCart.Instruction;
				allCart.Cancelled = kotCart.Cancelled;
			}
			else
			{
				_allCart.Add(new CartModel
				{
					ProductId = kotCart.ProductId,
					ProductName = kotCart.ProductName,
					Quantity = kotCart.Quantity,
					Rate = kotCart.Rate,
					Instruction = kotCart.Instruction,
					Cancelled = kotCart.Cancelled
				});
			}
		}
	}
	#endregion

	#region Bill Processing
	private async void billButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateFields()) return;

		if (await CheckKOT())
		{
			MessageBox.Show("KOT Items not yet Printed.", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		int personId = await InsertPerson();
		if (personId == 0)
		{
			MessageBox.Show("Failed to insert person data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		if (_runningBillModel is null)
		{
			MessageBox.Show("Cart is Empty. Push Items to KOT.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		NavigateToPaymentWindow(personId);
	}

	private void NavigateToPaymentWindow(int personId)
	{
		BillModel billModel = new()
		{
			Id = 0,
			UserId = _user.Id,
			LocationId = _user.LocationId,
			DiningAreaId = _diningAreaModel.Id,
			DiningTableId = _diningTableModel.Id,
			PersonId = personId,
			TotalPeople = int.TryParse(totalPeopleTextBox.Text, out int people) ? people : 0,
			AdjAmount = decimal.TryParse(adjAmountTextBox.Text, out decimal adj) ? adj : 0,
			AdjReason = adjReasonTextBox.Text,
			Remarks = remarkTextBox.Text,
			BillDateTime = DateTime.Now
		};

		RunningBillModel runningBillModel = new()
		{
			Id = _runningBillModel.Id,
			UserId = _runningBillModel.UserId,
			LocationId = _runningBillModel.LocationId,
			DiningAreaId = _runningBillModel.DiningAreaId,
			DiningTableId = _runningBillModel.DiningTableId,
			PersonId = _runningBillModel.PersonId,
			TotalPeople = _runningBillModel.TotalPeople,
			AdjAmount = _runningBillModel.AdjAmount,
			AdjReason = _runningBillModel.AdjReason,
			Remarks = _runningBillModel.Remarks,
			BillStartDateTime = DateTime.Now,
			Status = false
		};

		BillPaymentWindow billPaymentWindow = new(this, billModel, runningBillModel, _allCart);
		billPaymentWindow.ShowDialog();
	}

	private async Task<bool> CheckKOT()
	{
		if (_runningBillModel is null) return false;
		var kotOrders = await KOTData.LoadKOTBillDetailByRunningBillId(_runningBillModel.Id);
		return kotOrders.Any(k => k.Status);
	}
	#endregion

	private async void Window_Closed(object sender, EventArgs e)
	{
		await _tableDashboard.RefreshScreen();
		_tableDashboard.Show();
	}
}
