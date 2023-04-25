using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Core.Api;
using BookShop.Core.Api.Services;
using BookShop.Core.Contracts.Services;
using BookShop.Core.Models;

namespace BookShop.Core.Services;
public class CategoriesService : ICategoriesService
{
    private static readonly string baseUrl = "https://najsmumzxvblqolfshxr.supabase.co/rest/v1/";
    private static readonly string apikey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im5hanNtdW16eHZibHFvbGZzaHhyIiwicm9sZSI6ImFub24iLCJpYXQiOjE2ODIyNDgzNDMsImV4cCI6MTk5NzgyNDM0M30.q6lIBvirm1W8Aar3H58fVpmuSFw5IOLOz2PF2RpKRtI";
    private List<Categories> _allCategories;
    private readonly ICategoriesRepository _restCategoriesRepository 
        = new RestCategoriesRepository(baseUrl, apikey);


    public async Task<IEnumerable<Categories>> GetContentGridDataAsync()
    {
        if (_allCategories == null)
        {
            _allCategories = new List<Categories>(await _restCategoriesRepository.GetAsync());
        }

        await Task.CompletedTask;
        return _allCategories;
    }

    public async Task<IEnumerable<Categories>> GetGridDataAsync()
    {
        if (_allCategories == null)
        {
            _allCategories = new List<Categories>(await _restCategoriesRepository.GetAsync());
        }

        await Task.CompletedTask;
        return _allCategories;
    }
}
