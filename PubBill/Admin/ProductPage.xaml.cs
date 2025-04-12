using System.Windows;
using System.Windows.Controls;

namespace PubBill.Admin;

/// <summary>
/// Interaction logic for ProductPage.xaml
/// </summary>
public partial class ProductPage : Page
{
	public ProductPage() => InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) => await LoadData();

	private async Task LoadData()
	{
		var productGroups = await CommonData.LoadTableData<ProductGroupModel>(TableNames.ProductGroup);
		productGroupComboBox.ItemsSource = productGroups;
		productGroupComboBox.DisplayMemberPath = nameof(ProductGroupModel.Name);
		productGroupComboBox.SelectedValuePath = nameof(ProductGroupModel.Id);
		productGroupComboBox.SelectedIndex = 0;

		searchProductGroupComboBox.ItemsSource = productGroups;
		searchProductGroupComboBox.DisplayMemberPath = nameof(ProductGroupModel.Name);
		searchProductGroupComboBox.SelectedValuePath = nameof(ProductGroupModel.Id);
		searchProductGroupComboBox.SelectedIndex = 0;

		var productCategories = await ProductCategoryData.LoadProductCategoryByProductGroup((int)searchProductGroupComboBox.SelectedValue);
		productCategoryComboBox.ItemsSource = productCategories;
		productCategoryComboBox.DisplayMemberPath = nameof(ProductCategoryModel.Name);
		productCategoryComboBox.SelectedValuePath = nameof(ProductCategoryModel.Id);
		productCategoryComboBox.SelectedIndex = 0;

		searchProductCategoryComboBox.ItemsSource = productCategories;
		searchProductCategoryComboBox.DisplayMemberPath = nameof(ProductCategoryModel.Name);
		searchProductCategoryComboBox.SelectedValuePath = nameof(ProductCategoryModel.Id);
		searchProductCategoryComboBox.SelectedIndex = 0;

		var tax = await CommonData.LoadTableData<TaxModel>(TableNames.Tax);
		taxComboBox.ItemsSource = tax;
		taxComboBox.DisplayMemberPath = nameof(TaxModel.Code);
		taxComboBox.SelectedValuePath = nameof(TaxModel.Id);
		taxComboBox.SelectedIndex = 0;

		await ApplySearchFilter();
	}

	private async void productGroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (productGroupComboBox.SelectedItem is ProductGroupModel selectedProuctGroup)
		{
			productCategoryComboBox.ItemsSource = await ProductCategoryData.LoadProductCategoryByProductGroup(selectedProuctGroup.Id);
			productCategoryComboBox.DisplayMemberPath = nameof(ProductCategoryModel.Name);
			productCategoryComboBox.SelectedValuePath = nameof(ProductCategoryModel.Id);
			productCategoryComboBox.SelectedIndex = 0;
		}
	}

	private async void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) => await ApplySearchFilter();

	private async void searchProductGroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		await ApplySearchFilter();

		if (searchProductGroupComboBox.SelectedItem is ProductGroupModel selectedProductGroup)
		{
			searchProductCategoryComboBox.ItemsSource = await ProductCategoryData.LoadProductCategoryByProductGroup(selectedProductGroup.Id);
			searchProductCategoryComboBox.DisplayMemberPath = nameof(ProductCategoryModel.Name);
			searchProductCategoryComboBox.SelectedValuePath = nameof(ProductCategoryModel.Id);
			searchProductCategoryComboBox.SelectedIndex = 0;
		}
	}

	private async void searchProductCategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => await ApplySearchFilter();

	private async void showCheckBox_CheckedChanged(object sender, RoutedEventArgs e) => await ApplySearchFilter();

	private async void productDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) => await UpdateFields();

	private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateButtonField();

	private async Task ApplySearchFilter()
	{
		if (productDataGrid is null || searchProductGroupComboBox is null || searchProductCategoryComboBox is null || searchProductCategoryComboBox.SelectedValue is null) return;

		var nameSearch = searchTextBox.Text.Trim();

		bool showActive = showActiveCheckBox?.IsChecked ?? false;
		bool showInactive = showInactiveCheckBox?.IsChecked ?? false;

		productDataGrid.ItemsSource = (await ProductData.LoadProductByProductCategory((int)searchProductCategoryComboBox.SelectedValue))
			.Where(item => string.IsNullOrEmpty(nameSearch) || item.Name.Contains(nameSearch, StringComparison.CurrentCultureIgnoreCase))
			.Where(item => (showActive && item.Status) || (showInactive && !item.Status))
			.OrderBy(item => !item.Status)
			.ToList();

		await UpdateFields();
	}

	private async Task UpdateFields()
	{
		if (productDataGrid.SelectedItem is ProductModel selectedProduct)
		{
			nameTextBox.Text = selectedProduct.Name;
			codeTextBox.Text = selectedProduct.Code;
			productGroupComboBox.SelectedValue = searchProductGroupComboBox.SelectedValue;
			rateTextBox.Text = selectedProduct.Rate.ToString();
			taxComboBox.SelectedValue = selectedProduct.TaxId;
			statusCheckBox.IsChecked = selectedProduct.Status;
			saveButton.Content = "Update";
			saveButton.IsEnabled = true;

			productCategoryComboBox.ItemsSource = await ProductCategoryData.LoadProductCategoryByProductGroup((int)productGroupComboBox.SelectedValue);
			productCategoryComboBox.DisplayMemberPath = nameof(ProductCategoryModel.Name);
			productCategoryComboBox.SelectedValuePath = nameof(ProductCategoryModel.Id);
			productCategoryComboBox.SelectedValue = selectedProduct.ProductCategoryId;
		}

		else
		{
			nameTextBox.Clear();
			codeTextBox.Clear();
			rateTextBox.Text = "0.0";
			statusCheckBox.IsChecked = true;
			saveButton.Content = "Save";
			saveButton.IsEnabled = false;
		}

		UpdateButtonField();
	}

	private void UpdateButtonField()
	{
		if (productDataGrid is null) return;

		if (productDataGrid.SelectedItem is null) saveButton.Content = "Save";
		else saveButton.Content = "Update";

		if (!string.IsNullOrEmpty(nameTextBox.Text) && !string.IsNullOrEmpty(codeTextBox.Text) && !string.IsNullOrEmpty(rateTextBox.Text)) saveButton.IsEnabled = true;
		else saveButton.IsEnabled = false;
	}

	private void decimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateDecimalInput(sender, e);

	private bool ValidateForm()
	{
		if (string.IsNullOrEmpty(nameTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Product Name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrEmpty(codeTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Code", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrEmpty(rateTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Rate", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (productGroupComboBox.SelectedIndex == -1)
		{
			MessageBox.Show("Please select a Group", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (productCategoryComboBox.SelectedIndex == -1)
		{
			MessageBox.Show("Please select a Category", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm()) return;

		ProductModel productModel = new()
		{
			Id = productDataGrid.SelectedItem is ProductModel selectedProduct ? selectedProduct.Id : 0,
			Name = nameTextBox.Text,
			Code = codeTextBox.Text.RemoveSpace(),
			ProductCategoryId = (int)productCategoryComboBox.SelectedValue,
			Rate = decimal.Parse(rateTextBox.Text),
			TaxId = (int)taxComboBox.SelectedValue,
			Status = (bool)statusCheckBox.IsChecked
		};

		await ProductData.InsertProduct(productModel);

		await ApplySearchFilter();
	}
}
