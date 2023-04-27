// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using BookShop.ViewModels;
using BookShop.Core.Models;
using System.Security.Policy;
using BookShop.Contracts.Services;
using CommunityToolkit.WinUI.UI.Animations;
using Windows.Storage.Pickers;
using Windows.Globalization.NumberFormatting;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BookShop.Views;

/// <summary>
/// Add new product page
/// </summary>
public sealed partial class AddProductPage : Page
{
    private List<Categories> _categories;
    public AddProductViewModel ViewModel { get; }

    public AddProductPage()
    {
        ViewModel = App.GetService<AddProductViewModel>();
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        _categories = (List<Categories>) await App.Repository.Categories.GetAllCategoriesAsync();
    }

    private void SetNumberBoxNumberFormatter()
    {
        IncrementNumberRounder rounder = new IncrementNumberRounder();
        rounder.Increment = 0.25;
        rounder.RoundingAlgorithm = RoundingAlgorithm.RoundUp;

        DecimalFormatter formatter = new DecimalFormatter();
        formatter.IntegerDigits = 1;
        formatter.FractionDigits = 2;
        formatter.NumberRounder = rounder;

        foreach(var numberBox in productForm.Children)
        {
            if (numberBox is NumberBox ins)
            {
                ins.NumberFormatter = formatter;
            }
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
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
            ViewModel.Item.CategoryId = -1;
        }
    }

    private async void _showInfoDialog(string title, string message)
    {
        ContentDialog dialog = new ContentDialog();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = this.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = title;
        dialog.PrimaryButtonText = "OK";
        dialog.DefaultButton = ContentDialogButton.Primary;
        dialog.Content = message;

        var result = await dialog.ShowAsync();
    }


    private async void OnAddProductClick(object sender, RoutedEventArgs e)
    {
        string message = string.Empty;
        if (!ViewModel.ValidateField(ref message))
        {
            _showInfoDialog("Missing Fields", message);
            return;
        }

        string image = "";
        try
        {
            ViewModel.IsLoading = true;
            
            string webUrl = await App.Repository.Storage.UploadImageAsync(ViewModel.Item.Image, ViewModel.Item.ImagePath);
            image = ViewModel.Item.Image;

            ViewModel.Item.Image = webUrl;
            ViewModel.Item.Quantity = ViewModel.Item.OriginalQuantity;

            var result = await App.Repository.Products.UpsertProductAsync(ViewModel.Item);
            ViewModel.IsLoading = false;

            if (result.ToList().Count != 0)
            {
                _showInfoDialog("Success", "Add product successfully");
                foreach (var data in result)
                {
                    Console.WriteLine(data.Name);
                }
            }
            else
            {
                _showInfoDialog("Fail", "Add product unsuccessfully");
            }
        } catch (Exception ex)
        {
            ViewModel.IsLoading = false;
            _showInfoDialog("Error", ex.Message);
            ViewModel.Item.Image = image;
            App.Repository.Storage.DeleteImageAsync(ViewModel.Item.ImagePath);
        }
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
