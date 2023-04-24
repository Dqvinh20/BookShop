// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using BookShop.Contracts.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BookShop.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SplashPage : Page
{
    public SplashPage()
    {
        this.InitializeComponent();
    }

    public SplashPage(ILocalSettingsService s)
    {
        this.InitializeComponent();
    }

    public void OnLoaded(object sender, RoutedEventArgs e)
    {
        _checkLogin();
    }

    private async void _checkLogin()
    {
        //string username = ConfigurationManager.AppSettings["Username"]!;
        //string passwordIn64 = ConfigurationManager.AppSettings["Password"]!;
        //string entropyIn64 = ConfigurationManager.AppSettings["Entropy"]!;
        //string isStayLoggedIn = ConfigurationManager.AppSettings["IsStayLogged"]!;

        //if (isStayLoggedIn == "true")
        //{
        //    byte[] entropyInBytes = Convert.FromBase64String(entropyIn64);
        //    byte[] cypherTextInBytes = Convert.FromBase64String(passwordIn64);

        //    byte[] passwordInBytes = ProtectedData.Unprotect(cypherTextInBytes,
        //        entropyInBytes,
        //        DataProtectionScope.CurrentUser
        //    );

        //    string password = Encoding.UTF8.GetString(passwordInBytes);
        //    _login(username, password);
        //}
    }

    private async void _login(String username,String password)
    {
        bool isSuccess = true;
        if(isSuccess)
        {
            App.MainWindow.Content = App.GetService<ShellPage>();
        }
        else
        {
            App.MainWindow.Content = App.GetService<LoginPage>();
        }
    }
}
