using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BookShop.Core.Models;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class Account
{
    [JsonProperty("id")]
    public int? Id { get; set; }

    [JsonProperty("username")]
        
    public string Username { get; set; }

    [JsonProperty("password")]

    public string Password { get; set; }

    [JsonProperty("entropy")]

    public string Entropy { get; set; }

    [JsonProperty("created_at")]
    public DateTime? CreatedAt { get; set; }
}
