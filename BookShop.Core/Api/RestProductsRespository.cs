using BookShop.Core.Contracts.Services;
using BookShop.Core.Models;

namespace BookShop.Core.Api;
public class RestProductsRespository : IProductsRepository
{
    private readonly HttpHelper _http;
    private readonly string _accessToken;
    private readonly string _controller = "products";

    public RestProductsRespository(string baseUrl, string accessToken)
    {
        _http = new HttpHelper(baseUrl);
        _accessToken = accessToken;
    }

    public async Task DeleteProductAsync(int id) => await _http.DeleteAsync($"{_controller}?id=eq.{id}", _accessToken);
    public async Task<IEnumerable<Product>> GetAllProductsAsync() => await _http.GetAsync<IEnumerable<Product>>($"{_controller}?select=*,categories(*)", _accessToken, null);
    public async Task<Product> GetProductByIdAsync(int id)
    {
        var data = await _http.GetAsync<IEnumerable<Product>>($"{_controller}?select=*,categories(*),id=eq.{id}", _accessToken, null);
        return data.FirstOrDefault();
    }

    public async Task<IEnumerable<Product>> UpsertProductAsync(Product product)
    {
        List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
        headers.Add(KeyValuePair.Create("Prefer", "return=representation"));
        headers.Add(KeyValuePair.Create("Prefer", "resolution=merge-duplicates"));
        return await _http.PostAsync<Product, IEnumerable<Product>>(_controller, product, _accessToken, headers);
    }

    public async Task<IEnumerable<Product>> UpsertProductAsync(List<Product> products)
    {
        List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
        headers.Add(KeyValuePair.Create("Prefer", "return=representation"));
        headers.Add(KeyValuePair.Create("Prefer", "resolution=merge-duplicates"));
        return await _http.PostAsync<List<Product>, IEnumerable<Product>>(_controller, products, _accessToken, headers);
    }

}
