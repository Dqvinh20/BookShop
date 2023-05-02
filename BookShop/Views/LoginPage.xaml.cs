// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using BookShop.ViewModels;
using BookShop.Helpers;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using Windows.System;
using System.Diagnostics;
using Windows.UI.ViewManagement;
using Windows.Foundation;
using System.Configuration;
using System.Text;
using System.Security.Cryptography;
using BookShop.Contracts.Services;
using BookShop.Services;
using BookShop.Core.Models;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BookShop.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LoginPage : Page
{
    public LoginViewModel ViewModel
    {
        get;
    }

    public LoginPage(LoginViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        var themeService = App.GetService<IThemeSelectorService>();
        await themeService.SetRequestedThemeAsync();
    }
        
    private void RevealModeCheckbox_Changed(object sender, RoutedEventArgs e)
    {
        if (revealModeCheckBox.IsChecked == true)
        {
            passworBoxWithRevealmode.PasswordRevealMode = PasswordRevealMode.Visible;
        }
        else
        {
            passworBoxWithRevealmode.PasswordRevealMode = PasswordRevealMode.Hidden;
        }
    }
}
