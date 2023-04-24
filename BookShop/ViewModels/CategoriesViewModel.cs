using System.Collections.ObjectModel;

using BookShop.Contracts.ViewModels;
using BookShop.Core.Contracts.Services;
using BookShop.Core.Models;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BookShop.ViewModels;

public class CategoriesViewModel : ObservableRecipient, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;

    public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

    public CategoriesViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();

        // TODO: Replace with real data.
        var data = await _sampleDataService.GetGridDataAsync();

        foreach (var item in data)
        {
            Source.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
