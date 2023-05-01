using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Reactive.Linq;
using BookShop.Core.Models;
using BookShop.ViewModels;
using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;
using ExcelDataReader;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Globalization;
using Windows.Globalization.NumberFormatting;
using Windows.Storage.Pickers;

namespace BookShop.Views;

public sealed partial class HomePage : Page
{
    private List<Categories>? _categories;
    private AdvancedCollectionView _acv;
    public ProductsViewModel ViewModel
    {
        get;
    }

    public HomePage()
    {
        ViewModel = App.GetService<ProductsViewModel>();
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        _categories = (List<Categories>) await App.Repository.Categories.GetAllCategoriesAsync();

        _acv = new AdvancedCollectionView(_categories, false);
        _acv.SortDescriptions.Add(new SortDescription(nameof(Categories.Name), SortDirection.Ascending));
        _acv.Filter = item =>
        {
            bool isFoundResult = !TokenBoxCategories.Items.Contains(item) && (item as Categories).Name.Contains(TokenBoxCategories.Text, System.StringComparison.CurrentCultureIgnoreCase);
            return isFoundResult;
        };

        TokenBoxCategories.TextChanged += TextChanged;
        TokenBoxCategories.TokenItemAdding += TokenItemCreating;
        TokenBoxCategories.SuggestedItemsSource = _acv;

        MinPriceNumberBox.NumberFormatter = new CurrencyFormatter(CurrencyIdentifiers.VND)
        {
            IntegerDigits = 1,
            FractionDigits = 0,
        };
        MaxPriceNumberBox.NumberFormatter = new CurrencyFormatter(CurrencyIdentifiers.VND)
        {
            IntegerDigits = 1,
            FractionDigits = 0,
        };
    }

    public async Task ShowDialog(string title, string message)
    {
        await App.MainWindow.ShowMessageDialogAsync(message, title);
    }

    #region Filter Opts
    private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.CheckCurrent() && args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            _acv.RefreshFilter();
        }
    }
    private void TokenItemCreating(object sender, TokenItemAddingEventArgs e)
    {
        // Take the user's text and convert it to our data type (if we have a matching one).
        e.Item = _categories.FirstOrDefault((item) => item.Name.Contains(e.TokenText, System.StringComparison.CurrentCultureIgnoreCase));
        // Otherwise, create a new version of our data type
        if (e.Item == null)
        {
            e.Cancel = true;
        }
        if (TokenBoxCategories.Items.Contains(e.Item))
        {
            e.Cancel = true;
        }
    }
    private async void OnClearFilterButtonClick(object sender, RoutedEventArgs e)
    {
        await TokenBoxCategories.ClearAsync();
        ViewModel.MinPrice = 0;
        ViewModel.MaxPrice = 0;
        ViewModel.FilterFunc = null;
        ViewModel.CurrentPage = 1;
    }
    private void OnApplyFilterButtonClick(object sender, RoutedEventArgs e)
    {
        ViewModel.FilterFunc = item =>
        {
            if (item is Product product)
            {
                var filterByCategory = TokenBoxCategories.Items.Any(obj => obj is Categories); 
                var filterByPrice = ViewModel.MaxPrice != 0 || ViewModel.MinPrice != 0;

                // Check if user request filter items
                if (filterByCategory || filterByPrice)
                {
                    // Filter by category
                    var isInCategory = TokenBoxCategories.Items.FirstOrDefault((obj) =>
                    {
                        return obj is Categories && (obj as Categories).Id == product.CategoryId;
                    }) != null;

                    // Filter by price
                    var isBetweenPrice = product.Price >= ViewModel.MinPrice && product.Price <= ViewModel.MaxPrice;
                    
                    // Check if user apply for two condition
                    if (filterByCategory && filterByPrice)
                    {
                        return isInCategory && isBetweenPrice;
                    }

                    return isInCategory || isBetweenPrice;
                }
            }
            return true;
        };
        ViewModel.CurrentPage = 1;
    }
    #endregion

    #region AppBarButton
    
    public async void OnImportButtonClick()
    {
        // Create a file picker
        var openPicker = App.MainWindow.CreateOpenFilePicker();

        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.SuggestedStartLocation = PickerLocationId.Downloads;
        openPicker.FileTypeFilter.Add(".xlsx");

        // Open the picker for the user to pick a file
        var file = await openPicker.PickSingleFileAsync();
        if (file != null)
        {
            ContentDialog dialog = new ContentDialog();

            dialog.XamlRoot = this.XamlRoot;
            dialog.Title = "Warning!";
            dialog.PrimaryButtonText = "Yes";
            dialog.CloseButtonText = "No";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = "All products will be overwritten if existed. Are you sure you want to process?";

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.IsLoading = true;
                await _readExcelFile(file.Path);
                ViewModel.IsLoading = false;
                await ShowDialog("Success", "All product import successfully!");
                ViewModel.Refesh();
            }
        }
    }
    private async Task _readExcelFile(string filePath)
    {
        try
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var conf = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };
                    var result = reader.AsDataSet(conf);
                    await _importCategories(result.Tables["Categories"]);
                    await _importProducts(result.Tables["Books"]);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await ShowDialog("Error!", ex.Message);
        }
    }
    private async Task _importCategories(DataTable? categories)
    {
        if (categories == null) return;

        List<Categories> importCategories = new List<Categories>();

        for (var i = 0; i < categories.Rows.Count; i++)
        {
            // 0 - id, 1 - name
            Categories cat = new Categories()
            {
                Id = Convert.ToInt32(categories.Rows[i][0].ToString()),
                Name = categories.Rows[i][1].ToString().Trim()
            };

            importCategories.Add(cat);
        }

        await App.Repository.Categories.UpsertCategoryAsync(importCategories);
    }
    private async Task _importProducts(DataTable? products)
    {
        if (products == null) return;

        List<Product> importProducts = new List<Product>();

        for (var i = 0; i < products.Rows.Count; i++)
        {
            // 0 - id
            // 1 - name
            // 2 - author
            // 3 - image
            // 4 - image_path
            // 5 - description
            // 6 - publisher
            // 7 - published year
            // 8 - price
            // 9 - origin price
            // 10 - discount
            // 11 - quantity
            // 12 - category_id
            Product product = new Product()
            {
                Id = Convert.ToInt32(products.Rows[i][0].ToString()),
                Name = products.Rows[i][1].ToString()!.Trim(),
                Author = products.Rows[i][2].ToString()!.Trim(),
                Image = products.Rows[i][3].ToString()!.Trim(),
                ImagePath = products.Rows[i][4].ToString()?.Trim(),
                Description = products.Rows[i][5].ToString()!.Trim(),
                Publisher = products.Rows[i][6].ToString()!.Trim(),
                PublishedYear = Convert.ToInt32(products.Rows[i][7].ToString()!.Trim()),
                Price = Convert.ToInt32(products.Rows[i][8].ToString()!.Trim()),
                OriginalPrice = Convert.ToInt32(products.Rows[i][9].ToString()!.Trim()),
                Discount = Convert.ToDouble(products.Rows[i][10].ToString()!.Trim()),
                OriginalQuantity = Convert.ToInt32(products.Rows[i][11].ToString()!.Trim()),
                Quantity = Convert.ToInt32(products.Rows[i][11].ToString()!.Trim()),
                CategoryId = Convert.ToInt32(products.Rows[i][12].ToString()!.Trim())
            };

            importProducts.Add(product);
        }

        await App.Repository.Products.UpsertProductAsync(importProducts);
    }

    #endregion
}
