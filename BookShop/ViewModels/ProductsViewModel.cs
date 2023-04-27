using System.Collections.ObjectModel;
using System.Windows.Input;

using BookShop.Contracts.Services;
using BookShop.Contracts.ViewModels;
using BookShop.Core.Contracts.Services;
using BookShop.Core.Models;
using BookShop.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;

namespace BookShop.ViewModels;

public class ProductsViewModel : ObservableRecipient, INavigationAware
{
    private readonly INavigationService _navigationService;
    private readonly ISampleDataService _sampleDataService;

    public ICommand ItemClickCommand
    {
        get;
    }
    public ObservableCollection<Product> Source { get; } = new ObservableCollection<Product>();
    public ObservableCollection<Product> FilteredItem { private set; get; } = new ObservableCollection<Product>();
    public ObservableCollection<Categories> SelectedCategories { private set; get; } = new ObservableCollection<Categories>();
    public ProductsViewModel(INavigationService navigationService, ISampleDataService sampleDataService)
    {
        _navigationService = navigationService;
        _sampleDataService = sampleDataService;

        ItemClickCommand = new RelayCommand<Product>(OnItemClick);
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();

        // TODO: Replace with real data.
        var data = await App.Repository.Products.GetAllProductsAsync();
        foreach (var item in data)
        {
            Source.Add(item);
            FilteredItem.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }

    private void OnItemClick(Product? clickedItem)
    {
        if (clickedItem != null)
        {
            _navigationService.SetListDataItemForNextConnectedAnimation(clickedItem);
            _navigationService.NavigateTo(typeof(ProductsDetailViewModel).FullName!, clickedItem);
        }
    }

    public void OnAddProductClick()
    {
        _navigationService.NavigateTo(typeof(AddProductViewModel).FullName!, Source.ToList<Product>());
    }
    public async void OnImportButtonClick()
    {
        // Create a file picker
        var openPicker = App.MainWindow.CreateOpenFilePicker();

        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.SuggestedStartLocation = PickerLocationId.Downloads;
        openPicker.FileTypeFilter.Add(".csv");

        // Open the picker for the user to pick a file
        var file = await openPicker.PickSingleFileAsync();
        if (file != null)
        {
            Console.WriteLine(file.Path);
        }
    }

}
