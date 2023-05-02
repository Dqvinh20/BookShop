// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using BookShop.ViewModels;
using BookShop.Core.Models;
using BookShop.Contracts.Services;
using Windows.Storage.Pickers;
using Windows.Globalization.NumberFormatting;
using Windows.Globalization;
using Newtonsoft.Json.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BookShop.Views;

/// <summary>
/// Add new product page
/// </summary>
public sealed partial class UpsertProductPage : Page
{
    private List<Categories>? _categories;
    public UpsertProductViewModel ViewModel { get; }

    public UpsertProductPage()
    {
        ViewModel = App.GetService<UpsertProductViewModel>();
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        _categories = (List<Categories>) await App.Repository.Categories.GetAllCategoriesAsync();
        SetNumberBoxNumberFormatter();
    }

    private void SetNumberBoxNumberFormatter()
    {
        discountNumberBox.NumberFormatter = new PercentFormatter()
        {
            FractionDigits = 0,
            IntegerDigits = 1,
        };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {

        base.OnNavigatedTo(e);
        InitializeEditData();


    }
    public async void InitializeEditData()
    {
             
        if(ViewModel.Item.Name != "")
        {

            categoryBox.Text = ViewModel.Item.Category!.Name;

            orgPriceNumberBox.IsEnabled = false;
            orgQuantityNumberBox.IsEnabled = false;
            ViewModel.ImagePreview = ViewModel.Item.Image;
            button.Content = "Save";
        }
    }
    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        base.OnNavigatingFrom(e);
        if (e.NavigationMode == NavigationMode.Back)
        {
            var navigationService = App.GetService<INavigationService>();

            if (ViewModel.Item != null)
            {
                navigationService.SetListDataItemForNextConnectedAnimation(ViewModel.Item);
            }
        }
    }

    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        // Since selecting an item will also change the text,
        // only listen to changes caused by user entering text.
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            var suitableItems = new List<string>();
            var splitText = sender.Text.ToLower().Split(" ");
            foreach (var cat in _categories)
            {
                var found = splitText.All((key) =>
                {
                    return cat.Name.ToLower().Contains(key);
                });
                if (found)
                {
                    suitableItems.Add(cat.Name);
                }
            }
            if (suitableItems.Count == 0)
            {
                suitableItems.Add("No results found");
            }
            sender.ItemsSource = suitableItems;
        }
    }

    private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem.ToString() != "No results found")
        {
            _ = DispatcherQueue.TryEnqueue(() =>
            {
                int? id = _categories.Find(cat => cat.Name == args.SelectedItem.ToString()).Id;
                ViewModel.Item.CategoryId = id ?? -1;
            });
        }
        else
        {
            sender.Text = "";
            ViewModel.Item.CategoryId = -1;
        }
    }

    private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion != null)
        {
            int? id = _categories.Find(cat => cat.Name == args.ChosenSuggestion.ToString()).Id;
            ViewModel.Item.CategoryId = id ?? -1;
        }
        else
        {
            sender.Text = "";
            ViewModel.Item.CategoryId = -1;
        }
    }

    private async void OnButtonClick(object sender, RoutedEventArgs e)
    {
        string message = string.Empty;
        if (!ViewModel.ValidateField(ref message))
        {
            await App.MainWindow.ShowMessageDialogAsync(message, "Missing Fields");
            return;
        }

        ViewModel.IsLoading = true;
        string image = ViewModel.ImagePreview;

        try
        {
            var result = await OnCreateOrUpdateProduct();
            ViewModel.IsLoading = false;

            if (button.Content == "Add")
            {
                if (result != null)
                {
                    ViewModel.Item = result;
                    await App.MainWindow.ShowMessageDialogAsync("Add product successfully", "Success");
                }
                else
                {
                    await App.MainWindow.ShowMessageDialogAsync("Add product unsuccessfully", "Fail");
                }
            }
            else
            {
                if (result != null)
                {
                    ViewModel.Item = result;
                    await App.MainWindow.ShowMessageDialogAsync("Update product successfully", "Success");
                }
                else
                {
                    await App.MainWindow.ShowMessageDialogAsync("Update product unsuccessfully", "Fail");
                }
            }
        }
        catch (HttpRequestException httpEx)
        {
            await App.MainWindow.ShowMessageDialogAsync("Please check your internet connection!", "Unexpected Error!");
            ViewModel.Item.Image = image;
            await App.Repository.Storage.DeleteImageAsync(ViewModel.Item.ImagePath);
        }
        catch (Exception ex)
        {
            await App.MainWindow.ShowMessageDialogAsync(ex.Message, "Unexpected Error!");
            ViewModel.Item.Image = image;
            await App.Repository.Storage.DeleteImageAsync(ViewModel.Item.ImagePath);
        }
        finally
        {
            ViewModel.IsLoading = false;
        }

    }

    private async Task<Product> OnCreateOrUpdateProduct()
    {
        if (ViewModel.ImagePreview != ViewModel.Item.Image)
        {
            string webUrl = await App.Repository.Storage.UploadImageAsync(ViewModel.ImagePreview, ViewModel.Item.ImagePath);
            ViewModel.Item.Image = webUrl;
        }

        ViewModel.Item.Quantity = ViewModel.Item.OriginalQuantity;
        ViewModel.Item.Category = null;
        var result = await App.Repository.Products.UpsertProductAsync(ViewModel.Item);

        return result.FirstOrDefault();
    }

    private async void OnSelectImageClick(object sender, RoutedEventArgs e)
    {
        // Create a file picker
        var openPicker = App.MainWindow.CreateOpenFilePicker();

        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.SuggestedStartLocation = PickerLocationId.Downloads;
        openPicker.FileTypeFilter.Add(".jpg");
        openPicker.FileTypeFilter.Add(".jpeg");
        openPicker.FileTypeFilter.Add(".png");

        // Open the picker for the user to pick a file
        var file = await openPicker.PickSingleFileAsync();
        if (file != null)
        {
            
            ViewModel.ImagePreview = file.Path;
            ViewModel.Item.ImagePath = file.Name;
        }
    }
}
