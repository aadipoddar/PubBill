﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using PubBillLibrary.Data.Billing;
using PubBillLibrary.Models.Billing;

namespace PubBill.Billing;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class BillWindow : Window
{
	#region Initial Loading

	private readonly UserModel _user;
	private readonly TableDashboard _tableDashboard;
	private readonly DiningTableModel _diningTableModel;
	private readonly DiningAreaModel _diningAreaModel;
	private readonly RunningBillModel _runningBillModel;

	private static readonly ObservableCollection<CartModel> _cart = [];
	private static readonly ObservableCollection<CartModel> _kotCart = [];

	public BillWindow(UserModel user, TableDashboard tableDashboard, DiningTableModel diningTableModel, DiningAreaModel diningAreaModel)
	{
		InitializeComponent();

		_cart.Clear();
		cartDataGrid.ItemsSource = _cart;
		_user = user;
		_tableDashboard = tableDashboard;
		_diningTableModel = diningTableModel;
		_diningAreaModel = diningAreaModel;

		RefreshTotal();
	}

	public BillWindow(UserModel user, TableDashboard tableDashboard, DiningTableModel diningTableModel, DiningAreaModel diningAreaModel, RunningBillModel runningBillModel)
	{
		InitializeComponent();

		_cart.Clear();
		cartDataGrid.ItemsSource = _cart;
		_user = user;
		_tableDashboard = tableDashboard;
		_diningTableModel = diningTableModel;
		_diningAreaModel = diningAreaModel;
		_runningBillModel = runningBillModel;

		RefreshTotal();
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		diningAreaTextBox.Text = _diningAreaModel.Name;
		diningTableTextBox.Text = _diningTableModel.Name;
		runningTimeTextBox.Text = "0";

		paymentModeComboBox.ItemsSource = await CommonData.LoadTableDataByStatus<PaymentModeModel>(TableNames.PaymentMode);
		paymentModeComboBox.DisplayMemberPath = nameof(PaymentModeModel.Name);
		paymentModeComboBox.SelectedValuePath = nameof(PaymentModeModel.Id);
		paymentModeComboBox.SelectedIndex = 0;

		await LoadProductGroupComboBox();

		await LoadComponentsFromRunningBill();
	}

	private async Task LoadComponentsFromRunningBill()
	{
		if (_runningBillModel is null) return;

		var runningTime = DateTime.Now - _runningBillModel.BillStartDateTime;

		runningTimeTextBox.Text = runningTime.ToString("hh\\:mm");

		var person = await CommonData.LoadTableDataById<PersonModel>(TableNames.Person, _runningBillModel.PersonId);
		if (person is not null)
		{
			personNameTextBox.Text = person.Name;
			personNumberTextBox.Text = person.Number;
			loyaltyCheckBox.IsChecked = person.Loyalty;
		}

		var adjAmount = _runningBillModel.AdjAmount;
		var total = _runningBillModel.Total;
		var percent = adjAmount / (total + adjAmount) * 100;

		totalPeopleTextBox.Text = _runningBillModel.TotalPeople.ToString();
		adjAmountTextBox.Text = _runningBillModel.AdjAmount.ToString();
		adjPercentTextBox.Text = percent.ToString();
		adjReasonTextBox.Text = _runningBillModel.AdjReason;
		remarkTextBox.Text = _runningBillModel.Remarks;
		totalAmountTextBox.Text = _runningBillModel.Total.ToString();
		paymentModeComboBox.SelectedValue = _runningBillModel.PaymentModeId;

		_cart.Clear();

		var runningTableDetails = await RunningBillData.LoadRunningBillDetailByRunningBillId(_runningBillModel.Id);
		foreach (var item in runningTableDetails)
		{
			var product = await CommonData.LoadTableDataById<ProductModel>(TableNames.Product, item.ProductId);
			if (product is not null)
			{
				_cart.Add(new CartModel
				{
					ProductId = product.Id,
					ProductName = product.Name,
					Quantity = item.Quantity,
					Rate = item.Rate,
					Instruction = item.Instruction
				});
			}
		}

		RefreshTotal();
	}

	#endregion

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

	private async void searchCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (string.IsNullOrEmpty(searchCodeTextBox.Text)) return;

		var products = await CommonData.LoadTableDataByStatus<ProductModel>(TableNames.Product);
		var foundProduct = products.FirstOrDefault(p => p.Code == searchCodeTextBox.Text);

		if (foundProduct is not null)
		{
			var existingCart = _cart.FirstOrDefault(c => c.ProductId == foundProduct.Id);
			if (existingCart is not null) existingCart.Quantity++;
			else _cart.Add(new CartModel
			{
				ProductId = foundProduct.Id,
				ProductName = foundProduct.Name,
				Quantity = 1,
				Rate = foundProduct.Rate,
				Instruction = string.Empty
			});
			RefreshTotal();

			searchCodeTextBox.Clear();
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

	#region Calculate Totals

	private decimal _originalTotal;
	private bool _isUpdating = false;

	private void RefreshTotal()
	{
		cartDataGrid.Items.Refresh();

		_originalTotal = 0;
		foreach (CartModel cart in _cart) _originalTotal += cart.Total;

		foreach (var column in cartDataGrid.Columns)
			if (column is DataGridTextColumn textColumn)
				textColumn.ElementStyle = new Style(typeof(TextBlock))
				{
					Setters = { new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right) }
				};

		decimal adjPercent = 0;
		if (decimal.TryParse(adjPercentTextBox.Text, out decimal parsedPercent))
		{
			adjPercent = Math.Clamp(parsedPercent, 0, 100);
			if (parsedPercent != adjPercent) adjPercentTextBox.Text = adjPercent.ToString("N2");
		}

		else adjPercentTextBox.Text = "0.00";

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

	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) => e.Handled = !int.TryParse(e.Text, out _);

	private void decimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) => e.Handled = !decimal.TryParse(e.Text, out _);

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

	#region KOT

	private async void kotButton_Click(object sender, RoutedEventArgs e)
	{
		if (cartDataGrid.Items.Count == 0)
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
			TotalPeople = int.Parse(totalPeopleTextBox.Text),
			AdjAmount = decimal.Parse(adjAmountTextBox.Text),
			AdjReason = adjReasonTextBox.Text,
			Remarks = remarkTextBox.Text,
			Total = decimal.Parse(totalAmountTextBox.Text),
			PaymentModeId = (int)paymentModeComboBox.SelectedValue,
			BillStartDateTime = _runningBillModel?.BillStartDateTime ?? DateTime.Now
		};
		if (string.IsNullOrEmpty(runningBill.AdjAmount.ToString())) runningBill.AdjAmount = 0;
		if (string.IsNullOrEmpty(runningBill.TotalPeople.ToString())) runningBill.TotalPeople = 0;

		return await RunningBillData.InsertRunningBill(runningBill);
	}

	private async Task InsertRunningBillDetails(int runningBillId)
	{
		if (_runningBillModel is not null) await RunningBillData.DeleteRunningBillDetail(_runningBillModel.Id);

		foreach (CartModel cart in _cart)
			await RunningBillData.InsertRunningBillDetail(new RunningBillDetailModel
			{
				Id = 0,
				RunningBillId = runningBillId,
				ProductId = cart.ProductId,
				Quantity = cart.Quantity,
				Rate = cart.Rate,
				Instruction = cart.Instruction
			});
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
		await ChangeTableStatus();
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

	private async Task ChangeTableStatus()
	{
		if (_runningBillModel is not null) await RunningBillData.DeleteRunningBillDetail(_runningBillModel.Id);
		await RunningBillData.DeleteRunningBill(_runningBillModel.Id);
	}

	#endregion

	private async void Window_Closed(object sender, EventArgs e)
	{
		await _tableDashboard.RefreshScreen();
		_tableDashboard.Show();
	}
}