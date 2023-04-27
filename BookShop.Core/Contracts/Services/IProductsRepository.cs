using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Core.Models;

namespace BookShop.Core.Contracts.Services;
public interface IProductsRepository
{
    /// <summary>
    /// Returns all products. 
    /// </summary>
    Task<IEnumerable<Product>> GetAllProductsAsync();

    /// <summary>
    /// Returns the products with the given id. 
    /// </summary>
    Task<Product> GetProductByIdAsync(int id);

    /// <summary>
    /// Adds a new products if the products does not exist, updates the 
    /// existing products otherwise.
    /// </summary>
    Task<IEnumerable<Product>> UpsertProductAsync(Product product);

    /// <summary>
    /// Deletes a products.
    /// </summary>
    Task DeleteProductAsync(int id);
}
