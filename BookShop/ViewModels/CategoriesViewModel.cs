using System.Collections.ObjectModel;

using BookShop.Contracts.ViewModels;
using BookShop.Core.Contracts.Services;
using BookShop.Core.Models;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BookShop.ViewModels;

public class CategoriesViewModel : ObservableRecipient, INavigationAware
{
    private readonly ICategoriesService _categoriesService;

    private bool _isBusy;
    public bool IsBusy {
        get => _isBusy; set => SetProperty(ref _isBusy, value);
    }

    public ObservableCollection<Categories> Source { get; } = new ObservableCollection<Categories>();

    public CategoriesViewModel(ICategoriesService categoriesService)
    {
        _categoriesService = categoriesService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();
        // TODO: Replace with real data.
        IsBusy = true;
        var data = await _categoriesService.GetGridDataAsync();
        IsBusy = false;

        foreach (var item in data)
        {
            Source.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
