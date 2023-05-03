using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Core.Models;

namespace BookShop.Core.Contracts.Services;
public interface IInvoiceRepository
{
    /// <summary>
    /// Returns all invoice. 
    /// </summary>
    Task<IEnumerable<Invoice>> GetAllInvoiceAsync();

    /// <summary>
    /// Returns all invoice with a data field matching the start of the given string. 
    /// </summary>
    Task<IEnumerable<Invoice>> GetInvoiceWithQueryAsync(string query);

    /// <summary>
    /// Returns the invoice with the given id. 
    /// </summary>
    Task<Invoice> GetInvoiceByIdAsync(int id);

    /// <summary>
    /// Adds a new invoice if the invoice does not exist, updates the 
    /// existing invoice otherwise.
    /// </summary>
    Task<IEnumerable<Invoice>> UpsertInvoiceAsync(Invoice invoice);

    /// <summary>
    /// Deletes a invoice.
    /// </summary>
    Task DeleteInvoiceAsync(int id);

    #region InvoiceDetail
    /// <summary>
    /// Adds a new invoice detail if the invoice does not exist, updates the 
    /// existing invoice otherwise.
    /// </summary>
    Task<IEnumerable<InvoiceDetail>> AddInvoiceDetailAsync(InvoiceDetail invoiceDetail);

    /// <summary>
    /// Adds a list invoice detail if the invoice detail does not exist, updates the 
    /// existing invoice detail otherwise.
    /// </summary>
    Task<IEnumerable<InvoiceDetail>> AddInvoiceDetailAsync(List<InvoiceDetail> invoiceDetails);

    /// <summary>
    /// Deletes a invoice detail.
    /// </summary>
    Task DeleteInvoiceDetailAsync(int id);

    /// <summary>
    /// Deletes a list invoice detail.
    /// </summary>
    Task DeleteInvoiceDetailAsync(List<int> id);

    #endregion

    #region Customer

    /// <summary>
    /// Create or updated a customer.
    /// </summary>
    Task<IEnumerable<Customer>> CreateOrUpdateCustomerAsync(Customer customer);
    #endregion
}
