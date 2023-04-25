using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Core.Models;

namespace BookShop.Core.Contracts.Services;
public interface ICategoriesRepository
{
    /// <summary>
    /// Returns all categories. 
    /// </summary>
    Task<IEnumerable<Categories>> GetAllCategoriesAsync();

    /// <summary>
    /// Returns all categories with a data field matching the start of the given string. 
    /// </summary>
    Task<IEnumerable<Categories>> GetCategoriesWithQueryAsync(string query);

    /// <summary>
    /// Returns the categories with the given id. 
    /// </summary>
    Task<Categories> GetCategoryByIdAsync(int id);

    /// <summary>
    /// Adds a new categories if the categories does not exist, updates the 
    /// existing categories otherwise.
    /// </summary>
    Task<IEnumerable<Categories>> UpsertCategoryAsync(Categories categories);

    /// <summary>
    /// Deletes a categories.
    /// </summary>
    Task DeleteCategoryAsync(int id);
}
