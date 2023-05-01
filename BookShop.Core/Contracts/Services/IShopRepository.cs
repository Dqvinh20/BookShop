using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Core.Contracts.Services;
public interface IShopRepository
{
    /// <summary>
    /// Returns the accounts repository.
    /// </summary>
    IAccountRepository Accounts
    {
        get;
    }
    /// <summary>
    /// Returns the categories repository.
    /// </summary>
    ICategoriesRepository Categories
    {
        get;
    }
    /// <summary>
    /// Returns the products repository.
    /// </summary>
    IProductsRepository Products
    {
        get;
    }

    /// <summary>
    /// Returns the invoice repository.
    /// </summary>
    IInvoiceRepository Invoice
    {
        get;
    }

    /// <summary>
    /// Returns the storage repository.
    /// </summary>
    IStorageRepository Storage
    {
        get;
    }
}
