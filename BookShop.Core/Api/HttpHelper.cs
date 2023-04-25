using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BookShop.Core.Api;
internal class HttpHelper
{
    /// <summary>           
    /// The Base URL for the API.
    /// /// </summary>
    private readonly string _baseUrl;

    public HttpHelper(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    /// <summary>
    /// Makes an HTTP GET request to the given controller and returns the deserialized response content.
    /// </summary>
    public async Task<TResult> GetAsync<TResult>(string controller, string accessToken, List<KeyValuePair<string, string>> headers = null)
    {
        if (accessToken is null)
        {
            throw new ArgumentNullException(nameof(accessToken));
        }

        using (var client = BaseClient())
        {
            if (headers != null)
            {
                foreach (var pair in headers)
                {
                    client.DefaultRequestHeaders.Add(pair.Key, pair.Value);
                }
            }
            client.DefaultRequestHeaders.Add("apikey", accessToken);
            var response = await client.GetAsync(controller);
            string json = await response.Content.ReadAsStringAsync();
            TResult obj = JsonConvert.DeserializeObject<TResult>(json);
            return obj;
        }
    }

    /// <summary>
    /// Makes an HTTP POST request to the given controller with the given object as the body.
    /// Returns the deserialized response content.
    /// </summary>
    public async Task<TResult> PostAsync<TRequest, TResult>(string controller, TRequest body, string accessToken, List<KeyValuePair<string, string>> headers = null)
    {
        if (accessToken is null)
        {
            throw new ArgumentNullException(nameof(accessToken));
        }

        using (var client = BaseClient())
        {
            if (headers != null)
            {
                foreach (var pair in headers)
                {
                    client.DefaultRequestHeaders.Add(pair.Key, pair.Value);
                }
            }
            client.DefaultRequestHeaders.Add("apikey", accessToken);
            var response = await client.PostAsync(controller, new JsonStringContent(body));
            string json = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json);
            TResult obj = JsonConvert.DeserializeObject<TResult>(json);
            return obj;
        }
    }

    /// <summary>
    /// Makes an HTTP DELETE request to the given controller and includes all the given
    /// object's properties as URL parameters. Returns the deserialized response content.
    /// </summary>
    public async Task DeleteAsync(string controller, string accessToken, List<KeyValuePair<string, string>> headers = null)
    {
        if (accessToken is null)
        {
            throw new ArgumentNullException(nameof(accessToken));
        }

        using (var client = BaseClient())
        {
            if (headers != null)
            {
                foreach (var pair in headers)
                {
                    client.DefaultRequestHeaders.Add(pair.Key, pair.Value);
                }
            }
            client.DefaultRequestHeaders.Add("apikey", accessToken);
            await client.DeleteAsync(controller);
        }
    }

    /// <summary>
    /// Constructs the base HTTP client, including correct authorization and API version headers.
    /// </summary>
    private HttpClient BaseClient() => new HttpClient { BaseAddress = new Uri(_baseUrl) };

    /// <summary>
    /// Helper class for formatting <see cref="StringContent"/> as UTF8 application/json. 
    /// </summary>
    private class JsonStringContent : StringContent
    {
        /// <summary>
        /// Creates <see cref="StringContent"/> formatted as UTF8 application/json.
        /// </summary>
        public JsonStringContent(object obj)
            : base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
        {
        }
    }
}
