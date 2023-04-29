using BookShop.Core.Contracts.Services;
using BookShop.Core.Models;

namespace BookShop.Core.Api;
public class RestCategoriesRepository : ICategoriesRepository
{
    private readonly HttpHelper _http;
    private readonly string _accessToken;
    private readonly string _controller = "categories";

    public RestCategoriesRepository(string baseUrl, string accessToken)
    {
        _http = new HttpHelper(baseUrl);
        _accessToken = accessToken;
    }

    public async Task DeleteCategoryAsync(int id) => await _http.DeleteAsync($"{_controller}?id=eq.{id}", _accessToken);
    public async Task<IEnumerable<Categories>> GetAllCategoriesAsync()
    {
        List<Categories> result = new List<Categories>(await _http.GetAsync<IEnumerable<Categories>>(_controller, _accessToken));
        result.RemoveAt(0);
        result.Sort((x, y) => string.Compare(x.Name, y.Name));
        return result;
    }
    public async Task<IEnumerable<Categories>> GetCategoriesWithQueryAsync(string query) => await _http.GetAsync<IEnumerable<Categories>>($"{_controller}?{query}", _accessToken);
    public async Task<Categories> GetCategoryByIdAsync(int id) => await _http.GetAsync<Categories>($"{_controller}?id=eq.{id}", _accessToken);
    public async Task<IEnumerable<Categories>> UpsertCategoryAsync(Categories categories) {
        List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
        headers.Add(KeyValuePair.Create("Prefer", "return=representation"));
        headers.Add(KeyValuePair.Create("Prefer", "resolution=merge-duplicates"));
        return await _http.PostAsync<Categories, IEnumerable<Categories>>(_controller, categories, _accessToken, headers);
    }

    public async Task<IEnumerable<Categories>> UpsertCategoryAsync(List<Categories> categories)
    {
        List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
        headers.Add(KeyValuePair.Create("Prefer", "return=representation"));
        headers.Add(KeyValuePair.Create("Prefer", "resolution=merge-duplicates"));
        return await _http.PostAsync<List<Categories>, IEnumerable<Categories>>(_controller, categories, _accessToken, headers);
    }
}
