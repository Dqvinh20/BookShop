using Newtonsoft.Json;

namespace BookShop.Core.Models;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class Customer
{
    [JsonProperty("id")]
    public int? Id
    {
    get; set; }

    [JsonProperty("name")]

    public string Name
    {
    get; set;
    } = string.Empty;

    [JsonProperty("phone_num")]

    public string PhoneNumber
    {
    get; set; } = string.Empty;

    [JsonProperty("address")]
    public string Address
    {
        get; set;
    } = string.Empty;

    [JsonProperty("created_at")]
    public DateTime? CreatedAt
    {
    get; set; }
}
