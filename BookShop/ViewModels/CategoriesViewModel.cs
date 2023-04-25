using System.Collections.ObjectModel;

using BookShop.Contracts.ViewModels;
using BookShop.Core.Contracts.Services;
using BookShop.Core.Models;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BookShop.ViewModels;

public class CategoriesViewModel : ObservableRecipient, INavigationAware
{

    private bool _isBusy;
    public bool IsBusy {
        get => _isBusy; set => SetProperty(ref _isBusy, value);
    }

    public ObservableCollection<Categories> Source { get; } = new ObservableCollection<Categories>();

    public CategoriesViewModel()
    {
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();
        // TODO: Replace with real data.
        IsBusy = true;

        var data = await App.Repository.Categories.GetAllCategoriesAsync();
        foreach (var item in data)
        {
            Source.Add(item);
        }
        IsBusy = false;
    }

    public void OnNavigatedFrom()
    {
    }
}
