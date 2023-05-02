using System.Diagnostics;
using BookShop.Contracts.Services;
using BookShop.Contracts.ViewModels;
using BookShop.Core.Contracts.Services;
using BookShop.Core.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;

namespace BookShop.ViewModels;

public class ProductsDetailViewModel : ObservableRecipient, INavigationAware
{
    public readonly INavigationService _navigationService;

    private bool _isLoading = false;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public TextBlock CategoryName
    {
        set; get;
    }

    public ILocalSettingsService LocalSettingsService
    {
        get;
    }
    public INavigationService NavigationService
    {
        get;
    }

    private Product? _item;

    public Product? Item
    {
        get => _item;
        set => SetProperty(ref _item, value);
    }

    public ProductsDetailViewModel(INavigationService navigationService, ILocalSettingsService localSettingsService)
    {
        NavigationService = navigationService;
        LocalSettingsService = localSettingsService;    
    }

    public async void OnNavigatedTo(object parameter)
    {
        Debug.WriteLine("Load");
        if (parameter is Product product)
        {
            Item = product;
            if (Item.Category == null)
            {
                var item = await App.Repository.Products.GetProductByIdAsync((int)Item.Id);
                Item = item;
                CategoryName.Text = Item.Category.Name;
            }
        }

    }

    public void OnNavigatedFrom()
    {
    }
    public async Task OnDeleteProduct()
    {
        await App.Repository.Products.DeleteProductAsync((int)Item.Id);
    }
    public void OnEditProductClick()
    {
        NavigationService.NavigateTo(typeof(UpsertProductViewModel).FullName!, Item);
    }
}
