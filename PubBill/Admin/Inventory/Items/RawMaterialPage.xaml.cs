using System.Windows;
using System.Windows.Controls;

namespace PubBill.Admin.Inventory.Items;

/// <summary>
/// Interaction logic for RawMaterialPage.xaml
/// </summary>
public partial class RawMaterialPage : Page
{
	public RawMaterialPage() =>
		InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		var rawMaterialCategories = await CommonData.LoadTableData<RawMaterialCategoryModel>(TableNames.RawMaterialCategory);
		rawMaterialCategoryComboBox.ItemsSource = rawMaterialCategories;
		rawMaterialCategoryComboBox.DisplayMemberPath = nameof(RawMaterialCategoryModel.Name);
		rawMaterialCategoryComboBox.SelectedValuePath = nameof(RawMaterialCategoryModel.Id);
		rawMaterialCategoryComboBox.SelectedIndex = 0;

		searchRawMaterialCategoryComboBox.ItemsSource = rawMaterialCategories;
		searchRawMaterialCategoryComboBox.DisplayMemberPath = nameof(RawMaterialCategoryModel.Name);
		searchRawMaterialCategoryComboBox.SelectedValuePath = nameof(RawMaterialCategoryModel.Id);
		searchRawMaterialCategoryComboBox.SelectedIndex = 0;

		var tax = await CommonData.LoadTableData<TaxModel>(TableNames.Tax);
		taxComboBox.ItemsSource = tax;
		taxComboBox.DisplayMemberPath = nameof(TaxModel.Code);
		taxComboBox.SelectedValuePath = nameof(TaxModel.Id);
		taxComboBox.SelectedIndex = 0;

		await ApplySearchFilter();
	}

	private async void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
		await ApplySearchFilter();

	private async void searchRawMaterialCategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
		await ApplySearchFilter();

	private async void showCheckBox_CheckedChanged(object sender, RoutedEventArgs e) =>
		await ApplySearchFilter();

	private async void rawMaterialDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) =>
		await UpdateFields();

	private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
		UpdateButtonField();

	private async Task ApplySearchFilter()
	{
		if (rawMaterialDataGrid is null || searchRawMaterialCategoryComboBox is null || searchRawMaterialCategoryComboBox.SelectedValue is null) return;

		var nameSearch = searchTextBox.Text.Trim();

		bool showActive = showActiveCheckBox?.IsChecked ?? false;
		bool showInactive = showInactiveCheckBox?.IsChecked ?? false;

		rawMaterialDataGrid.ItemsSource = (await RawMaterialData.LoadRawMaterialByRawMaterialCategory((int)searchRawMaterialCategoryComboBox.SelectedValue))
			.Where(item => string.IsNullOrEmpty(nameSearch) || item.Name.Contains(nameSearch, StringComparison.CurrentCultureIgnoreCase))
			.Where(item => (showActive && item.Status) || (showInactive && !item.Status))
			.OrderBy(item => !item.Status)
			.ToList();

		await UpdateFields();
	}

	private async Task UpdateFields()
	{
		if (rawMaterialDataGrid.SelectedItem is RawMaterialModel selectedRawMaterial)
		{
			nameTextBox.Text = selectedRawMaterial.Name;
			codeTextBox.Text = selectedRawMaterial.Code;
			mrpTextBox.Text = selectedRawMaterial.MRP.ToString();
			taxComboBox.SelectedValue = selectedRawMaterial.TaxId;
			statusCheckBox.IsChecked = selectedRawMaterial.Status;
			saveButton.Content = "Update";
			saveButton.IsEnabled = true;

			rawMaterialCategoryComboBox.ItemsSource = await CommonData.LoadTableData<RawMaterialCategoryModel>(TableNames.RawMaterialCategory);
			rawMaterialCategoryComboBox.DisplayMemberPath = nameof(RawMaterialCategoryModel.Name);
			rawMaterialCategoryComboBox.SelectedValuePath = nameof(RawMaterialCategoryModel.Id);
			rawMaterialCategoryComboBox.SelectedValue = selectedRawMaterial.RawMaterialCategoryId;
		}

		else
		{
			nameTextBox.Clear();
			codeTextBox.Clear();
			mrpTextBox.Text = "0.0";
			statusCheckBox.IsChecked = true;
			saveButton.Content = "Save";
			saveButton.IsEnabled = false;
		}

		UpdateButtonField();
	}

	private void UpdateButtonField()
	{
		if (rawMaterialDataGrid is null) return;

		if (rawMaterialDataGrid.SelectedItem is null) saveButton.Content = "Save";
		else saveButton.Content = "Update";

		if (!string.IsNullOrEmpty(nameTextBox.Text) && !string.IsNullOrEmpty(codeTextBox.Text) && !string.IsNullOrEmpty(mrpTextBox.Text)) saveButton.IsEnabled = true;
		else saveButton.IsEnabled = false;
	}

	private void decimalTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateDecimalInput(sender, e);

	private bool ValidateForm()
	{
		if (string.IsNullOrEmpty(nameTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Raw Material Name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrEmpty(codeTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Code", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrEmpty(mrpTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a MRP", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (rawMaterialCategoryComboBox.SelectedIndex == -1)
		{
			MessageBox.Show("Please select a Category", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm()) return;

		RawMaterialModel rawMaterialModel = new()
		{
			Id = rawMaterialDataGrid.SelectedItem is RawMaterialModel selectedRawMaterial ? selectedRawMaterial.Id : 0,
			Name = nameTextBox.Text,
			RawMaterialCategoryId = (int)rawMaterialCategoryComboBox.SelectedValue,
			Code = codeTextBox.Text.RemoveSpace(),
			MRP = decimal.Parse(mrpTextBox.Text),
			TaxId = (int)taxComboBox.SelectedValue,
			Status = (bool)statusCheckBox.IsChecked
		};

		await RawMaterialData.InsertRawMaterial(rawMaterialModel);

		await ApplySearchFilter();
	}
}
