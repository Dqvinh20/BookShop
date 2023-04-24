using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.Hosting;

namespace BookShop.ViewModels;

public class LoginViewModel : ObservableObject
{
    private String? _userName = "";

    private String? _password = "";

    public String? Username {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }
    public String? Password { get => _password; set => SetProperty(ref _password, value); }

    public LoginViewModel()
    {
    }
}
