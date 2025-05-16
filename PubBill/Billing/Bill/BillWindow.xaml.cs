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
		RefreshTotal();
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
				_allCart.Add(new CartModel
				{
					ProductId = product.Id,
					ProductName = product.Name,
					Quantity = item.Quantity,
					Rate = item.Rate,
					BaseTotal = item.BaseTotal,
					Instruction = item.Instruction,
					Discountable = item.Discountable,
					SelfDiscount = item.SelfDiscount,
					DiscPercent = item.DiscPercent,
					DiscAmount = item.DiscAmount,
					AfterDiscount = item.AfterDiscount,
					CGSTPercent = item.CGSTPercent,
					CGSTAmount = item.CGSTAmount,
					SGSTPercent = item.SGSTPercent,
					SGSTAmount = item.SGSTAmount,
					IGSTPercent = item.IGSTPercent,
					IGSTAmount = item.IGSTAmount,
					Total = item.Total,
					Cancelled = item.Cancelled
				});
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

		RefreshTotal();
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

	#region DataGrid Events
	private void cartDataGrid_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		if (e.OriginalSource is FrameworkElement element && element.DataContext is CartModel model)
		{
			if (cartTabControl.SelectedIndex == 0)
			{
				BillItemManageWindow detailWindow = new(model, decimal.Parse(discountPercentTextBox.Text), false);
				bool? result = detailWindow.ShowDialog();

				if (result == true && model.Quantity == 0)
					_allCart.Remove(model);
			}

			else if (cartTabControl.SelectedIndex == 1)
			{
				BillItemManageWindow detailWindow = new(model, decimal.Parse(discountPercentTextBox.Text));
				bool? result = detailWindow.ShowDialog();

				if (result == true && model.Quantity == 0)
					_kotCart.Remove(model);
			}

			RefreshTotal();
		}
	}
	#endregion

	#region Price Calculations
	private decimal _baseTotal = 0;
	private bool _isUpdating = false;

	private void RefreshTotal()
	{
		cartDataGrid.Items.Refresh();
		kotCartDataGrid.Items.Refresh();

		_baseTotal = BillWindowHelper.CalculateBaseTotal(_allCart, _kotCart);
		baseTotalAmountTextBox.Text = _baseTotal.FormatIndianCurrency();

		discountAmountTextBox.Text = BillWindowHelper.CalculateDiscountAmount(_allCart, _kotCart).ToString("N2");
		afterDiscsTotalAmountTextBox.Text = BillWindowHelper.CalculateAfterDiscountTotal(_allCart, _kotCart).FormatIndianCurrency();

		decimal subTotal = BillWindowHelper.CalculateSubTotal(_allCart, _kotCart);
		subTotalAmountTextBox.Text = subTotal.FormatIndianCurrency();

		decimal servicePercent = decimal.Parse(servicePercentTextBox.Text);
		decimal serviceAmount = subTotal * (servicePercent / 100);

		serviceAmountTextBox.Text = serviceAmount.ToString("N2");

		decimal entryPaid = decimal.Parse(entryPaidTextBox.Text);
		totalAmountTextBox.Text = (subTotal + serviceAmount - entryPaid).FormatIndianCurrency();
	}

	private void discountPercentTextBox_TextChanged(object sender, TextChangedEventArgs e)
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

		// Update Items with this value
		foreach (var item in _allCart)
		{
			if (item.SelfDiscount)
				continue;

			item.DiscPercent = percent;
			item.DiscAmount = item.BaseTotal * (item.DiscPercent / 100);
			item.AfterDiscount = item.BaseTotal - item.DiscAmount;
			item.CGSTAmount = item.AfterDiscount * (item.CGSTPercent / 100);
			item.SGSTAmount = item.AfterDiscount * (item.SGSTPercent / 100);
			item.IGSTAmount = item.AfterDiscount * (item.IGSTPercent / 100);
			item.Total = item.AfterDiscount + item.CGSTAmount + item.SGSTAmount + item.IGSTAmount;
		}

		foreach (var item in _kotCart)
		{
			if (item.SelfDiscount)
				continue;

			item.DiscPercent = percent;
			item.DiscAmount = item.BaseTotal * (item.DiscPercent / 100);
			item.AfterDiscount = item.BaseTotal - item.DiscAmount;
			item.CGSTAmount = item.AfterDiscount * (item.CGSTPercent / 100);
			item.SGSTAmount = item.AfterDiscount * (item.SGSTPercent / 100);
			item.IGSTAmount = item.AfterDiscount * (item.IGSTPercent / 100);
			item.Total = item.AfterDiscount + item.CGSTAmount + item.SGSTAmount + item.IGSTAmount;
		}

		RefreshTotal();

		_isUpdating = false;
	}

	private void servicePercentTextBox_TextChanged(object sender, TextChangedEventArgs e)
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

		RefreshTotal();

		_isUpdating = false;
	}

	private void serviceAmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
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

		decimal subTotal = BillWindowHelper.CalculateSubTotal(_allCart, _kotCart);
		decimal servicePercent = subTotal != 0 ? amount / subTotal * 100 : 0;

		servicePercentTextBox.Text = servicePercent.ToString("N2");

		RefreshTotal();

		_isUpdating = false;
	}
	#endregion

	#region KOT Processing
	private async Task AddProductToKotCart(ProductModel product)
	{
		var existingProduct = _kotCart.FirstOrDefault(c => c.ProductId == product.Id && c.Cancelled == false);

		if (existingProduct is not null)
		{
			existingProduct.Quantity++;
			existingProduct.Rate = product.Rate;

			existingProduct.BaseTotal = existingProduct.Rate * existingProduct.Quantity;
			existingProduct.DiscAmount = existingProduct.BaseTotal * (existingProduct.DiscPercent / 100);
			existingProduct.AfterDiscount = existingProduct.BaseTotal - existingProduct.DiscAmount;

			existingProduct.CGSTAmount = existingProduct.AfterDiscount * (existingProduct.CGSTPercent / 100);
			existingProduct.SGSTAmount = existingProduct.AfterDiscount * (existingProduct.SGSTPercent / 100);
			existingProduct.IGSTAmount = existingProduct.AfterDiscount * (existingProduct.IGSTPercent / 100);

			existingProduct.Total = existingProduct.AfterDiscount + existingProduct.CGSTAmount + existingProduct.SGSTAmount + existingProduct.IGSTAmount;
		}

		else
		{
			var productTax = await CommonData.LoadTableDataById<ProductTaxModel>(ViewNames.ProductTax, product.Id);

			var discountPercent = decimal.Parse(discountPercentTextBox.Text);
			var discountAmount = product.Rate * (discountPercent / 100);
			var afterDiscount = product.Rate - discountAmount;
			var cgstAmount = afterDiscount * (productTax.CGSTPercent / 100);
			var sgstAmount = afterDiscount * (productTax.SGSTPercent / 100);
			var igstAmount = afterDiscount * (productTax.IGSTPercent / 100);
			var total = afterDiscount + cgstAmount + sgstAmount + igstAmount;

			_kotCart.Add(new CartModel
			{
				ProductId = product.Id,
				ProductName = product.Name,
				Quantity = 1,
				Rate = product.Rate,
				BaseTotal = product.Rate,
				Discountable = true,
				SelfDiscount = false,
				DiscPercent = discountPercent,
				DiscAmount = discountAmount,
				AfterDiscount = afterDiscount,
				CGSTPercent = productTax.CGSTPercent,
				CGSTAmount = cgstAmount,
				SGSTPercent = productTax.SGSTPercent,
				SGSTAmount = sgstAmount,
				IGSTPercent = productTax.IGSTPercent,
				IGSTAmount = igstAmount,
				Total = total,
				Cancelled = false,
				Instruction = string.Empty
			});
		}
		RefreshTotal();
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
				BaseTotal = cart.BaseTotal,
				Instruction = cart.Instruction,
				Discountable = cart.Discountable,
				SelfDiscount = cart.SelfDiscount,
				DiscPercent = cart.DiscPercent,
				DiscAmount = cart.DiscAmount,
				AfterDiscount = cart.AfterDiscount,
				CGSTPercent = cart.CGSTPercent,
				CGSTAmount = cart.CGSTAmount,
				SGSTPercent = cart.SGSTPercent,
				SGSTAmount = cart.SGSTAmount,
				IGSTPercent = cart.IGSTPercent,
				IGSTAmount = cart.IGSTAmount,
				Total = cart.Total,
				Cancelled = cart.Cancelled
			});
	}

	private static void MergeCartItems()
	{
		foreach (var kotCart in _kotCart)
		{
			var allCart = _allCart
				.FirstOrDefault(c =>
				c.ProductId == kotCart.ProductId &&
				c.Cancelled == kotCart.Cancelled &&
				c.Discountable == kotCart.Discountable &&
				c.SelfDiscount == kotCart.SelfDiscount);

			if (allCart is not null)
			{
				allCart.Quantity += kotCart.Quantity;
				allCart.BaseTotal = allCart.Rate * allCart.Quantity;

				allCart.DiscPercent = kotCart.DiscPercent;
				allCart.DiscAmount = allCart.BaseTotal * (allCart.DiscPercent / 100);
				allCart.AfterDiscount = allCart.BaseTotal - allCart.DiscAmount;

				allCart.SGSTPercent = kotCart.SGSTPercent;
				allCart.CGSTPercent = kotCart.CGSTPercent;
				allCart.IGSTPercent = kotCart.IGSTPercent;

				allCart.CGSTAmount = allCart.AfterDiscount * (allCart.CGSTPercent / 100);
				allCart.SGSTAmount = allCart.AfterDiscount * (allCart.SGSTPercent / 100);
				allCart.IGSTAmount = allCart.AfterDiscount * (allCart.IGSTPercent / 100);

				allCart.Total = allCart.AfterDiscount + allCart.CGSTAmount + allCart.SGSTAmount + allCart.IGSTAmount;
			}
			else
				_allCart.Add(kotCart);

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

	#region Validation
	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private void decimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateDecimalInput(sender, e);

	private async void Window_Closed(object sender, EventArgs e)
	{
		await _tableDashboard.RefreshScreen();
		_tableDashboard.Show();
	}
	#endregion
}
