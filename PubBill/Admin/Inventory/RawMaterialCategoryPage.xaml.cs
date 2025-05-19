using System.Windows;
using System.Windows.Controls;

namespace PubBill.Admin.Inventory;

/// <summary>
/// Interaction logic for RawMaterialCategoryPage.xaml
/// </summary>
public partial class RawMaterialCategoryPage : Page
{
	public RawMaterialCategoryPage() =>
		InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await ApplySearchFilter();

	private async void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
		await ApplySearchFilter();

	private async void showCheckBox_CheckedChanged(object sender, RoutedEventArgs e) =>
		await ApplySearchFilter();

	private void rawMaterialCategoryDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) =>
		UpdateFields();

	private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
		UpdateButtonField();

	private async Task ApplySearchFilter()
	{
		if (rawMaterialCategoryDataGrid is null) return;

		var nameSearch = searchTextBox.Text.Trim();

		bool showActive = showActiveCheckBox?.IsChecked ?? false;
		bool showInactive = showInactiveCheckBox?.IsChecked ?? false;

		rawMaterialCategoryDataGrid.ItemsSource = (await CommonData.LoadTableData<RawMaterialCategoryModel>(TableNames.RawMaterialCategory))
			.Where(item => string.IsNullOrEmpty(nameSearch) || item.Name.Contains(nameSearch, StringComparison.CurrentCultureIgnoreCase))
			.Where(item => (showActive && item.Status) || (showInactive && !item.Status))
			.OrderBy(item => !item.Status)
			.ToList();

		UpdateFields();
	}

	private void UpdateFields()
	{
		if (rawMaterialCategoryDataGrid.SelectedItem is RawMaterialCategoryModel selectedRawMaterial)
		{
			nameTextBox.Text = selectedRawMaterial.Name;
			statusCheckBox.IsChecked = selectedRawMaterial.Status;
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
		if (rawMaterialCategoryDataGrid.SelectedItem is null) saveButton.Content = "Save";
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

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm()) return;

		RawMaterialCategoryModel rawMaterialCategory = new()
		{
			Id = rawMaterialCategoryDataGrid.SelectedItem is RawMaterialCategoryModel selectedRawMaterialCategory ? selectedRawMaterialCategory.Id : 0,
			Name = nameTextBox.Text,
			Status = (bool)statusCheckBox.IsChecked
		};

		await RawMaterialCategoryData.InsertRawMaterialCategory(rawMaterialCategory);

		await ApplySearchFilter();
	}
}