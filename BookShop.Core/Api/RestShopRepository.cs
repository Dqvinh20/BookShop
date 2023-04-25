using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Core.Contracts.Services;

namespace BookShop.Core.Api;
public class RestShopRepository : IShopRepository
{
    private readonly string _url = "https://najsmumzxvblqolfshxr.supabase.co/rest/v1/";
    private readonly string _accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im5hanNtdW16eHZibHFvbGZzaHhyIiwicm9sZSI6ImFub24iLCJpYXQiOjE2ODIyNDgzNDMsImV4cCI6MTk5NzgyNDM0M30.q6lIBvirm1W8Aar3H58fVpmuSFw5IOLOz2PF2RpKRtI";
    //public RestShopRepository(string url, string accessToken)
    //{
    //    _url = url;
    //    _accessToken = accessToken;
    //}

    public ICategoriesRepository Categories => new RestCategoriesRepository(_url, _accessToken);
}
