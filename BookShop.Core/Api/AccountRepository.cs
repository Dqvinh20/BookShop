using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Core.Contracts.Services;
using BookShop.Core.Models;

namespace BookShop.Core.Api;
public class AccountRepository : IAccountRepository
{
    private readonly HttpHelper _http;
    private readonly string _accessToken;
    private readonly string _controller = "accounts";

    public AccountRepository(string baseUrl, string accessToken)
    {
        _http = new HttpHelper(baseUrl);
        _accessToken = accessToken;
    }

    public async Task<IEnumerable<Account>> GetAccountByUsernameAsync(string username)
    {
        return await _http.GetAsync<IEnumerable<Account>>($"{_controller}?username=eq.{username}", _accessToken);
    }

    private async Task<bool> IsAccountExisted(string username)
    {
        return (await GetAccountByUsernameAsync(username)).Count() > 0;
    }

    public async Task<Tuple<bool, string>> CreateNewAccountAsync(Account account)
    {
        if (await IsAccountExisted(account.Username))
        {
            return await Task.FromResult(Tuple.Create(false, "Account already existed!!!"));
        }

        var data = await _http.PostAsync<Account, object>(_controller, account, _accessToken);
        return Tuple.Create(true, "Create account successfully!");
    }
}
