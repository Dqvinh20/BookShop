using System.ComponentModel;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using BookShop.Contracts.Services;
using BookShop.Core.Models;
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
        var account = (await App.Repository.Accounts.GetAccountByUsernameAsync(Account.Username)).FirstOrDefault();
        if (account == null)
        {
            await App.MainWindow.ShowMessageDialogAsync("Username or password incorrect!", "Login Fail !");
        }
        else
        {
            string decryptPassword = _decryptPassword(account.Password, account.Entropy);
            if (Account.Password == decryptPassword)
            {
                if (IsStayLogged)
                {
                    Account.Password = account.Password;
                    Account.Entropy = account.Entropy;
                    SaveCredential();
                }
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
        // ViewModel.Account.Username = "DemoAccount";
        // ViewModel.Account.Password = "123456";

        IsLoading = true;
        var isAuth = await CheckAuthenticated();
        IsLoading = false;

        await Task.Delay(400);
        if (isAuth)
        {
            App.MainWindow.Content = App.GetService<ShellPage>();
        }
    }

    private string _decryptPassword(string passwordIn64, string entropyIn64)
    {
        try
        {
            if (passwordIn64.Length != 0)
            {
                byte[] entropyInBytes = Convert.FromBase64String(entropyIn64);
                byte[] cypherTextInBytes = Convert.FromBase64String(passwordIn64);

                byte[] passwordInBytes = ProtectedData.Unprotect(
                    cypherTextInBytes,
                    entropyInBytes,
                    DataProtectionScope.CurrentUser
                );

                string password = Encoding.UTF8.GetString(passwordInBytes);
                return password;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
        }
        return string.Empty;
    }

    private string _encryptPassword(string password, string entropyIn64)
    {
        var passwordInBytes = Encoding.UTF8.GetBytes(password);
        byte[] entropyInBytes = Convert.FromBase64String(entropyIn64);

        var cypherText = ProtectedData.Protect(
            passwordInBytes,
            entropyInBytes,
            DataProtectionScope.CurrentUser
        );

        var passwordIn64 = Convert.ToBase64String(cypherText);

        return passwordIn64;
    }
}
