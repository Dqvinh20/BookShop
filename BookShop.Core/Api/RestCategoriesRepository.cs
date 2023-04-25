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
    public async Task<IEnumerable<Categories>> GetAllCategoriesAsync() => await _http.GetAsync<IEnumerable<Categories>>(_controller, _accessToken);
    public async Task<IEnumerable<Categories>> GetCategoriesWithQueryAsync(string query) => await _http.GetAsync<IEnumerable<Categories>>($"{_controller}?{query}", _accessToken);
    public async Task<Categories> GetCategoryByIdAsync(int id) => await _http.GetAsync<Categories>($"{_controller}?id=eq.{id}", _accessToken);
    public async Task<IEnumerable<Categories>> UpsertCategoryAsync(Categories categories) => await _http.PostAsync<Categories, IEnumerable<Categories>>(_controller, categories, _accessToken);
}
