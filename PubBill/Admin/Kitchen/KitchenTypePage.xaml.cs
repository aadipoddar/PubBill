using System.Windows;
using System.Windows.Controls;

namespace PubBill.Admin.Kitchen;

/// <summary>
/// Interaction logic for KitchenTypePage.xaml
/// </summary>
public partial class KitchenTypePage : Page
{
	public KitchenTypePage() =>
		InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		if (kitchenTypeDataGrid is null) return;

		var nameSearch = searchTextBox.Text.Trim();

		var kitchenTypes = await CommonData.LoadTableData<KitchenTypeModel>(TableNames.KitchenType);

		bool showActive = showActiveCheckBox?.IsChecked ?? false;
		bool showInactive = showInactiveCheckBox?.IsChecked ?? false;

		kitchenTypeDataGrid.ItemsSource = kitchenTypes
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

	private void kitchenTypeDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) =>
		UpdateFields();

	private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
		UpdateButtonField();

	private void UpdateFields()
	{
		if (kitchenTypeDataGrid.SelectedItem is KitchenTypeModel selectedKitchenType)
		{
			nameTextBox.Text = selectedKitchenType.Name;
			statusCheckBox.IsChecked = selectedKitchenType.Status;
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
		if (kitchenTypeDataGrid.SelectedItem is null) saveButton.Content = "Save";
		else saveButton.Content = "Update";

		if (!string.IsNullOrEmpty(nameTextBox.Text)) saveButton.IsEnabled = true;
		else saveButton.IsEnabled = false;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (string.IsNullOrEmpty(nameTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Kitchen Mode Name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		KitchenTypeModel kitchenTypeModel = new()
		{
			Id = kitchenTypeDataGrid.SelectedItem is KitchenTypeModel selectedKitchType ? selectedKitchType.Id : 0,
			Name = nameTextBox.Text,
			Status = (bool)statusCheckBox.IsChecked
		};

		await KitchenTypeData.InsertKitchenType(kitchenTypeModel);

		await LoadData();
	}
}
