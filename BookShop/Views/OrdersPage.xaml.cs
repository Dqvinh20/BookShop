using BookShop.Core.Models;
using BookShop.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BookShop.Views;

public sealed partial class OrdersPage : Page
{
    public OrdersViewModel ViewModel
    {
        get;
    }

    public OrdersPage()
    {
        ViewModel = App.GetService<OrdersViewModel>();
        DataContext = this;
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        //for (int i = 0; i < 10; i++)
        //{
        //    await CreateSampleInvoice(i);
        //}
    }

    private async Task CreateSampleInvoice(int i)
    {
        // Tao customer trc
        Customer customer = new Customer()
        {
            Name = "Tran Hong Quan " + i,
            PhoneNumber = i.ToString(),
        };
        var newCustomerInfo = (await App.Repository.Invoice.CreateOrUpdateCustomerAsync(customer)).FirstOrDefault();
        Console.WriteLine(newCustomerInfo);

        // Tao don hang
        Invoice invoice = new Invoice()
        {
            CustomerId = newCustomerInfo!.Id,
        };
        var newInvoice = (await App.Repository.Invoice.UpsertInvoiceAsync(invoice)).FirstOrDefault();
        Console.WriteLine(newInvoice);

        // Nhap chi tiet don hang
        List<InvoiceDetail> invoiceDetails = new List<InvoiceDetail>() {
            new InvoiceDetail()
            {
                ProductId = 1,
                Quantity = 10,
            },
            new InvoiceDetail()
            {
                ProductId = 2,
                Quantity = 10,
            },
            new InvoiceDetail()
            {
                ProductId = 3,
                Quantity = 10,
            },
        };

        foreach (var detail in invoiceDetails)
        {
            detail.InvoiceId = newInvoice!.Id;
        }
        await App.Repository.Invoice.AddInvoiceDetailAsync(invoiceDetails);
    }

    private void clearCurrentOrderButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void createOrderWithCurrentButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void deleteCakeButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void AppBarButton_Drop(object sender, DragEventArgs e)
    {

    }

    
}
