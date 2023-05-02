using BookShop.Contracts.Services;
using BookShop.Contracts.ViewModels;
using BookShop.Core.Contracts.Services;
using BookShop.Core.Models;

using CommunityToolkit.Mvvm.ComponentModel;

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

    public void OnNavigatedTo(object parameter)
    {
        if (parameter is Product product)
        {
            Item = product;
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
