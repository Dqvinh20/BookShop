using BookShop.Core.Contracts.Services;
using BookShop.Core.Models;

namespace BookShop.Core.Api;
public class RestInvoiceRepository : IInvoiceRepository
{
    private readonly HttpHelper _http;
    private readonly string _accessToken;
    private readonly string _controller = "invoice";
    private readonly string _invoice_detail_controller = "invoice_detail";
    private readonly string _customers_controller = "customers";


    public RestInvoiceRepository(string baseUrl, string accessToken)
    {
        _http = new HttpHelper(baseUrl);
        _accessToken = accessToken;
    }

    #region InvoiceDetail
    public async Task<IEnumerable<InvoiceDetail>> AddInvoiceDetailAsync(InvoiceDetail invoiceDetail)
    {
        return await AddInvoiceDetailAsync(new List<InvoiceDetail>() { invoiceDetail });
    }
    public async Task<IEnumerable<InvoiceDetail>> AddInvoiceDetailAsync(List<InvoiceDetail> invoiceDetails)
    {
        List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
        headers.Add(KeyValuePair.Create("Prefer", "return=representation"));
        headers.Add(KeyValuePair.Create("Prefer", "resolution=merge-duplicates"));
        return await _http.PostAsync<List<InvoiceDetail>, IEnumerable<InvoiceDetail>>(_invoice_detail_controller, invoiceDetails, _accessToken, headers);
    }
    public async Task DeleteInvoiceDetailAsync(List<int> id) => throw new NotImplementedException();
    public async Task DeleteInvoiceDetailAsync(int id) => throw new NotImplementedException();

    #endregion

    #region Customer
    public async Task<IEnumerable<Customer>> CreateOrUpdateCustomerAsync(Customer customer)
    {
        List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
        headers.Add(KeyValuePair.Create("Prefer", "return=representation"));
        headers.Add(KeyValuePair.Create("Prefer", "resolution=merge-duplicates"));
        return await _http.PostAsync<Customer, IEnumerable<Customer>>(_customers_controller, customer, _accessToken, headers);
    }

    #endregion
    public async Task<IEnumerable<Invoice>> GetAllInvoiceAsync()
    {
        // Get all relationship
        string controller = $"{_controller}?select=*,customers(*),invoice_detail(*,products(*))";
        return await _http.GetAsync<IEnumerable<Invoice>>(controller, _accessToken);
    }
    public async Task<Invoice> GetInvoiceByIdAsync(int id) => throw new NotImplementedException();
    public async Task<IEnumerable<Invoice>> GetInvoiceWithQueryAsync(string query) => throw new NotImplementedException();
    public async Task<IEnumerable<Invoice>> UpsertInvoiceAsync(Invoice invoice) 
    {
        List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
        headers.Add(KeyValuePair.Create("Prefer", "return=representation"));
        headers.Add(KeyValuePair.Create("Prefer", "resolution=merge-duplicates"));
        return await _http.PostAsync<Invoice, IEnumerable<Invoice>>(_controller, invoice, _accessToken, headers);
    }

    public async Task DeleteInvoiceAsync(int id)
    {
        await _http.DeleteAsync($"{_controller}?id=eq.{id}", _accessToken);
    }
}
