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
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        var data = await App.Repository.Invoice.GetAllInvoiceAsync();
        foreach (var item in data)
        {
            foreach(var detail in item.InvoiceDetails)
            {
                Console.WriteLine(detail.Product);
            }
        }
    }

    private async Task CreateSampleInvoice()
    {
        // Tao customer trc
        Customer customer = new Customer()
        {
            Name = "Tran Hong Quan",
            PhoneNumber = "1234567890",
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
}
