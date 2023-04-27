using BookShop.Contracts.ViewModels;
using BookShop.Core.Contracts.Services;
using BookShop.Core.Models;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BookShop.ViewModels;

public class ProductsDetailViewModel : ObservableRecipient, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;
    private Product? _item;

    public Product? Item
    {
        get => _item;
        set => SetProperty(ref _item, value);
    }

    public ProductsDetailViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
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
}
