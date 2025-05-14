using System.Windows;
using System.Windows.Controls;

namespace PubBill.Admin.Dining;

/// <summary>
/// Interaction logic for DiningAreaKitchenPage.xaml
/// </summary>
public partial class DiningAreaKitchenPage : Page
{
	public DiningAreaKitchenPage() =>
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

		var kitchens = await CommonData.LoadTableData<KitchenModel>(TableNames.Kitchen);
		kitchenComboBox.ItemsSource = kitchens;
		kitchenComboBox.DisplayMemberPath = nameof(KitchenModel.Name);
		kitchenComboBox.SelectedValuePath = nameof(KitchenModel.Id);
		kitchenComboBox.SelectedIndex = 0;

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

	private async void searchAreaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
		await ApplySearchFilter();

	private async void showCheckBox_CheckedChanged(object sender, RoutedEventArgs e) =>
		await ApplySearchFilter();

	private async void diningAreaKitchenDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) =>
		await UpdateFields();

	private async Task ApplySearchFilter()
	{
		if (diningAreaKitchenDataGrid is null || searchLocationComboBox is null || searchAreaComboBox is null || searchAreaComboBox.SelectedValue is null) return;

		bool showActive = showActiveCheckBox?.IsChecked ?? false;
		bool showInactive = showInactiveCheckBox?.IsChecked ?? false;

		diningAreaKitchenDataGrid.ItemsSource = (await CommonData.LoadTableData<DiningAreaKitchenModel>(TableNames.DiningAreaKitchen))
			.Where(item => item.DiningAreaId == (int)searchAreaComboBox.SelectedValue)
			.Where(item => (showActive && item.Status) || (showInactive && !item.Status))
			.OrderBy(item => !item.Status)
			.ToList();

		await UpdateFields();
	}

	private async Task UpdateFields()
	{
		if (diningAreaKitchenDataGrid.SelectedItem is DiningAreaKitchenModel selectedDiningAreaKitchen)
		{
			locationComboBox.SelectedValue = searchLocationComboBox.SelectedValue;
			kitchenComboBox.SelectedValue = selectedDiningAreaKitchen.KitchenId;

			statusCheckBox.IsChecked = selectedDiningAreaKitchen.Status;
			saveButton.Content = "Update";

			diningAreaComboBox.ItemsSource = await DiningAreaData.LoadDiningAreaByLocation((int)locationComboBox.SelectedValue);
			diningAreaComboBox.DisplayMemberPath = nameof(DiningAreaModel.Name);
			diningAreaComboBox.SelectedValuePath = nameof(DiningAreaModel.Id);
			diningAreaComboBox.SelectedValue = selectedDiningAreaKitchen.DiningAreaId;
		}

		else
		{
			statusCheckBox.IsChecked = true;
			saveButton.Content = "Save";
		}

		UpdateButtonField();
	}

	private void UpdateButtonField()
	{
		if (diningAreaKitchenDataGrid.SelectedItem is null) saveButton.Content = "Save";
		else saveButton.Content = "Update";
	}

	private bool ValidateForm()
	{
		if (locationComboBox.SelectedIndex == -1)
		{
			MessageBox.Show("Please select a Location", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm()) return;

		DiningAreaKitchenModel diningAreaKitchenModel = new()
		{
			Id = diningAreaKitchenDataGrid.SelectedItem is DiningAreaKitchenModel selectedDiningAreaKitchen ? selectedDiningAreaKitchen.Id : 0,
			DiningAreaId = (int)diningAreaComboBox.SelectedValue,
			KitchenId = (int)kitchenComboBox.SelectedValue,
			Status = (bool)statusCheckBox.IsChecked
		};

		await DiningAreaKitchenData.InsertDiningAreaKitchen(diningAreaKitchenModel);

		await ApplySearchFilter();
	}
}
