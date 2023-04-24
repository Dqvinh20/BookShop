using BookShop.ViewModels;

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
}
