using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PubBill.Inventory.Purchase;

/// <summary>
/// Interaction logic for PurchaseWindow.xaml
/// </summary>
public partial class PurchaseWindow : Window
{
	#region LoadData
	private readonly PurchaseOverviewModel _purchaseModel = null;
	private List<SupplierModel> _suppliers = [];
	private static readonly ObservableCollection<PurchaseCartModel> _cart = [];

	public PurchaseWindow() =>
		InitializeComponent();

	public PurchaseWindow(PurchaseOverviewModel purchaseModel)
	{
		_purchaseModel = purchaseModel;
		InitializeComponent();
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		billDatePicker.SelectedDate = DateTime.Now;
		cartDataGrid.ItemsSource = _cart;
		_suppliers = await CommonData.LoadTableDataByStatus<SupplierModel>(TableNames.Supplier);

		var rawMaterialCategories = await CommonData.LoadTableDataByStatus<RawMaterialCategoryModel>(TableNames.RawMaterialCategory);
		rawMaterialCategoryListBox.ItemsSource = rawMaterialCategories;
		rawMaterialCategoryListBox.DisplayMemberPath = nameof(RawMaterialCategoryModel.Name);
		rawMaterialCategoryListBox.SelectedValuePath = nameof(RawMaterialCategoryModel.Id);
		rawMaterialCategoryListBox.SelectedIndex = 0;

		await LoadPurchase();

		RefreshTotal();
	}

	private async Task LoadPurchase()
	{
		if (_purchaseModel is null)
			return;

		billDatePicker.SelectedDate = _purchaseModel.BillDate.ToDateTime(new TimeOnly(0, 0));
		billNoTextBox.Text = _purchaseModel.BillNo;

		var supplier = await CommonData.LoadTableDataById<SupplierModel>(TableNames.Supplier, _purchaseModel.SupplierId);
		supplierNameTextBox.Text = supplier.Name;

		_cart.Clear();

		var purchaseDetails = await PurchaseData.LoadPurchaseDetailByPurchase(_purchaseModel.Id);
		foreach (var item in purchaseDetails)
		{
			var rawMaterial = await CommonData.LoadTableDataById<RawMaterialModel>(TableNames.RawMaterial, item.RawMaterialId);

			_cart.Add(new PurchaseCartModel()
			{
				RawMaterialId = item.RawMaterialId,
				RawMaterialName = rawMaterial.Name,
				Quantity = item.Quantity,
				Rate = item.Rate,
				BaseTotal = item.BaseTotal,
				DiscPercent = item.DiscPercent,
				DiscAmount = item.DiscAmount,
				AfterDiscount = item.AfterDiscount,
				CGSTPercent = item.CGSTPercent,
				CGSTAmount = item.CGSTAmount,
				SGSTPercent = item.SGSTPercent,
				SGSTAmount = item.SGSTAmount,
				IGSTPercent = item.IGSTPercent,
				IGSTAmount = item.IGSTAmount,
				Total = item.Total
			});
		}

		cashDiscountPercentTextBox.Text = _purchaseModel.CashDiscountPercent.ToString();
		RefreshTotal();
	}

	private void supplierTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		var nameSearch = supplierNameTextBox.Text.Trim();

		var filteredSuppliers = _suppliers.Where(item =>
			string.IsNullOrEmpty(nameSearch) || item.Name.Contains(nameSearch, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

		if (filteredSuppliers is not null)
		{
			supplierGSTTextBox.Text = filteredSuppliers.GSTNo;
			supplierNumberTextBox.Text = filteredSuppliers.Phone;
			supplierEmailTextBox.Text = filteredSuppliers.Email;
		}

		if (filteredSuppliers is null || string.IsNullOrEmpty(nameSearch))
		{
			supplierGSTTextBox.Clear();
			supplierNumberTextBox.Clear();
			supplierEmailTextBox.Clear();
		}

		RefreshTotal();
	}
	#endregion

	#region RawMaterial
	private async void rawMaterialCategoryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (rawMaterialCategoryListBox.SelectedValue is int categoryId)
		{
			searchRawMaterialNameTextBox.Clear();
			rawMaterialDataGrid.ItemsSource = await RawMaterialData.LoadRawMaterialByRawMaterialCategory(categoryId);
		}

		RefreshTotal();
	}

	private async void searchRawMaterialNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		var searchText = searchRawMaterialNameTextBox.Text.Trim().RemoveSpace();
		if (string.IsNullOrEmpty(searchText))
			return;

		rawMaterialCategoryListBox.SelectedValue = null;

		rawMaterialDataGrid.ItemsSource = (await CommonData.LoadTableDataByStatus<RawMaterialModel>(TableNames.RawMaterial))
			.Where(r => r.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
			.ToList();

		RefreshTotal();
	}

	private async void rawMaterialDataGrid_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		if (e.OriginalSource is FrameworkElement element && element.DataContext is RawMaterialModel rawMaterial)
		{
			var existingProduct = _cart.FirstOrDefault(c => c.RawMaterialId == rawMaterial.Id);

			if (existingProduct is not null)
			{
				existingProduct.Quantity++;

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
				var productTax = await CommonData.LoadTableDataById<TaxModel>(TableNames.Tax, rawMaterial.TaxId);

				var discountPercent = 0;
				var discountAmount = rawMaterial.MRP * (discountPercent / 100);
				var afterDiscount = rawMaterial.MRP - discountAmount;
				var cgstAmount = afterDiscount * (productTax.CGST / 100);
				var sgstAmount = afterDiscount * (productTax.SGST / 100);
				var igstAmount = afterDiscount * (productTax.IGST / 100);
				var total = afterDiscount + cgstAmount + sgstAmount + igstAmount;

				_cart.Add(new PurchaseCartModel
				{
					RawMaterialId = rawMaterial.Id,
					RawMaterialName = rawMaterial.Name,
					Quantity = 1,
					Rate = rawMaterial.MRP,
					BaseTotal = rawMaterial.MRP,
					DiscPercent = discountPercent,
					DiscAmount = discountAmount,
					AfterDiscount = afterDiscount,
					CGSTPercent = productTax.CGST,
					CGSTAmount = cgstAmount,
					SGSTPercent = productTax.SGST,
					SGSTAmount = sgstAmount,
					IGSTPercent = productTax.IGST,
					IGSTAmount = igstAmount,
					Total = total,
				});
			}
		}

		RefreshTotal();
	}

	private void cartDataGrid_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		if (e.OriginalSource is FrameworkElement element && element.DataContext is PurchaseCartModel model)
		{
			PurchaseItemManageWindow purchaseItemManageWindow = new(model);
			bool? result = purchaseItemManageWindow.ShowDialog();

			if (result == true && model.Quantity == 0)
				_cart.Remove(model);
		}

		RefreshTotal();
	}
	#endregion

	#region Price Calculations
	private void cashDiscountPercentTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (!decimal.TryParse(cashDiscountPercentTextBox.Text, out decimal discountPercent))
		{
			cashDiscountPercentTextBox.Text = "0.00";
			discountPercent = 0;
		}
		discountPercent = Math.Clamp(discountPercent, 0, 100);
		if (discountPercent != decimal.Parse(cashDiscountPercentTextBox.Text))
			cashDiscountPercentTextBox.Text = discountPercent.ToString();

		RefreshTotal();
	}

	private void RefreshTotal()
	{
		if (cartDataGrid is null)
			return;

		cartDataGrid.Items.Refresh();

		var baseTotalAmount = _cart.Sum(x => x.BaseTotal);
		baseTotalAmountTextBox.Text = baseTotalAmount.FormatIndianCurrency();

		var discAmount = _cart.Sum(x => x.DiscAmount);
		var discountPercent = baseTotalAmount == 0 ? 0 : discAmount / baseTotalAmount * 100;
		discountPercentTextBox.Text = $"{discountPercent:F2}%";

		discountAmountTextBox.Text = discAmount.FormatIndianCurrency();
		afterDiscsTotalAmountTextBox.Text = _cart.Sum(x => x.AfterDiscount).FormatIndianCurrency();
		subTotalAmountTextBox.Text = _cart.Sum(x => x.Total).FormatIndianCurrency();

		var cashDiscount = decimal.Parse(cashDiscountPercentTextBox.Text);
		var cashDiscountAmount = _cart.Sum(x => x.Total) * (cashDiscount / 100);
		cashDiscountAmountTextBox.Text = cashDiscountAmount.ToString();
		totalAmountTextBox.Text = (_cart.Sum(x => x.Total) - cashDiscountAmount).FormatIndianCurrency();
	}
	#endregion

	#region Validation
	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateIntegerInput(sender, e);

	private void decimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateDecimalInput(sender, e);

	private bool ValidateForm()
	{
		if (string.IsNullOrEmpty(supplierGSTTextBox.Text.Trim()))
		{
			MessageBox.Show("Please select a supplier.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (_cart.Count == 0)
		{
			MessageBox.Show("Please add items to the cart.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrEmpty(billNoTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a bill number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		return true;
	}
	#endregion

	#region Saving
	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm())
			return;

		int purchaseId = await InsertPurchase();
		if (purchaseId == 0)
		{
			MessageBox.Show("Failed to save purchase data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		await InsertPurchaseDetail(purchaseId);
		await InsertStock(purchaseId);

		MessageBox.Show("Purchase data saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
		Close();
	}

	private async Task<int> InsertPurchase()
	{
		var filteredSupplier = _suppliers.Where(item =>
					string.IsNullOrEmpty(supplierNameTextBox.Text.Trim()) || item.Name.Contains(supplierNameTextBox.Text.Trim(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

		if (filteredSupplier is null)
		{
			MessageBox.Show("Please select a valid supplier.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return 0;
		}

		return
			await PurchaseData.InsertPurchase(new()
			{
				Id = _purchaseModel?.Id ?? 0,
				SupplierId = filteredSupplier.Id,
				BillDate = DateOnly.FromDateTime(billDatePicker.SelectedDate.Value),
				BillNo = billNoTextBox.Text.Trim(),
				CDPercent = decimal.Parse(cashDiscountPercentTextBox.Text),
				CDAmount = decimal.Parse(cashDiscountAmountTextBox.Text),
				Remarks = remarksTextBox.Text.Trim(),
				Status = true
			});
	}

	private async Task InsertPurchaseDetail(int purchaseId)
	{
		if (_purchaseModel is not null)
		{
			var purchaseDetails = await PurchaseData.LoadPurchaseDetailByPurchase(_purchaseModel.Id);
			foreach (var item in purchaseDetails)
			{
				item.Status = false;
				await PurchaseData.InsertPurchaseDetail(item);
			}
		}

		foreach (var item in _cart)
			await PurchaseData.InsertPurchaseDetail(new PurchaseDetailModel
			{
				Id = 0,
				PurchaseId = purchaseId,
				RawMaterialId = item.RawMaterialId,
				Quantity = item.Quantity,
				Rate = item.Rate,
				BaseTotal = item.BaseTotal,
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
				Status = true
			});
	}

	private async Task InsertStock(int purchaseId)
	{
		if (_purchaseModel is not null)
		{
			var stockDetails = await StockData.LoadStockByPurchase(_purchaseModel.Id);
			foreach (var stock in stockDetails)
				await StockData.DeleteStock(stock.Id);
		}

		foreach (var item in _cart)
			await StockData.InsertStock(new()
			{
				Id = 0,
				RawMaterialId = item.RawMaterialId,
				Quantity = item.Quantity,
				Type = StockType.Purchase.ToString(),
				PurchaseId = purchaseId,
				TransactionDT = billDatePicker.SelectedDate.Value
			});
	}
	#endregion
}
