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

    private void addBookButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void bookComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void shipipngFeeTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void shipToggleButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void shipipngFeeTextBox_LostFocus(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void cancelOrderButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void finishOrderButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }
}
