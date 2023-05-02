using System.Collections.ObjectModel;
using BookShop.Core.Models;
using BookShop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;

namespace BookShop.ViewModels;

public class DashboardViewModel : ObservableRecipient
{
    private bool _isLoading = false;

    public bool IsLoading
    {
        get => _isLoading; set => SetProperty(ref _isLoading, value);
    }
    public int OutOfStockThreshold
    {
        private set;
        get;
    } = 500;

   
    public ObservableCollection<DashboardItem> SummarySource
    {
        get; set;
    } = new ObservableCollection<DashboardItem>();

    public ObservableCollection<Product> ProductsSource
    {
        get; set;
    } = new ObservableCollection<Product>();

    public DashboardViewModel()
    {
        
    }

    public async Task InitializeAsync()
    {
        IsLoading = true;

        var availableItems = await App.Repository.Products.GetAllProductsAsync();
        var productsCount = availableItems.Sum(i => i.Quantity);
        var categoriesCount = (await App.Repository.Categories.GetAllCategoriesAsync()).Count() - 1; //Remove Unknown Cat
        // TODO: Call api invoices
        var invoicesCount = 500;

        var lists = availableItems.Where(i => i.Quantity < OutOfStockThreshold);

        foreach (var item in lists)
        {
            ProductsSource.Add(item);
        }

        SummarySource.Add(new DashboardItem() { Content = $"{productsCount:n0}", Title = "Total number of products", BackgroundColor = new SolidColorBrush(Colors.Purple) });
        SummarySource.Add(new DashboardItem() { Content = $"{categoriesCount:n0}", Title = "Total number of categories", BackgroundColor = new SolidColorBrush(Colors.HotPink) });
        SummarySource.Add(new DashboardItem() { Content = $"{invoicesCount:n0}", Title = "Total number of invoices", BackgroundColor = new SolidColorBrush(Colors.SkyBlue) });
        SummarySource.Add(new DashboardItem() { Content = $"{availableItems.Count():n0}", Title = "Total available items", BackgroundColor = new SolidColorBrush(Colors.DarkCyan) });
        IsLoading = false;
    }
}
