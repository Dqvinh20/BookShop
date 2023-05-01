using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Core.Models;

namespace BookShop.Core.Contracts.Services;
public interface IAccountRepository
{
    /// <summary>
    /// Returns account info
    /// </summary>
    Task<IEnumerable<Account>> GetAccountByUsernameAsync(string username);

    /// <summary>
    /// Add new account 
    /// </summary>
    Task<Tuple<bool, string>> CreateNewAccountAsync(Account account);
}
