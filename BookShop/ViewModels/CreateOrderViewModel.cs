using System.Collections.ObjectModel;
using System.Diagnostics;
using BookShop.Contracts.ViewModels;
using BookShop.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using CommunityToolkit.WinUI.UI.Controls;
using CommunityToolkit.WinUI.UI;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace BookShop.ViewModels;

public class CreateOrderViewModel : ObservableRecipient, INavigationAware
{
    private bool _isLoading = false;

    public bool IsLoading
    {
        set => SetProperty(ref _isLoading, value);
        get => _isLoading;
    }
    private IEnumerable<Product> Products
    {
        get; set; 
    }

    public ICommand ItemDeleteCommand
    {
        get;
    }

    public ObservableCollection<InvoiceDetail> ProductsCart 
    { 
        get => _productsCart;
        set {
            SetProperty(ref _productsCart, value);
        } 
    }

    ObservableCollection<InvoiceDetail> _productsCart;

    private AutoSuggestBox? _productSuggestBox = null;
   

    private Product? _suggestedProduct = null;
    private double _currentProductQuantity = 0;

    private InvoiceDetail? _selectedRow = null;

    // Customer Info
    private string? _customerName = string.Empty;
    private string? _phoneNumber = string.Empty;
    private string? _address = string.Empty;

    private long _totalInvoiceMoney = 0;
    public long TotalInvoiceMoney
    {
        get => _totalInvoiceMoney; 
        set => SetProperty(ref _totalInvoiceMoney, value);
    }
    public string? CustomerName
    {
        set => SetProperty(ref _customerName, value); get => _customerName;
    }

    public string? PhoneNumber
    {
        set => SetProperty(ref _phoneNumber, value); get => _phoneNumber;
    }

    public string? Address
    {
        set => SetProperty(ref _address, value); get => _address;
    }

    public Product? SuggestedProduct
    {
        private set
        {
            if (value == null)
            {
                CurrentProductQuantity = 0;
            }
            SetProperty(ref _suggestedProduct, value);
        }
        get => _suggestedProduct;
    }

    public InvoiceDetail? SelectedRow { set => SetProperty(ref _selectedRow, value); get => _selectedRow; }

    public double CurrentProductQuantity
    {
        get => _currentProductQuantity;
        set
        {
            if (value is double.NaN)
            {
                SetProperty(ref _currentProductQuantity,0);
                return;
            }
            SetProperty(ref _currentProductQuantity, Convert.ToInt32(value));
        }
    }
    public CreateOrderViewModel()
    {
        ProductsCart = new ObservableCollection<InvoiceDetail>();
        ItemDeleteCommand = new RelayCommand(OnItemDeleteClick);
    }

    public void OnAddToCartClick(object sender, RoutedEventArgs e) 
    {
        if (SuggestedProduct == null)
        {
            return;
        }
        if (CurrentProductQuantity == 0)
        {
            App.MainWindow.ShowMessageDialogAsync("You need to input product quantity", "Fail on add to cart !");
            return;
        }
        var product = SuggestedProduct as Product;

        if (ProductsCart.Count != 0)
        {
            var result = ProductsCart.Where(p => p.ProductId == product.Id).FirstOrDefault();
            if (result != null)
            {
                App.MainWindow.ShowMessageDialogAsync("Product already in cart", "Fail on add to cart !");
                return;
            }
        }

        InvoiceDetail invoiceDetail = new InvoiceDetail()
        {
            ProductId = product.Id,
            Product = product,
            Quantity = Convert.ToInt64(CurrentProductQuantity),
            UnitSellPrice = product.DiscountPrice,
            TotalMoney = product.DiscountPrice * Convert.ToInt64(CurrentProductQuantity)
        };

        ProductsCart.Add(invoiceDetail);
        TotalInvoiceMoney = ProductsCart.Sum(p => p.TotalMoney);
        if (_productSuggestBox != null) { _productSuggestBox.Text = ""; }
        Debug.WriteLine(ProductsCart.Sum(p => p.TotalMoney));
        SuggestedProduct = null;
    }

    public void OnItemDeleteClick()
    {
        if (SelectedRow != null)
        {
            if (SelectedRow is InvoiceDetail invoiceDetail)
            {
                ProductsCart.Remove(invoiceDetail);
                TotalInvoiceMoney = ProductsCart.Sum(p => p.TotalMoney);
                ProductsCart = new ObservableCollection<InvoiceDetail>(ProductsCart);
            }
        }
    }

    public void OnCellChange(object sender, DataGridCellEditEndedEventArgs e)
    {
        var currEdit = e.Row.DataContext as InvoiceDetail;
        currEdit.TotalMoney = currEdit.Quantity * currEdit.UnitSellPrice;
        TotalInvoiceMoney = ProductsCart.Sum(p => p.TotalMoney);
        Debug.WriteLine(ProductsCart.Sum(p => p.TotalMoney));
        ProductsCart = new ObservableCollection<InvoiceDetail>(ProductsCart);
    }

    public bool IsReadyToAdd(ref string message)
    {
        if (string.IsNullOrEmpty(CustomerName) || string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(Address))
        {
            message = "You need to fill all customer info!";
            return false;
        }

        if (ProductsCart == null || ProductsCart.Count == 0)
        {
            message = "You don't have any product in cart to create invoice!";
            return false;
        }

        return true;
    }

    public async Task CreateInvoice()
    {
        IsLoading = true;
        Customer? customerReturn;
        try
        {
            // Tao customer trc
            Customer customer = new Customer()
            {
                Name = CustomerName,
                PhoneNumber = PhoneNumber,
                Address = Address,
            };
            customerReturn = (await App.Repository.Invoice.CreateOrUpdateCustomerAsync(customer)).FirstOrDefault();
        } catch (Exception ex)
        {
            IsLoading = false;
            await App.MainWindow.ShowMessageDialogAsync("Fail on create new customer", "Error!");
            return;
        }
        try
        {
            // Tao don hang
            Invoice invoice = new Invoice()
            {
                CustomerId = customerReturn!.Id,
            };
            var newInvoice = (await App.Repository.Invoice.UpsertInvoiceAsync(invoice)).FirstOrDefault();

            // Nhap chi tiet don hang
            foreach (var detail in ProductsCart)
            {
                detail.Product = null;
                detail.InvoiceId = newInvoice!.Id;
            }
            await App.Repository.Invoice.AddInvoiceDetailAsync(ProductsCart.ToList());

            ProductsCart.Clear();
            CustomerName = "";
            PhoneNumber = "";
            Address = "";
            await App.MainWindow.ShowMessageDialogAsync("Create new invoice successfully!", "Success");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            IsLoading = false;
            await App.MainWindow.ShowMessageDialogAsync("Fail on create new invoice", "Error!");
            return;
        } finally
        {
            IsLoading = false;
        }
    }

    public async void OnNavigatedTo(object parameter)
    {
        App.SetMinWidthWindow(1100);
        App.SetMinHeightWindow(600);

        Products = (await App.Repository.Products.GetAllProductsAsync()).ToList();

    }
    public void OnNavigatedFrom()
    {
        App.SetMinWidthWindow(500);
        App.SetMinWidthWindow(500);
    }

    #region Search Book Name
    private void SelectBook(string bookName)
    {
        if (bookName != "")
        {
            SuggestedProduct = Products.Where(p => p.Name == bookName).FirstOrDefault();
        }
        else
        {
            SuggestedProduct = null;    
        }
        Debug.WriteLine(SuggestedProduct);
    }

    private List<Product> SearchBook(string query)
    {
        var suggestions = new List<Product>();

        var querySplit = query.Split(" ");

        var matchingItems = Products.Where(
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
        _productSuggestBox = sender;
        if (sender.Text == "" || sender.Text == string.Empty)
        {
            SuggestedProduct = null;
        }
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
            if (args.ChosenSuggestion == "No results found")
            {
                sender.Text = "";
                return;
            }
            //User selected an item, take an action
            SelectBook(args.ChosenSuggestion as string);
        }
        else if (!string.IsNullOrEmpty(args.QueryText))
        {
            sender.Text = "";
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

}
