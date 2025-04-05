using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace PubBill.Admin;

/// <summary>
/// Interaction logic for PersonPage.xaml
/// </summary>
public partial class PersonPage : Page
{
	#region LoadData

	public PersonPage() => InitializeComponent();

	private async void Page_Loaded(object sender, RoutedEventArgs e) => await LoadData();

	private async Task LoadData()
	{
		if (personDataGrid is null) return;

		var nameSearch = searchNameTextBox.Text.Trim();
		var numberSearch = searchNumberTextBox.Text.Trim();

		bool showLoyalty = showLoyaltyCheckBox?.IsChecked ?? false;
		bool showNonLoyalty = showNonLoyaltyCheckBox?.IsChecked ?? false;

		personDataGrid.ItemsSource = (await CommonData.LoadTableData<PersonModel>(TableNames.Person))
			.Where(item => string.IsNullOrEmpty(nameSearch) || item.Name.Contains(nameSearch, StringComparison.CurrentCultureIgnoreCase))
			.Where(item => string.IsNullOrEmpty(numberSearch) || item.Number.Contains(numberSearch, StringComparison.CurrentCultureIgnoreCase))
			.Where(item => (showLoyalty && item.Loyalty) || (showNonLoyalty && !item.Loyalty))
			.ToList();

		UpdateFields();
	}

	private void UpdateFields()
	{
		if (personDataGrid.SelectedItem is PersonModel selectedPerson)
		{
			nameTextBox.Text = selectedPerson.Name;
			numberTextBox.Text = selectedPerson.Number;
			loyaltyCheckBox.IsChecked = selectedPerson.Loyalty;
			saveButton.Content = "Update";
			saveButton.IsEnabled = true;
		}

		else
		{
			nameTextBox.Clear();
			numberTextBox.Clear();
			loyaltyCheckBox.IsChecked = false;
			saveButton.Content = "Save";
			saveButton.IsEnabled = false;
		}

		UpdateButtonField();
	}

	private void UpdateButtonField()
	{
		if (personDataGrid.SelectedItem is null)
			saveButton.Content = "Save";
		else
			saveButton.Content = "Update";

		if (!string.IsNullOrEmpty(nameTextBox.Text) && !string.IsNullOrEmpty(numberTextBox.Text)) saveButton.IsEnabled = true;
		else saveButton.IsEnabled = false;
	}

	#endregion

	#region Events

	private async void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) => await LoadData();

	private async void showCheckBox_CheckedChanged(object sender, RoutedEventArgs e) => await LoadData();

	private void personDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) => UpdateFields();

	private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateButtonField();

	private void textBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
	{
		Regex regex = new("[^0-9]+");
		e.Handled = regex.IsMatch(e.Text);
	}

	#endregion

	#region Saving

	private bool ValidateForm()
	{
		if (string.IsNullOrEmpty(nameTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a User Name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		if (string.IsNullOrEmpty(numberTextBox.Text.Trim()))
		{
			MessageBox.Show("Please enter a Number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		return true;
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		if (!ValidateForm()) return;

		PersonModel personModel = new()
		{
			Id = personDataGrid.SelectedItem is PersonModel selectedPerson ? selectedPerson.Id : 0,
			Name = nameTextBox.Text,
			Number = numberTextBox.Text,
			Loyalty = (bool)loyaltyCheckBox.IsChecked
		};

		if (personDataGrid.SelectedItem is PersonModel personModel1)
		{
			var foundPerson = await PersonData.LoadPersonByNumber(personModel.Number);
			if (foundPerson is not null && foundPerson.Id != personModel1.Id)
			{
				MessageBox.Show("Number already exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
		}
		else
		{
			if (await PersonData.LoadPersonByNumber(personModel.Number) is not null)
			{
				MessageBox.Show("Number already exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
		}

		await PersonData.InsertPerson(personModel);

		await LoadData();
	}

	#endregion
}
