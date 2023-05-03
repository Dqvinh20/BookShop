using System.Diagnostics;
using BookShop.Core.Models;
using BookShop.ViewModels;
using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Controls;

namespace BookShop.Views;

public sealed partial class CreateOrderPage : Page
{
    public CreateOrderViewModel ViewModel
    {
        get;
    }

    public CreateOrderPage()
    {
        ViewModel = App.GetService<CreateOrderViewModel>();
        InitializeComponent();
    }

    private async void OnAddNewInvoice(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        string message = "";
        if (ViewModel.IsReadyToAdd(ref message))
        {
            await ViewModel.CreateInvoice();
        }
        else
        {
            await App.MainWindow.ShowMessageDialogAsync(message, "Fail on create");
        }
    }
}
