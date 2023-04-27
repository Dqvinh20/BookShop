using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using BookShop.Core.Contracts.Services;
using Supabase.Storage;
using FileOptions = Supabase.Storage.FileOptions;

namespace BookShop.Core.Api;
public class StorageRepository : IStorageRepository
{
    private Supabase.Client _client;
    private string bucketName = "Testing";

    public StorageRepository(string baseUrl, string accessToken)
    {
        _client = new Supabase.Client(baseUrl, accessToken);
        _client.InitializeAsync();
    }

    public async Task<bool> DeleteImageAsync(string imagePath)
    {
        var data = await _client.Storage.From(bucketName).Remove(new List<string> { imagePath });
        return data.ToList().Count != 0;
    }
    public async Task<string> UploadImageAsync(string imagePath, string imageName, EventHandler<float> onProgress = null)
    {
        var data = await _client.Storage.From(bucketName).Upload(imagePath, imageName, new FileOptions { Upsert = true }, onProgress);
        var publicUrl = _client.Storage.From(bucketName).GetPublicUrl(imageName);
        return Uri.EscapeUriString(publicUrl);
    }
}
