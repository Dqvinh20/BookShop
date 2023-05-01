using BookShop.Contracts.Services;
using BookShop.Helpers;
using BookShop.Views;

using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Navigation;

namespace BookShop.ViewModels;

public class ShellViewModel : ObservableRecipient
{
    private bool _isBackEnabled;
    private object? _selected;
    private string? _selectedPageKey;

    public ILocalSettingsService LocalSettingsService
    {
        get;
    }

    public INavigationService NavigationService
    {
        get;
    }

    public INavigationViewService NavigationViewService
    {
        get;
    }

    public bool IsBackEnabled
    {
        get => _isBackEnabled;
        set => SetProperty(ref _isBackEnabled, value);
    }

    public object? Selected
    {
        get => _selected;
        set => SetProperty(ref _selected, value);
    }

    public ShellViewModel(INavigationService navigationService, INavigationViewService navigationViewService, ILocalSettingsService localSettingsService)
    {
        LocalSettingsService = localSettingsService;
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        NavigationViewService = navigationViewService;
    }

    public async Task SaveNavigationHistory()
    {
        Console.WriteLine(_selectedPageKey);
        await LocalSettingsService.SaveSettingAsync("NavigationState", _selectedPageKey ?? "");
    }

    public async Task LoadNavigationHistory()
    {
        var data = await LocalSettingsService.ReadSettingAsync<string>("NavigationState");

        if (data == string.Empty)
        {
            NavigationService.NavigateTo(typeof(ProductsViewModel).FullName!, null, true);
        }
        else
        {
            NavigationService.NavigateTo(data, null, true);
        }

        await Task.CompletedTask;
    }

    private void OnNavigated(Type sourcePageType)
    {

        IsBackEnabled = NavigationService.CanGoBack;

        if (sourcePageType == typeof(SettingsPage))
        {
            Selected = NavigationViewService.SettingsItem;
            _selectedPageKey = typeof(SettingsViewModel).FullName!;
            return;
        }

        var selectedItem = NavigationViewService.GetSelectedItem(sourcePageType);
        if (selectedItem != null)
        {
            if (selectedItem?.GetValue(NavigationHelper.NavigateToProperty) is string pageKey)
            {
                _selectedPageKey = pageKey;
            }

            Selected = selectedItem;
        }
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;
        OnNavigated(e.SourcePageType);
    }
}
