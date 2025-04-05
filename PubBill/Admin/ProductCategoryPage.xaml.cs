using System.Windows;
using System.Windows.Controls;

namespace PubBill.Admin;

/// <summary>
/// Interaction logic for ProductCategoryPage.xaml
/// </summary>
public partial class ProductCategoryPage : Page
{
	public ProductCategoryPage()
	{
		InitializeComponent();
	}

	private async void Page_Loaded(object sender, RoutedEventArgs e) => await LoadData();

	private async Task LoadData()
	{
		var productGroups = await CommonData.LoadTableData<ProductGroupModel>(TableNames.ProductGroup);

		searchProductGroupComboBox.ItemsSource = productGroups;
		searchProductGroupComboBox.DisplayMemberPath = nameof(ProductGroupModel.Name);
		searchProductGroupComboBox.SelectedValuePath = nameof(ProductGroupModel.Id);
		searchProductGroupComboBox.SelectedIndex = 0;

		productGroupComboBox.ItemsSource = productGroups;
		productGroupComboBox.DisplayMemberPath = nameof(ProductGroupModel.Name);
		productGroupComboBox.SelectedValuePath = nameof(ProductGroupModel.Id);
		productGroupComboBox.SelectedIndex = 0;

		await ApplySearchFilter();
	}

	private async void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) => await ApplySearchFilter();

	private async void searchProductGroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => await ApplySearchFilter();

	private async void showCheckBox_CheckedChanged(object sender, RoutedEventArgs e) => await ApplySearchFilter();

	private void productCategoryDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) => UpdateFields();

	private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateButtonField();

	private async Task ApplySearchFilter()
	{
		if (productCategoryDataGrid is null || searchProductGroupComboBox is null) return;

		var nameSearch = searchTextBox.Text.Trim();

		bool showActive = showActiveCheckBox?.IsChecked ?? false;
		bool showInactive = showInactiveCheckBox?.IsChecked ?? false;

		productCategoryDataGrid.ItemsSource = (await ProductCategoryData.LoadProductCategoryByProductGroup((int)searchProductGroupComboBox.SelectedValue))
			.Where(item => string.IsNullOrEmpty(nameSearch) || item.Name.Contains(nameSearch, StringComparison.CurrentCultureIgnoreCase))
			.Where(item => (showActive && item.Status) || (showInactive && !item.Status))
			.OrderBy(item => !item.Status)
			.ToList();

		UpdateFields();
	}

	private void UpdateFields()
	{
		if (productCategoryDataGrid.SelectedItem is ProductCategoryModel selectedProductCategory)
		{
			nameTextBox.Text = selectedProductCategory.Name;
			productGroupComboBox.SelectedValue = selectedProductCategory.ProductGroupId;
			statusCheckBox.IsChecked = selectedProductCategory.Status;
			saveButton.Content = "Update";
			saveButton.IsEnabled = true;
		}

		else
		{
			nameTextBox.Clear();
			statusCheckBox.IsChecked = true;
			saveButton.Content = "Save";
			saveButton.IsEnabled = false;
		}

		UpdateButtonField();
	}

	private void UpdateButtonField()
	{
		if (productCategoryDataGrid.SelectedItem is null) saveButton.Content = "Save";
		else saveButton.Content = "Update";

		if (!string.IsNullOrEmpty(nameTextBox.Text)) saveButton.IsEnabled = true;
		else saveButton.IsEnabled = false;
	}

	private bool ValidateForm()
	{
		if (string.IsNullOrEmpty(nameTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter an Category Name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (productGroupComboBox.SelectedIndex == -1)
		{
			MessageBox.Show("Please select a Group", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm()) return;

		ProductCategoryModel productCategory = new()
		{
			Id = productCategoryDataGrid.SelectedItem is ProductCategoryModel selectedProductCategory ? selectedProductCategory.Id : 0,
			Name = nameTextBox.Text,
			ProductGroupId = (int)productGroupComboBox.SelectedValue,
			Status = (bool)statusCheckBox.IsChecked
		};

		await ProductCategoryData.InsertProductCategory(productCategory);

		await ApplySearchFilter();
	}
}
