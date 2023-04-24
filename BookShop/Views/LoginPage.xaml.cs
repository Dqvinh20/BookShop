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
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Username = "123"; 
        //ViewModel.Username = await _localSettingsService.ReadSettingAsync<String>("username");
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

    private void StayLoggedCheckbox_Changed(object sender, RoutedEventArgs e)
    {
        
    }

    private bool _login()
    {
        return true;
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        //var config = ConfigurationManager.OpenExeConfiguration(
        //                ConfigurationUserLevel.None);
        
        //Tuple<String, String> result = _encryptPassword();
        //config.AppSettings.Settings["Username"].Value = ViewModel.Username;
        //config.AppSettings.Settings["Password"].Value = result.Item1;
        //config.AppSettings.Settings["Entropy"].Value = result.Item2;
        //config.AppSettings.Settings["IsStayLogged"].Value = stayLoggedIn.IsChecked == true ? "true" : "false";
        //config.Save(ConfigurationSaveMode.Full);
        //ConfigurationManager.RefreshSection("appSettings");
    }

    private Tuple<String, String> _encryptPassword()
    {
        var passwordInBytes = Encoding.UTF8.GetBytes(ViewModel.Password);
        var entropy = new byte[20];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(entropy);
        }

        var cypherText = ProtectedData.Protect(
            passwordInBytes,
            entropy,
            DataProtectionScope.CurrentUser
        );

        var passwordIn64 = Convert.ToBase64String(cypherText);
        var entropyIn64 = Convert.ToBase64String(entropy);
        
        return Tuple.Create(passwordIn64, entropyIn64);
    }
   
}
