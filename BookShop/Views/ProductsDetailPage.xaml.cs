using System.Diagnostics;
using BookShop.Contracts.Services;
using BookShop.Core.Models;
using BookShop.Services;
using BookShop.ViewModels;

using CommunityToolkit.WinUI.UI.Animations;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Navigation;

namespace BookShop.Views;

public sealed partial class ProductsDetailPage : Page
{
    public ProductsDetailViewModel ViewModel
    {
        get;
    }

    
    
    public ProductsDetailPage()
    {
        ViewModel = App.GetService<ProductsDetailViewModel>();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        ViewModel.CategoryName = categoryName;
        base.OnNavigatedTo(e);
        this.RegisterElementForConnectedAnimation("animationKeyContentGrid", itemHero);
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        base.OnNavigatingFrom(e);
        if (e.NavigationMode == NavigationMode.Back)
        {
            var navigationService = App.GetService<INavigationService>();

            if (ViewModel.Item != null)
            {
                navigationService.SetListDataItemForNextConnectedAnimation(ViewModel.Item);
            }
        }
    }

    public async void ShowDialog_Click(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog = new ContentDialog();

        dialog.XamlRoot = this.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = "Delete this book";
        dialog.Content = "Are you sure you want to delete it?";
        dialog.PrimaryButtonText = "Yes, delete it";
        dialog.CloseButtonText = "Cancel";
        dialog.DefaultButton = ContentDialogButton.Primary;
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            ViewModel.IsLoading = true;

            try
            {
                await ViewModel.OnDeleteProduct();
                ViewModel.NavigationService.GoBack();
            }
            catch(HttpRequestException ex)
            {
                await App.MainWindow.ShowMessageDialogAsync("Please check your internet connection!", "Delete Product Error");
            }
            finally
            {
                ViewModel.IsLoading = false;

            }
        } 

    }
    public void EditProductClick(object sender, RoutedEventArgs e)
    {
        ViewModel.OnEditProductClick();
    }
}
