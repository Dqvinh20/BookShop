using System.Collections.ObjectModel;
using System.Reactive.Linq;
using BookShop.Core.Models;
using BookShop.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BookShop.Views;

public sealed partial class HomePage : Page
{
    //private List<Categories> _categories;

    public ProductsViewModel ViewModel
    {
        get;
    }

    public HomePage()
    {
        ViewModel = App.GetService<ProductsViewModel>();
        InitializeComponent();
        //Loaded += OnLoaded;
    }
    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        //_categories = (List<Categories>) await App.Repository.Categories.GetAllCategoriesAsync();
    }

    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        // Since selecting an item will also change the text,
        // only listen to changes caused by user entering text.
        //if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        //{
        //    var suitableItems = new List<Categories>();
        //    var splitText = sender.Text.ToLower().Split(" ");
        //    foreach (var cat in _categories)
        //    {
        //        var found = splitText.All((key) =>
        //        {
        //            return cat.Name.ToLower().Contains(key);
        //        });
        //        if (found)
        //        {
        //            suitableItems.Add(cat);
        //        }
        //    }
        //    if (suitableItems.Count == 0)
        //    {
        //        suitableItems.Add(new Categories()
        //        {
        //            Name = "No results found"
        //        });
        //    }

        //    sender.ItemsSource = suitableItems;
        //}
    }

    private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        Categories currChosen = (Categories)args.SelectedItem;
        if (currChosen.Name != "No results found")
        {
            _ = DispatcherQueue.TryEnqueue(() =>
            {
                var exist = ViewModel.SelectedCategories.Any(cat => cat.Name == currChosen.Name);
            });
        }
        else
        {
            sender.ItemsSource = ViewModel.SelectedCategories;
        }
    }

    private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {

    }

    
}
