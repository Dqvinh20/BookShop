using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Core.Models;

namespace BookShop.Core.Contracts.Services;
public interface ICategoriesService
{
    Task<IEnumerable<Categories>> GetContentGridDataAsync();
    Task<IEnumerable<Categories>> GetGridDataAsync();
}
