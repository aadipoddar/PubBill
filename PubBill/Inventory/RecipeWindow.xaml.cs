using System.Collections.ObjectModel;
using System.Windows;

namespace PubBill.Inventory;

/// <summary>
/// Interaction logic for RecipeWindow.xaml
/// </summary>
public partial class RecipeWindow : Window
{
	private readonly ObservableCollection<RawMaterialRecipeModel> _rawMaterials = [];
	private RecipeModel _recipe = new();

	public RecipeWindow() =>
		InitializeComponent();

	#region Load Data
	private async void Window_Loaded(object sender, RoutedEventArgs e) =>
		await LoadData();

	private async Task LoadData()
	{
		productGroupComboBox.ItemsSource = await CommonData.LoadTableData<ProductGroupModel>(TableNames.ProductGroup);
		productGroupComboBox.DisplayMemberPath = nameof(ProductGroupModel.Name);
		productGroupComboBox.SelectedValuePath = nameof(ProductGroupModel.Id);
		productGroupComboBox.SelectedIndex = 0;

		productCategoryComboBox.ItemsSource = await ProductCategoryData.LoadProductCategoryByProductGroup((int)productGroupComboBox.SelectedValue);
		productCategoryComboBox.DisplayMemberPath = nameof(ProductCategoryModel.Name);
		productCategoryComboBox.SelectedValuePath = nameof(ProductCategoryModel.Id);
		productCategoryComboBox.SelectedIndex = 0;

		productComboBox.ItemsSource = await ProductData.LoadProductByProductCategory((int)productCategoryComboBox.SelectedValue);
		productComboBox.DisplayMemberPath = nameof(ProductModel.Name);
		productComboBox.SelectedValuePath = nameof(ProductModel.Id);
		productComboBox.SelectedIndex = 0;

		rawMaterialCategoryComboBox.ItemsSource = await CommonData.LoadTableData<RawMaterialCategoryModel>(TableNames.RawMaterialCategory);
		rawMaterialCategoryComboBox.DisplayMemberPath = nameof(RawMaterialCategoryModel.Name);
		rawMaterialCategoryComboBox.SelectedValuePath = nameof(RawMaterialCategoryModel.Id);
		rawMaterialCategoryComboBox.SelectedIndex = 0;

		rawMaterialComboBox.ItemsSource = await RawMaterialData.LoadRawMaterialByRawMaterialCategory((int)rawMaterialCategoryComboBox.SelectedValue);
		rawMaterialComboBox.DisplayMemberPath = nameof(RawMaterialModel.Name);
		rawMaterialComboBox.SelectedValuePath = nameof(RawMaterialModel.Id);
		rawMaterialComboBox.SelectedIndex = 0;

		quantityTextBox.Text = "1.00";

		_rawMaterials.Clear();
		recipeDataGrid.ItemsSource = _rawMaterials;
		recipeDataGrid.Items.Refresh();
	}

	private async Task LoadRecipe()
	{
		_rawMaterials.Clear();
		recipeDataGrid.Items.Refresh();

		saveButton.Content = "Save";

		if (productComboBox.SelectedValue is null)
			return;

		_recipe = await RecipeData.LoadRecipeByProduct((int)productComboBox.SelectedValue);
		if (_recipe is null)
			return;

		var recipeDetails = await RecipeData.LoadRecipeDetailByRecipe(_recipe.Id);
		if (recipeDetails is null)
			return;

		_rawMaterials.Clear();
		recipeDataGrid.Items.Refresh();

		foreach (var recipeDetail in recipeDetails)
		{
			var rawMaterial = await CommonData.LoadTableDataById<RawMaterialModel>(TableNames.RawMaterial, recipeDetail.RawMaterialId);

			_rawMaterials.Add(new RawMaterialRecipeModel
			{
				RawMaterialId = recipeDetail.RawMaterialId,
				RawMaterialName = rawMaterial.Name,
				RawMaterialCategoryId = rawMaterial.RawMaterialCategoryId,
				Quantity = recipeDetail.Quantity
			});

			saveButton.Content = "Update";
		}

		recipeDataGrid.Items.Refresh();
	}

	private async void productGroupComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
	{
		if (productGroupComboBox.SelectedValue is null)
			return;

		productCategoryComboBox.ItemsSource = await ProductCategoryData.LoadProductCategoryByProductGroup((int)productGroupComboBox.SelectedValue);
		productCategoryComboBox.DisplayMemberPath = nameof(ProductCategoryModel.Name);
		productCategoryComboBox.SelectedValuePath = nameof(ProductCategoryModel.Id);
		productCategoryComboBox.SelectedIndex = 0;

		await LoadRecipe();
	}

	private async void productCategoryComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
	{
		if (productCategoryComboBox is null || productCategoryComboBox.SelectedValue is null)
			return;

		productComboBox.ItemsSource = await ProductData.LoadProductByProductCategory((int)productCategoryComboBox.SelectedValue);
		productComboBox.DisplayMemberPath = nameof(ProductModel.Name);
		productComboBox.SelectedValuePath = nameof(ProductModel.Id);
		productComboBox.SelectedIndex = 0;

		await LoadRecipe();
	}

	private async void productComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) =>
		await LoadRecipe();

	private async void rawMaterialCategoryComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
	{
		rawMaterialComboBox.ItemsSource = await RawMaterialData.LoadRawMaterialByRawMaterialCategory((int)rawMaterialCategoryComboBox.SelectedValue);
		rawMaterialComboBox.DisplayMemberPath = nameof(RawMaterialModel.Name);
		rawMaterialComboBox.SelectedValuePath = nameof(RawMaterialModel.Id);
		rawMaterialComboBox.SelectedIndex = 0;
	}
	#endregion

	#region Quantity
	private void numberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) =>
		Helper.ValidateDecimalInput(sender, e);

	private void quantityMinusButton_Click(object sender, RoutedEventArgs e) =>
		UpdateQuantity(-1);

	private void quantityPlusButton_Click(object sender, RoutedEventArgs e) =>
		UpdateQuantity(1);

	private void UpdateQuantity(int change)
	{
		decimal newQty = decimal.Parse(quantityTextBox.Text) + change;
		quantityTextBox.Text = Math.Max(0, newQty).ToString();
	}
	#endregion

	private void addButton_Click(object sender, RoutedEventArgs e)
	{
		UpdateQuantity(0);

		if (string.IsNullOrEmpty(quantityTextBox.Text))
			return;

		if (rawMaterialComboBox.SelectedItem is not RawMaterialModel selectedRawMaterial)
			return;

		if (_rawMaterials.Any(x => x.RawMaterialId == selectedRawMaterial.Id))
			_rawMaterials.FirstOrDefault(x => x.RawMaterialId == selectedRawMaterial.Id).Quantity += decimal.Parse(quantityTextBox.Text);

		else
			_rawMaterials.Add(new RawMaterialRecipeModel
			{
				RawMaterialId = selectedRawMaterial.Id,
				RawMaterialName = selectedRawMaterial.Name,
				RawMaterialCategoryId = selectedRawMaterial.RawMaterialCategoryId,
				Quantity = decimal.Parse(quantityTextBox.Text)
			});

		recipeDataGrid.Items.Refresh();
	}

	private void amountDataGrid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		if (recipeDataGrid.SelectedItem is RawMaterialRecipeModel selectedRawMaterial)
		{
			quantityTextBox.Text = selectedRawMaterial.Quantity.ToString();
			rawMaterialCategoryComboBox.SelectedValue = selectedRawMaterial.RawMaterialCategoryId;
			rawMaterialComboBox.SelectedValue = selectedRawMaterial.RawMaterialId;
			_rawMaterials.Remove(selectedRawMaterial);
		}
		else
		{
			quantityTextBox.Text = "1";
			rawMaterialComboBox.SelectedIndex = 0;
		}

		recipeDataGrid.Items.Refresh();
	}

	private async void saveButton_Click(object sender, RoutedEventArgs e)
	{
		recipeDataGrid.Items.Refresh();

		if (productComboBox.SelectedItem is not ProductModel selectedProduct)
			return;

		if (_rawMaterials.Count == 0)
		{
			MessageBox.Show("Please add raw materials to the recipe.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			return;
		}

		if (_recipe is not null)
		{
			var recipeDetails = await RecipeData.LoadRecipeDetailByRecipe(_recipe.Id);

			foreach (var item in recipeDetails)
				await RecipeData.InsertRecipeDetail(new RecipeDetailModel
				{
					Id = item.Id,
					RecipeId = item.RecipeId,
					RawMaterialId = item.RawMaterialId,
					Quantity = item.Quantity,
					Status = false,
				});
		}

		var recipe = new RecipeModel
		{
			Id = _recipe?.Id ?? 0,
			ProductId = selectedProduct.Id,
			Status = true,
		};

		recipe.Id = await RecipeData.InsertRecipe(recipe);

		foreach (var rawMaterial in _rawMaterials)
			await RecipeData.InsertRecipeDetail(new RecipeDetailModel
			{
				Id = 0,
				RecipeId = recipe.Id,
				RawMaterialId = rawMaterial.RawMaterialId,
				Quantity = rawMaterial.Quantity,
				Status = true,
			});

		MessageBox.Show("Recipe saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
		await LoadData();
	}
}