﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Core.Contracts.Services;

namespace BookShop.Core.Api;
public class RestShopRepository : IShopRepository
{
    private readonly string _baseUrl = "https://najsmumzxvblqolfshxr.supabase.co";
    private readonly string _accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im5hanNtdW16eHZibHFvbGZzaHhyIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTY4MjI0ODM0MywiZXhwIjoxOTk3ODI0MzQzfQ.CW8xjHcIJlEan7sywfKJwkiaoG0mQFwA8Oa-YXuH89o";
    //public RestShopRepository(string url, string accessToken)
    //{
    //    _url = url;
    //    _accessToken = accessToken;
    //}

    public ICategoriesRepository Categories => new RestCategoriesRepository($"{_baseUrl}/rest/v1/", _accessToken);

    public IProductsRepository Products => new RestProductsRespository($"{_baseUrl}/rest/v1/", _accessToken);

    public IStorageRepository Storage => new StorageRepository(_baseUrl, _accessToken);
}
