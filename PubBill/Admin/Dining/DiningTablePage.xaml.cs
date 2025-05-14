using System.Windows;
using System.Windows.Controls;

namespace PubBill.Admin.Dining;

/// <summary>
/// Interaction logic for DiningTablePage.xaml
/// </summary>
public partial class DiningTablePage : Page
{
	public DiningTablePage() =>
		InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		var locations = await CommonData.LoadTableData<LocationModel>(TableNames.Location);
		locationComboBox.ItemsSource = locations;
		locationComboBox.DisplayMemberPath = nameof(LocationModel.Name);
		locationComboBox.SelectedValuePath = nameof(LocationModel.Id);
		locationComboBox.SelectedIndex = 0;

		searchLocationComboBox.ItemsSource = locations;
		searchLocationComboBox.DisplayMemberPath = nameof(LocationModel.Name);
		searchLocationComboBox.SelectedValuePath = nameof(LocationModel.Id);
		searchLocationComboBox.SelectedIndex = 0;

		var diningAreas = await DiningAreaData.LoadDiningAreaByLocation((int)searchLocationComboBox.SelectedValue);
		diningAreaComboBox.ItemsSource = diningAreas;
		diningAreaComboBox.DisplayMemberPath = nameof(DiningAreaModel.Name);
		diningAreaComboBox.SelectedValuePath = nameof(DiningAreaModel.Id);
		diningAreaComboBox.SelectedIndex = 0;

		searchAreaComboBox.ItemsSource = diningAreas;
		searchAreaComboBox.DisplayMemberPath = nameof(DiningAreaModel.Name);
		searchAreaComboBox.SelectedValuePath = nameof(DiningAreaModel.Id);
		searchAreaComboBox.SelectedIndex = 0;

		await ApplySearchFilter();
	}

	private async void locationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (locationComboBox.SelectedItem is LocationModel selectedLocation)
		{
			diningAreaComboBox.ItemsSource = await DiningAreaData.LoadDiningAreaByLocation(selectedLocation.Id);
			diningAreaComboBox.DisplayMemberPath = nameof(DiningAreaModel.Name);
			diningAreaComboBox.SelectedValuePath = nameof(DiningAreaModel.Id);
			diningAreaComboBox.SelectedIndex = 0;
		}
	}

	private async void searchLocationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		await ApplySearchFilter();

		if (searchLocationComboBox.SelectedItem is LocationModel selectedLocation)
		{
			searchAreaComboBox.ItemsSource = await DiningAreaData.LoadDiningAreaByLocation(selectedLocation.Id);
			searchAreaComboBox.DisplayMemberPath = nameof(DiningAreaModel.Name);
			searchAreaComboBox.SelectedValuePath = nameof(DiningAreaModel.Id);
			searchAreaComboBox.SelectedIndex = 0;
		}
	}

	private async void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
		await ApplySearchFilter();

	private async void searchAreaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
		await ApplySearchFilter();

	private async void showCheckBox_CheckedChanged(object sender, RoutedEventArgs e) =>
		await ApplySearchFilter();

	private async void diningTableDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) =>
		await UpdateFields();

	private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
		UpdateButtonField();

	private async Task ApplySearchFilter()
	{
		if (diningTableDataGrid is null || searchLocationComboBox is null || searchAreaComboBox is null || searchAreaComboBox.SelectedValue is null) return;

		var nameSearch = searchTextBox.Text.Trim();

		bool showActive = showActiveCheckBox?.IsChecked ?? false;
		bool showInactive = showInactiveCheckBox?.IsChecked ?? false;

		diningTableDataGrid.ItemsSource = (await DiningTableData.LoadDiningTableByDiningArea((int)searchAreaComboBox.SelectedValue))
			.Where(item => string.IsNullOrEmpty(nameSearch) || item.Name.Contains(nameSearch, StringComparison.CurrentCultureIgnoreCase))
			.Where(item => (showActive && item.Status) || (showInactive && !item.Status))
			.OrderBy(item => !item.Status)
			.ToList();

		await UpdateFields();
	}

	private async Task UpdateFields()
	{
		if (diningTableDataGrid.SelectedItem is DiningTableModel selectedDiningTable)
		{
			nameTextBox.Text = selectedDiningTable.Name;
			locationComboBox.SelectedValue = searchLocationComboBox.SelectedValue;
			statusCheckBox.IsChecked = selectedDiningTable.Status;
			saveButton.Content = "Update";
			saveButton.IsEnabled = true;

			diningAreaComboBox.ItemsSource = await DiningAreaData.LoadDiningAreaByLocation((int)locationComboBox.SelectedValue);
			diningAreaComboBox.DisplayMemberPath = nameof(DiningAreaModel.Name);
			diningAreaComboBox.SelectedValuePath = nameof(DiningAreaModel.Id);
			diningAreaComboBox.SelectedValue = selectedDiningTable.DiningAreaId;
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
		if (diningTableDataGrid.SelectedItem is null) saveButton.Content = "Save";
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

		if (locationComboBox.SelectedIndex == -1)
		{
			MessageBox.Show("Please select a Location", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (diningAreaComboBox.SelectedIndex == -1)
		{
			MessageBox.Show("Please select an Area", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm()) return;

		DiningTableModel diningTableModel = new()
		{
			Id = diningTableDataGrid.SelectedItem is DiningTableModel selectedDiningTable ? selectedDiningTable.Id : 0,
			Name = nameTextBox.Text,
			DiningAreaId = (int)diningAreaComboBox.SelectedValue,
			Status = (bool)statusCheckBox.IsChecked
		};

		await DiningTableData.InsertDiningTable(diningTableModel);

		await ApplySearchFilter();
	}
}
