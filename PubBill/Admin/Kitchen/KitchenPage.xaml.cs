using System.Drawing.Printing;
using System.Windows;
using System.Windows.Controls;

namespace PubBill.Admin.Kitchen;

/// <summary>
/// Interaction logic for KitchenPage.xaml
/// </summary>
public partial class KitchenPage : Page
{
	public KitchenPage() =>
		InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		var kitchenType = await CommonData.LoadTableData<KitchenTypeModel>(TableNames.KitchenType);
		kitchenTypeComboBox.ItemsSource = kitchenType;
		kitchenTypeComboBox.DisplayMemberPath = nameof(KitchenTypeModel.Name);
		kitchenTypeComboBox.SelectedValuePath = nameof(KitchenTypeModel.Id);
		kitchenTypeComboBox.SelectedIndex = 0;

		searchKitchenTypeComboBox.ItemsSource = kitchenType;
		searchKitchenTypeComboBox.DisplayMemberPath = nameof(KitchenTypeModel.Name);
		searchKitchenTypeComboBox.SelectedValuePath = nameof(KitchenTypeModel.Id);
		searchKitchenTypeComboBox.SelectedIndex = 0;

		printerComboBox.ItemsSource = PrinterSettings.InstalledPrinters;
		printerComboBox.SelectedIndex = 0;

		await ApplySearchFilter();
	}

	private async void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
		await ApplySearchFilter();

	private async void searchKitchenTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
		await ApplySearchFilter();

	private async void showCheckBox_CheckedChanged(object sender, RoutedEventArgs e) =>
		await ApplySearchFilter();

	private void kitchenDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) =>
		UpdateFields();

	private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
		UpdateButtonField();

	private async Task ApplySearchFilter()
	{
		if (kitchenDataGrid is null || searchKitchenTypeComboBox is null) return;

		var nameSearch = searchTextBox.Text.Trim();

		bool showActive = showActiveCheckBox?.IsChecked ?? false;
		bool showInactive = showInactiveCheckBox?.IsChecked ?? false;

		kitchenDataGrid.ItemsSource = (await CommonData.LoadTableData<KitchenModel>(TableNames.Kitchen))
			.Where(item => string.IsNullOrEmpty(nameSearch) || item.Name.Contains(nameSearch, StringComparison.CurrentCultureIgnoreCase))
			.Where(item => item.KitchenTypeId == (int)searchKitchenTypeComboBox.SelectedValue)
			.Where(item => (showActive && item.Status) || (showInactive && !item.Status))
			.OrderBy(item => !item.Status)
			.ToList();

		UpdateFields();
	}

	private void UpdateFields()
	{
		if (kitchenDataGrid.SelectedItem is KitchenModel selectedKitchen)
		{
			nameTextBox.Text = selectedKitchen.Name;
			kitchenTypeComboBox.SelectedValue = selectedKitchen.KitchenTypeId;
			printerComboBox.SelectedValue = selectedKitchen.PrinterName;
			statusCheckBox.IsChecked = selectedKitchen.Status;
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
		if (kitchenDataGrid.SelectedItem is null) saveButton.Content = "Save";
		else saveButton.Content = "Update";

		if (!string.IsNullOrEmpty(nameTextBox.Text)) saveButton.IsEnabled = true;
		else saveButton.IsEnabled = false;
	}

	private bool ValidateForm()
	{
		if (string.IsNullOrEmpty(nameTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter an Table Name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm()) return;

		KitchenModel kitchen = new()
		{
			Id = kitchenDataGrid.SelectedItem is KitchenModel selectedKitchen ? selectedKitchen.Id : 0,
			Name = nameTextBox.Text,
			PrinterName = (string)printerComboBox.SelectedItem,
			KitchenTypeId = (int)kitchenTypeComboBox.SelectedValue,
			Status = (bool)statusCheckBox.IsChecked
		};

		await KitchenData.InsertKitchen(kitchen);

		await ApplySearchFilter();
	}
}
