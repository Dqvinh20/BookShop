using System.Configuration;
using System.Windows.Interop;
using System.Windows.Navigation;
using BookShop.Contracts.Services;
using BookShop.Helpers;
using BookShop.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

using Windows.System;

namespace BookShop.Views;

// TODO: Update NavigationViewItem titles and icons in ShellPage.xaml.
public sealed partial class ShellPage : Page
{
    public ShellViewModel ViewModel
    {
        get;
    }

    public ShellPage(ShellViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
        
        ViewModel.NavigationService.Frame = NavigationFrame;
        ViewModel.NavigationViewService.Initialize(NavigationViewControl);

        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            App.MainWindow.ExtendsContentIntoTitleBar = true;
            App.MainWindow.SetTitleBar(AppTitleBar);
            App.MainWindow.Activated += MainWindow_Activated;
            AppTitleBarText.Text = "AppDisplayName".GetLocalized();
        }
        else
        {
            AppTitleBar.Visibility = Visibility.Collapsed;
        }
    }

    private async void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var themeService = App.GetService<IThemeSelectorService>();
        await themeService.SetRequestedThemeAsync();

        await ViewModel.LoadNavigationHistory();

        App.MainWindow.AppWindow.Closing += OnClosing;
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));
    }

    private async void OnClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        await ViewModel.SaveNavigationHistory();
    }

    private async void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            var resource = args.WindowActivationState == WindowActivationState.Deactivated ? "WindowCaptionForegroundDisabled" : "WindowCaptionForeground";
            AppTitleBarText.Foreground = (SolidColorBrush)App.Current.Resources[resource];
        }
    }

    private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            AppTitleBar.Margin = new Thickness()
            {
                Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
                Top = AppTitleBar.Margin.Top,
                Right = AppTitleBar.Margin.Right,
                Bottom = AppTitleBar.Margin.Bottom
            };
        }
    }

    private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
    {
        var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

        if (modifiers.HasValue)
        {
            keyboardAccelerator.Modifiers = modifiers.Value;
        }

        keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

        return keyboardAccelerator;
    }

    private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        var navigationService = App.GetService<INavigationService>();

        var result = navigationService.GoBack();

        args.Handled = result;
    }

    private void OnSignOutClick(object sender, TappedRoutedEventArgs e)
    {
        var config = ConfigurationManager.OpenExeConfiguration(
                       ConfigurationUserLevel.None);
        config.AppSettings.Settings["Password"].Value = "";
        config.Save(ConfigurationSaveMode.Full);
        ConfigurationManager.RefreshSection("appSettings");

        App.MainWindow.Content = App.GetService<LoginPage>();
    }
}
