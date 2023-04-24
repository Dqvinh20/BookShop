using BookShop.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace BookShop.Views;

public sealed partial class StatisticsPage : Page
{
    public StatisticsViewModel ViewModel
    {
        get;
    }

    public StatisticsPage()
    {
        ViewModel = App.GetService<StatisticsViewModel>();
        InitializeComponent();
    }
}
