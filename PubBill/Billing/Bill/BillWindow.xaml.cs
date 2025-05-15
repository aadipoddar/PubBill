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
	private UserModel _user;
	private DiningTableModel _diningTable;
	private DiningAreaModel _diningArea;
	private readonly TableDashboard _tableDashboard;
	private readonly RunningBillModel _runningBillModel;

	private static readonly ObservableCollection<CartModel> _allCart = [], _kotCart = [];
	#endregion

	#region Constructors
	public BillWindow(UserModel user, TableDashboard tableDashboard, DiningTableModel diningTableModel)
	{
		InitializeComponent();

		_allCart.Clear();
		_kotCart.Clear();
		cartDataGrid.ItemsSource = _allCart;
		kotCartDataGrid.ItemsSource = _kotCart;

		_user = user;
		_diningTable = diningTableModel;
		_tableDashboard = tableDashboard;
	}

	public BillWindow(TableDashboard tableDashboard, RunningBillModel runningBillModel)
	{
		InitializeComponent();

		_allCart.Clear();
		_kotCart.Clear();
		cartDataGrid.ItemsSource = _allCart;
		kotCartDataGrid.ItemsSource = _kotCart;

		_tableDashboard = tableDashboard;
		_runningBillModel = runningBillModel;
	}
	#endregion

	#region Initialization
	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		await InitializeTextFields();
		await LoadProductGroupComboBox();
		await LoadComponentsFromRunningBill();
		await RefreshTotal();
	}

	private async Task InitializeTextFields()
	{
		if (_runningBillModel is not null)
		{
			_diningTable = await CommonData.LoadTableDataById<DiningTableModel>(TableNames.DiningTable, _runningBillModel.DiningTableId);
			_user = await CommonData.LoadTableDataById<UserModel>(TableNames.User, _runningBillModel.UserId);
		}

		_diningArea = await CommonData.LoadTableDataById<DiningAreaModel>(TableNames.DiningArea, _diningTable.DiningAreaId);
		diningAreaTextBox.Text = _diningArea.Name;
		diningTableTextBox.Text = _diningTable.Name;
		runningTimeTextBox.Text = "0";

		// Apply right alignment to all text columns
		foreach (var column in cartDataGrid.Columns)
			if (column is DataGridTextColumn textColumn)
				textColumn.ElementStyle = new Style(typeof(TextBlock))
				{
					Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right) }
				};
	}

	private async Task LoadComponentsFromRunningBill()
	{
		if (_runningBillModel is null) return;

		UpdateRunningTime();
		await LoadPersonData();
		UpdateAdjustmentFields();
		await LoadCartItems();
	}

	private void UpdateRunningTime()
	{
		var runningTime = DateTime.Now - _runningBillModel.BillStartDateTime;
		runningTimeTextBox.Text = runningTime.ToString("hh\\:mm");
	}

	private async Task LoadPersonData()
	{
		totalPeopleTextBox.Text = _runningBillModel.TotalPeople.ToString();

		if (_runningBillModel.PersonId is null) return;

		var person = await CommonData.LoadTableDataById<PersonModel>(TableNames.Person, (int)_runningBillModel.PersonId);
		if (person is not null)
		{
			personNameTextBox.Text = person.Name;
			personNumberTextBox.Text = person.Number;
			loyaltyCheckBox.IsChecked = person.Loyalty;
		}
	}

	private void UpdateAdjustmentFields()
	{
		discountPercentTextBox.Text = _runningBillModel.DiscPercent.ToString();
		discountReasonTextBox.Text = _runningBillModel.DiscReason;
		servicePercentTextBox.Text = _runningBillModel.ServicePercent.ToString();
		remarkTextBox.Text = _runningBillModel.Remarks;
	}

	private async Task LoadCartItems()
	{
		_allCart.Clear();

		var runningBillDetails = await RunningBillData.LoadRunningBillDetailByRunningBillId(_runningBillModel.Id);
		foreach (var item in runningBillDetails)
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
			button.Click += async (sender, e) => await AddProductToKotCart(product);
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
			await AddProductToKotCart(foundProduct);
			searchCodeTextBox.Clear();
		}
	}
	#endregion

	#region Person Management
	private async void personNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		var foundPersonTransaction = await PubEntryData.LaodTransactionByLocationPerson(_diningArea.LocationId, personNumberTextBox.Text);

		if (foundPersonTransaction is not null)
		{
			var totalAdvance = 0;
			var advance = await PubEntryData.LoadAdvanceByTransactionId(foundPersonTransaction.Id);

			if (advance is not null)
			{
				var advanceDetails = await PubEntryData.LoadAdvanceDetailByAdvanceId(advance.Id);
				totalAdvance = (int)advanceDetails.Sum(x => x?.Amount);
			}

			personNameTextBox.Text = foundPersonTransaction.PersonName;
			personNameTextBox.IsReadOnly = true;
			loyaltyCheckBox.IsChecked = foundPersonTransaction.PersonLoyalty;
			totalPeopleTextBox.Text = (foundPersonTransaction.Male + foundPersonTransaction.Female).ToString();
			entryPaidTextBox.Text = (foundPersonTransaction.Cash + foundPersonTransaction.Card + foundPersonTransaction.UPI + foundPersonTransaction.Amex + totalAdvance).ToString();
		}

		else
		{
			totalPeopleTextBox.Text = "1";
			entryPaidTextBox.Text = "0";

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

		await RefreshTotal();
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

		var foundPerson = await PersonData.LoadPersonByNumber(personNumberTextBox.Text);
		if (foundPerson is not null)
			person.Id = foundPerson.Id;

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
		if (selectedSale is null) return;

		quantityTextBox.Text = selectedSale.Quantity.ToString();
		instructionsTextBox.Text = selectedSale.Instruction;
		cancelledCheckBox.IsChecked = selectedSale.Cancelled;
		instructionsTextBox.Focus();
	}

	private async void instructionsTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (instructionsTextBox is null || allCartTabItem is null || kotCartTabItem is null) return;

		if (cartTabControl.SelectedIndex == 0 && cartDataGrid?.SelectedItem is CartModel sale1)
			sale1.Instruction = instructionsTextBox.Text;
		else if (cartTabControl.SelectedIndex == 1 && kotCartDataGrid?.SelectedItem is CartModel sale2)
			sale2.Instruction = instructionsTextBox.Text;

		else instructionsTextBox.Clear();

		await RefreshTotal();
	}

	private async void quantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (quantityTextBox is null || allCartTabItem is null || kotCartTabItem is null) return;

		if (cartTabControl.SelectedIndex == 0 && cartDataGrid?.SelectedItem is CartModel sale)
		{
			int newQuantity = int.Parse(quantityTextBox.Text);
			int oldQuantity = sale.Quantity;
			int quantityChange = newQuantity - oldQuantity;

			if (newQuantity > oldQuantity)
				await AddProductToKotCart(new CartModel
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

		await RefreshTotal();
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

	private async void cancelledCheckBox_Checked(object sender, RoutedEventArgs e)
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
			await AddProductToKotCart(kotSale);
		}

		await RefreshTotal();
	}

	private async void cancelledCheckBox_Unchecked(object sender, RoutedEventArgs e)
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
			await AddProductToKotCart(kotSale);
		}

		await RefreshTotal();
	}
	#endregion

	#region Price Calculations
	private decimal _baseTotal = 0;
	private bool _isUpdating = false;

	private async Task RefreshTotal()
	{
		cartDataGrid.Items.Refresh();
		kotCartDataGrid.Items.Refresh();

		_baseTotal = BillWindowHelper.CalculateBaseTotal(_allCart, _kotCart);
		baseTotalAmountTextBox.Text = _baseTotal.FormatIndianCurrency();

		decimal discountPercent = decimal.Parse(discountPercentTextBox.Text);
		decimal discountAmout = _baseTotal * (discountPercent / 100);

		discountAmountTextBox.Text = discountAmout.ToString("N2");
		afterDiscsTotalAmountTextBox.Text = (_baseTotal - discountAmout).FormatIndianCurrency();

		decimal productTax = await BillWindowHelper.CalculateProductTotalTax(_allCart, _kotCart, discountPercent);
		decimal subTotal = _baseTotal - discountAmout + productTax;
		subTotalAmountTextBox.Text = subTotal.FormatIndianCurrency();

		decimal servicePercent = decimal.Parse(servicePercentTextBox.Text);
		decimal serviceAmount = subTotal * (servicePercent / 100);

		serviceAmountTextBox.Text = serviceAmount.ToString("N2");

		decimal entryPaid = decimal.Parse(entryPaidTextBox.Text);
		totalAmountTextBox.Text = (subTotal + serviceAmount - entryPaid).FormatIndianCurrency();
	}

	private async void discountPercentTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (_isUpdating || discountPercentTextBox is null || discountAmountTextBox is null || totalAmountTextBox is null) return;

		_isUpdating = true;

		if (!decimal.TryParse(discountPercentTextBox.Text, out decimal percent))
		{
			discountPercentTextBox.Text = "0.00";
			percent = 0;
		}

		percent = Math.Clamp(percent, 0, 100);
		if (percent != decimal.Parse(discountPercentTextBox.Text))
			discountPercentTextBox.Text = percent.ToString("N2");

		await RefreshTotal();

		_isUpdating = false;
	}

	private async void discountAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (_isUpdating || discountAmountTextBox is null || discountPercentTextBox is null || totalAmountTextBox is null) return;

		_isUpdating = true;

		if (!decimal.TryParse(discountAmountTextBox.Text, out decimal amount))
		{
			discountAmountTextBox.Text = "0.00";
			amount = 0;
		}

		amount = Math.Clamp(amount, 0, _baseTotal);
		if (amount != decimal.Parse(discountAmountTextBox.Text))
			discountAmountTextBox.Text = amount.ToString("N2");

		decimal discountPercent = _baseTotal != 0 ? (amount / _baseTotal) * 100 : 0;
		discountPercentTextBox.Text = discountPercent.ToString("N2");

		await RefreshTotal();

		_isUpdating = false;
	}

	private async void servicePercentTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (_isUpdating || servicePercentTextBox is null || serviceAmountTextBox is null || totalAmountTextBox is null) return;

		_isUpdating = true;

		if (!decimal.TryParse(servicePercentTextBox.Text, out decimal percent))
		{
			servicePercentTextBox.Text = "0.00";
			percent = 0;
		}

		percent = Math.Clamp(percent, 0, 100);
		if (percent != decimal.Parse(servicePercentTextBox.Text))
			servicePercentTextBox.Text = percent.ToString("N2");

		await RefreshTotal();

		_isUpdating = false;
	}

	private async void serviceAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (_isUpdating || servicePercentTextBox is null || serviceAmountTextBox is null || totalAmountTextBox is null) return;

		_isUpdating = true;

		if (!decimal.TryParse(serviceAmountTextBox.Text, out decimal amount))
		{
			serviceAmountTextBox.Text = "0.00";
			amount = 0;
		}

		amount = Math.Clamp(amount, 0, _baseTotal);
		if (amount != decimal.Parse(serviceAmountTextBox.Text))
			serviceAmountTextBox.Text = amount.ToString("N2");

		decimal subTotal = _baseTotal - decimal.Parse(discountAmountTextBox.Text);
		decimal servicePercent = subTotal != 0 ? (amount / subTotal) * 100 : 0;

		servicePercentTextBox.Text = servicePercent.ToString("N2");

		await RefreshTotal();

		_isUpdating = false;
	}
	#endregion

	#region Validation
	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private void decimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateDecimalInput(sender, e);
	#endregion

	#region KOT Processing
	private async Task AddProductToKotCart(ProductModel product)
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
		await RefreshTotal();
	}

	private async Task AddProductToKotCart(CartModel cart)
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
		await RefreshTotal();
	}

	private async void kotButton_Click(object sender, RoutedEventArgs e)
	{
		if (_kotCart.Count == 0)
		{
			MessageBox.Show("Cart is Empty. Add Items to Cart.", "Empty Cart", MessageBoxButton.OK, MessageBoxImage.Warning);
			return;
		}

		int? personId = null;
		if (!string.IsNullOrEmpty(personNumberTextBox.Text))
			personId = await InsertPerson();

		int runningBillId = await InsertRunningBill(personId);
		if (runningBillId == 0)
		{
			MessageBox.Show("Failed to insert running bill data.", "Missing Bill Id", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		await InsertKOTBillDetails(runningBillId);
		await InsertRunningBillDetails(runningBillId);
		Close();
	}

	private async Task<int> InsertRunningBill(int? personId)
	{
		RunningBillModel runningBill = new()
		{
			Id = _runningBillModel?.Id ?? 0,
			UserId = _runningBillModel?.UserId ?? _user.Id,
			LocationId = _diningArea.LocationId,
			DiningAreaId = _diningArea.Id,
			DiningTableId = _diningTable.Id,
			PersonId = personId,
			TotalPeople = int.TryParse(totalPeopleTextBox.Text, out int people) ? people : 0,
			DiscPercent = decimal.Parse(discountPercentTextBox.Text),
			DiscReason = discountReasonTextBox.Text,
			ServicePercent = decimal.Parse(servicePercentTextBox.Text),
			Remarks = remarkTextBox.Text,
			BillStartDateTime = _runningBillModel?.BillStartDateTime ?? DateTime.Now,
			BillId = _runningBillModel?.BillId ?? null,
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
			var allCart = _allCart.FirstOrDefault(c => c.ProductId == kotCart.ProductId && c.Cancelled == kotCart.Cancelled && c.Instruction == kotCart.Instruction);
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
		if (_runningBillModel is null)
		{
			MessageBox.Show("Cart is Empty. Add Items to Cart.", "Empty Cart", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		if (_kotCart.Count > 0)
		{
			MessageBox.Show("KOT Cart not Empty. Push Items to KOT.", "Push KOT", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		if (await BillWindowHelper.CheckKOT(_runningBillModel))
		{
			MessageBox.Show("KOT Items not yet Printed.", "Print KOT", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		int? personId = null;
		if (!string.IsNullOrEmpty(personNumberTextBox.Text))
			personId = await InsertPerson();

		NavigateToPaymentWindow(personId);
	}

	private void NavigateToPaymentWindow(int? personId)
	{
		BillModel billModel = new()
		{
			Id = _runningBillModel?.BillId ?? 0,
			UserId = _runningBillModel?.UserId ?? _user.Id,
			LocationId = _diningArea.LocationId,
			DiningAreaId = _diningArea.Id,
			DiningTableId = _diningTable.Id,
			PersonId = personId,
			TotalPeople = int.TryParse(totalPeopleTextBox.Text, out int people) ? people : 0,
			DiscPercent = decimal.Parse(discountPercentTextBox.Text),
			DiscReason = discountReasonTextBox.Text,
			ServicePercent = decimal.Parse(servicePercentTextBox.Text),
			EntryPaid = int.Parse(entryPaidTextBox.Text),
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
			DiscPercent = decimal.Parse(discountPercentTextBox.Text),
			DiscReason = discountReasonTextBox.Text,
			ServicePercent = decimal.Parse(servicePercentTextBox.Text),
			Remarks = _runningBillModel.Remarks,
			BillStartDateTime = DateTime.Now,
			BillId = _runningBillModel.BillId,
			Status = false
		};

		BillPaymentWindow billPaymentWindow = new(this, billModel, runningBillModel, _allCart);
		billPaymentWindow.ShowDialog();
	}
	#endregion

	private async void Window_Closed(object sender, EventArgs e)
	{
		await _tableDashboard.RefreshScreen();
		_tableDashboard.Show();
	}
}
