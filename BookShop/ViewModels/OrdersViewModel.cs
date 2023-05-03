using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using BookShop.Contracts.ViewModels;
using BookShop.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BookShop.ViewModels;

public class OrdersViewModel : ObservableRecipient, INavigationAware
{
    public ObservableCollection<Invoice>? OriginalInvoices { private set; get; } = null;

    public AdvancedCollectionView FilteredItems
    {
        private set; get;
    }

    private object? _selectedRow = null;

    public object? SelectedRow
    {
        get => _selectedRow;
        set => SetProperty(ref _selectedRow, value);
    }

    public ICommand ItemDeleteCommand
    {
        get;
    }
    // Paging info
    private int _currentPage = 1;
    private int _itemsPerPage = 5;
    private int _maxPage = 1;
    private int _totalItems = 0;

    // Filter info
    private Func<Invoice, bool>? _filterFunc = null;
    private DateTimeOffset? _fromDate = DateTime.Now.Date;
    private DateTimeOffset? _toDate = DateTime.Now.Date;

    public DateTimeOffset? CurrentDate { get; } = DateTime.Now.Date;

    private bool _isLoading = false;

    public bool IsLoading
    {
        get => _isLoading; set => SetProperty(ref _isLoading, value);
    }

    public int ItemPerPage
    {
        get => _itemsPerPage;
        set
        {
            SetProperty(ref _itemsPerPage, (int)value);
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

    public Func<Invoice, bool> FilterFunc
    {
        set
        {
            SetProperty(ref _filterFunc, value);
        }
        get => _filterFunc;
    }

    public DateTimeOffset? FromDate
    {
        set
        {
            SetProperty(ref _fromDate, value);
        }
        get
        {
            return _fromDate;
        }
    }
    public DateTimeOffset? ToDate
    {
        set
        {
            SetProperty(ref _toDate, value);
        }
        get
        {
            return _toDate;
        }
    }

    public OrdersViewModel()
    {
        FilteredItems = new AdvancedCollectionView(OriginalInvoices, true);
        ItemDeleteCommand = new AsyncRelayCommand(OnItemDeleteClick);  
        //FilteredItems.SortDescriptions.Add(new SortDescription(nameof(Invoice.CreatedAt), SortDirection.Descending));
    }

    public async void OnMenuFlyoutItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item)
        {
            if (item.Tag.ToString() == "apply")
            {
                FilterFunc = (invoice) =>
                {
                    return invoice.CreatedAt >= FromDate && invoice.CreatedAt <= ToDate;
                };
            }
            else
            {
                FilterFunc = null;
            }
            await Refesh();
        }
    }

    private async Task OnItemDeleteClick()
    {
        if (SelectedRow != null)
        {
            if (SelectedRow is Invoice invoice)
            {
                try
                {
                    await App.Repository.Invoice.DeleteInvoiceAsync((int) invoice.Id!);
                } catch (HttpRequestException httpEx)
                {
                    await App.MainWindow.ShowMessageDialogAsync("Please check your internet connection!", "Unexpected Error!");
                } catch (Exception ex)
                {
                    await App.MainWindow.ShowMessageDialogAsync(ex.Message, "Unexpected Error!");
                }
            }
        }
        await Refesh(true);
    }

    public async Task Refesh(bool isFetchNewData = false)
    {
        if (isFetchNewData)
        {
            OriginalInvoices = new ObservableCollection<Invoice>(await App.Repository.Invoice.GetAllInvoiceAsync());
        }

        _updateDataSource(_currentPage);
        _updatePagingInfo();
    }

    #region Paging Data
    private Tuple<List<Invoice>, int> _pagingData(int page = 1, int itemPerPage = 5, Func<Invoice, bool> filter = null)
    {
        var filterList = OriginalInvoices.AsEnumerable();
        if (filter != null)
        {
            filterList = filterList.Where(filter);
        }
        var result = filterList.Skip((page - 1) * itemPerPage).Take(itemPerPage);
        return Tuple.Create(result.ToList(), filterList.Count());
    }

    private void _updateDataSource(int page = 1)
    {
        (var items, _totalItems) = _pagingData(page, _itemsPerPage, FilterFunc);
        FilteredItems.Source = items;
        FilteredItems.Refresh();
    }

    private void _updatePagingInfo()
    {
        MaxPage = _totalItems / ItemPerPage + (_totalItems % ItemPerPage == 0 ? 0 : 1);
        if (CurrentPage > MaxPage && _totalItems != 0) { CurrentPage = 1; }
    }

    #endregion
    private void _setMinWidthWindow(int width)
    {
        var manager = WinUIEx.WindowManager.Get(App.MainWindow);
        manager.MinWidth = width;
    }

    public async void OnNavigatedTo(object parameter)
    {
        _setMinWidthWindow(800);
        IsLoading = true;
        var invoices = await App.Repository.Invoice.GetAllInvoiceAsync();
        OriginalInvoices ??= new ObservableCollection<Invoice>(invoices);
        CurrentPage = 1;
        IsLoading = false;
    }
    public void OnNavigatedFrom()
    {
        _setMinWidthWindow(500);

    }
}
