using BookShop.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace BookShop.Views;

public sealed partial class HomePage : Page
{
    public ProductsViewModel ViewModel
    {
        get;
    }

    public HomePage()
    {
        ViewModel = App.GetService<ProductsViewModel>();
        InitializeComponent();
    }
}
