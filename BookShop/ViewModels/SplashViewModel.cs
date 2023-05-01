using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Contracts.Services;
using BookShop.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;

namespace BookShop.ViewModels;
public class SplashViewModel : ObservableRecipient
{
    public SplashViewModel()
    {
    }

    public async Task CheckCredential()
    {
        var config = ConfigurationManager.OpenExeConfiguration(
                        ConfigurationUserLevel.None);

        var username = config.AppSettings.Settings["Username"].Value!;
        var passwordIn64 = config.AppSettings.Settings["Password"].Value!;
        var isStayLogged = config.AppSettings.Settings["IsStayLogged"].Value!;
        await Task.Delay(1000);

        if (username.Length == 0 || passwordIn64.Length == 0)
        {
            _goToLoginPage();
            return;
        }

        if (isStayLogged.Length != 0 && Convert.ToBoolean(isStayLogged))
        {
            var account = (await App.Repository.Accounts.GetAccountByUsernameAsync(username)).FirstOrDefault();

            if (account != null && account.Password == passwordIn64)
            {
                _goToShellPage();
            }
            else
            {
                _goToLoginPage();
            }
        }

        await Task.CompletedTask;
    }
    private void _goToLoginPage()
    {
        App.MainWindow.Content = App.GetService<LoginPage>();
    }
    private void _goToShellPage()
    {
        App.MainWindow.Content = App.GetService<ShellPage>();
    }
}
