using System.ComponentModel;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using BookShop.Contracts.Services;
using BookShop.Core.Models;
using BookShop.Helpers;
using BookShop.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

namespace BookShop.ViewModels;

public class LoginViewModel : ObservableObject
{
    private Account _account = new();
    private bool _isStayLogged = false;
    private bool _isLoading = false;

    public Account Account
    {
        get => _account;
        set => SetProperty(ref _account, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool IsStayLogged
    {
        get => _isStayLogged;
        set => SetProperty(ref _isStayLogged, value);
    }
    public LoginViewModel()
    {
        LoadFromAppConfig();
    }

    public void LoadFromAppConfig()
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        var isStayedLogged = config.AppSettings.Settings["IsStayLogged"].Value;

        if (isStayedLogged.Length != 0)
        {
            IsStayLogged = Convert.ToBoolean(isStayedLogged.ToString());
        }

        Account = new Account()
        {
            Username = config.AppSettings.Settings["Username"].Value
        };
    }
    public void SaveCredential()
    {
        var config = ConfigurationManager.OpenExeConfiguration(
                        ConfigurationUserLevel.None);

        config.AppSettings.Settings["Username"].Value = Account.Username;
        config.AppSettings.Settings["Password"].Value = Account.Password;
        config.AppSettings.Settings["IsStayLogged"].Value = IsStayLogged.ToString();
        config.Save(ConfigurationSaveMode.Full);
        ConfigurationManager.RefreshSection("appSettings");
    }

    public async Task<bool> CheckAuthenticated()
    {
        var account = (await App.Repository.Accounts.GetAccountByUsernameAsync(Account.Username.Trim())).FirstOrDefault();
        if (account == null)
        {
            await App.MainWindow.ShowMessageDialogAsync("Username or password incorrect!", "Login Fail !");
        }
        else
        {
            bool isVerify = SecurePasswordHelper.Verify(Account.Password.Trim(), account.Password);
            if (isVerify)
            {
                if (IsStayLogged)
                {
                    Account.Password = account.Password;
                }
                else
                {
                    Account.Password = "";
                }

                SaveCredential();
                return true;
            }
            else
            {
                await App.MainWindow.ShowMessageDialogAsync("Username or password incorrect!", "Login Fail !");
                return false;
            }
        }
        return false;
    }

    public async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        Account.Username = "DemoAccount";
        Account.Password = "123456";

        IsLoading = true;
        var isAuth = await CheckAuthenticated();
        IsLoading = false;

        await Task.Delay(400);
        if (isAuth)
        {
            App.MainWindow.Content = App.GetService<ShellPage>();
        }
    }
}
