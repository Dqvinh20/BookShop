
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

using BookShop.Contracts.Services;
using BookShop.Contracts.ViewModels;
using BookShop.Core.Contracts.Services;
using BookShop.Core.Models;
using BookShop.Views;
using CommunityToolkit.Common.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI;
using ExcelDataReader;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Storage.Pickers;

namespace BookShop.ViewModels;

public class ProductsViewModel : ObservableRecipient, INavigationAware
{
    #region Declare Variable
    private readonly INavigationService _navigationService;
    public ObservableCollection<Categories> SelectedCategories { private set; get; } = new ObservableCollection<Categories>();
    public ObservableCollection<Product>? OriginItem { private set; get; } = null;
    public AdvancedCollectionView FilteredItems { private set; get; }

    private bool _isLoading = false;

    // Paging info
    private int _currentPage = 1;
    private int _itemsPerPage = 5;
    private int _maxPage = 1;
    private int _totalItems = 0;

    // Filter info
    private bool _hasResults = true;
    private string _search = "";
    private Func<Product, bool>? _filterFunc = null;
    private double _minPrice = 0;
    private double _maxPrice = 0;

    public bool IsLoading
    {
        get => _isLoading; set => SetProperty(ref _isLoading, value);
    }

    public ICommand ItemClickCommand{ get; }

    public Func<Product, bool> FilterFunc
    {
        set {
            SetProperty(ref _filterFunc, value);
        }
        get => _filterFunc;
    }

    public int ItemPerPage
    {
        get => _itemsPerPage;
        set
        {
            SetProperty(ref _itemsPerPage, (int) value);
            Refesh();
        }
    }

    public int CurrentPage
    {
        get => _currentPage;
        set 
        {
            SetProperty(ref _currentPage, value);
            Refesh();
        }
    }

    public int MaxPage
    {
        get => _maxPage;
        set => SetProperty(ref _maxPage, value);
    }
   
    public double MinPrice
    {
        set {
            if (double.IsNaN(value))
            {
                SetProperty(ref _minPrice, 0);
                return;
            }
            SetProperty(ref _minPrice, (int) value);
        }
        get
        {
            return _minPrice;
        }
    }
    public double MaxPrice
    {
        set
        {
            if (double.IsNaN(value))
            {
                SetProperty(ref _maxPrice, 0);
                return; 
            }

            SetProperty(ref _maxPrice, (int) value);
        }
        get
        {
            return _maxPrice;
        }
    }

    public bool HasResult
    {
        get => _hasResults;
        set => SetProperty(ref _hasResults, value);
    }

    #endregion

    public ProductsViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        FilteredItems = new AdvancedCollectionView(OriginItem, true);
        FilteredItems.SortDescriptions.Add(new SortDescription(nameof(Product.Name), SortDirection.Ascending));
        ItemClickCommand = new RelayCommand<Product>(OnItemClick);
    }

    public async Task Refesh(bool isFetchNewData = false)
    {
        if (isFetchNewData)
        {
            OriginItem = new ObservableCollection<Product>(await App.Repository.Products.GetAllProductsAsync());
        }

        _updateDataSource(_currentPage);
        _updatePagingInfo();

    }

    public async void OnNavigatedTo(object parameter)
    {
        IsLoading = true;
        OriginItem ??= new ObservableCollection<Product>(await App.Repository.Products.GetAllProductsAsync());
        CurrentPage = 1;
        IsLoading = false;
    }
    public void OnNavigatedFrom()
    {
    }
    private void OnItemClick(Product? clickedItem)
    {
        if (clickedItem != null)
        {
            _navigationService.SetListDataItemForNextConnectedAnimation(clickedItem);
            _navigationService.NavigateTo(typeof(ProductsDetailViewModel).FullName!, clickedItem);
        }
    }

    #region Paging Data
    private Tuple<List<Product>, int> _pagingData(int page = 1, int itemPerPage = 5, string keyword = "", Func<Product, bool> filter = null)
    {
        var filterList = OriginItem.Where(item =>
        {
            var splitKeyword = keyword.Split(" ");
            bool flag = true;
            foreach (string queryToken in splitKeyword)
            {
                // Check if token is not in string 
                if (item.Name.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                {
                    // Token is not in string, so we ignore this item. 
                    flag = false;
                }
            }
            return flag;
        });
        if (filter != null)
        {
            filterList = filterList.Where(filter);
        }
        var result = filterList.Skip((page - 1) * itemPerPage).Take(itemPerPage);
        return Tuple.Create(result.ToList(), filterList.Count());
    }

    private void _updateDataSource(int page = 1)
    {
        (var items, _totalItems) = _pagingData(page, _itemsPerPage, _search, FilterFunc);
        FilteredItems.Source = items;
        FilteredItems.Refresh();
        HasResult = items.Count != 0;
    }

    private void _updatePagingInfo()
    {
        MaxPage = _totalItems / ItemPerPage + (_totalItems % ItemPerPage == 0 ? 0 : 1);
    }

    #endregion

    #region Search Book Name
    private void SelectBook(string bookName)
    {
        _search = bookName;
        CurrentPage = 1;
    }

    private List<Product> SearchBook(string query)
    {
        var suggestions = new List<Product>();

        var querySplit = query.Split(" ");
       
        var matchingItems = OriginItem.Where(
            item =>
            {
                // Idea: check for every word entered (separated by space) if it is in the name,  
                // e.g. for query "split button" the only result should "SplitButton" since its the only query to contain "split" and "button" 
                // If any of the sub tokens is not in the string, we ignore the item. So the search gets more precise with more words 
                bool flag = true;
                foreach (string queryToken in querySplit)
                {
                    // Check if token is not in string 
                    if (item.Name.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                    {
                        // Token is not in string, so we ignore this item. 
                        flag = false;
                    }
                }
                return flag;
            });

        foreach (var item in matchingItems)
        {
            suggestions.Add(item);
        }
        return suggestions.OrderByDescending(i => i.Name.StartsWith(query, StringComparison.CurrentCultureIgnoreCase)).ThenBy(i => i.Name).ToList();
    }

    public void BookSearch_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            var suggestions = SearchBook(sender.Text);

            if (suggestions.Count > 0)
                sender.ItemsSource = suggestions.Select(p => p.Name);
            else
                sender.ItemsSource = new string[] { "No results found" };
        }
    }
    public void BookSearch_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion != null && args.ChosenSuggestion is string)
        {
            //User selected an item, take an action
            SelectBook(args.ChosenSuggestion as string);
        }
        else if (!string.IsNullOrEmpty(args.QueryText))
        {
            //Do a fuzzy search based on the text
            SelectBook(sender.Text);
        }
    }
    public void BookSearch_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        // Don't autocomplete the TextBox when we are showing "no results"
        string selected = (string)args.SelectedItem;
        if (!selected.Equals("No results found"))
        {
            sender.Text = selected;
        }
    }
   
    #endregion
    public void OnAddProductClick()
    {
        _navigationService.NavigateTo(typeof(UpsertProductViewModel).FullName!, OriginItem.ToList<Product>());
    }
}
