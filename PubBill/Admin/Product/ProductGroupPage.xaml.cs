using System.Windows;
using System.Windows.Controls;

namespace PubBill.Admin.Product;

/// <summary>
/// Interaction logic for ProductGroupPage.xaml
/// </summary>
public partial class ProductGroupPage : Page
{
	public ProductGroupPage() =>
		InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		if (productGroupDataGrid is null) return;

		var nameSearch = searchTextBox.Text.Trim();

		var productGroups = await CommonData.LoadTableData<ProductGroupModel>(TableNames.ProductGroup);

		bool showActive = showActiveCheckBox?.IsChecked ?? false;
		bool showInactive = showInactiveCheckBox?.IsChecked ?? false;

		productGroupDataGrid.ItemsSource = productGroups
			.Where(item => string.IsNullOrEmpty(nameSearch) || item.Name.Contains(nameSearch, StringComparison.CurrentCultureIgnoreCase))
			.Where(item => (showActive && item.Status) || (showInactive && !item.Status))
			.OrderBy(item => !item.Status)
			.ToList();

		UpdateFields();
	}

	private async void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
		await LoadData();

	private async void showCheckBox_CheckedChanged(object sender, RoutedEventArgs e) =>
		await LoadData();

	private void productGroupDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) =>
		UpdateFields();

	private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
		UpdateButtonField();

	private void UpdateFields()
	{
		if (productGroupDataGrid.SelectedItem is ProductGroupModel selectedProductGroup)
		{
			nameTextBox.Text = selectedProductGroup.Name;
			statusCheckBox.IsChecked = selectedProductGroup.Status;
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
		if (productGroupDataGrid.SelectedItem is null) saveButton.Content = "Save";
		else saveButton.Content = "Update";

		if (!string.IsNullOrEmpty(nameTextBox.Text)) saveButton.IsEnabled = true;
		else saveButton.IsEnabled = false;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(nameTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Group Name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		ProductGroupModel productGroupModel = new()
		{
			Id = productGroupDataGrid.SelectedItem is ProductGroupModel selectedProductGroup ? selectedProductGroup.Id : 0,
			Name = nameTextBox.Text,
			Status = (bool)statusCheckBox.IsChecked
		};

		await ProductGroupData.InsertProductGroup(productGroupModel);

		await LoadData();
	}
}
