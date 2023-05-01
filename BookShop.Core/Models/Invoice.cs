using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BookShop.Core.Models;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class Invoice
{
    [JsonProperty("id")]
    public int? Id
    {
        get; set;
    }

    [JsonProperty("customer_id")]
    public int? CustomerId
    {
        get; set;
    }

    [JsonProperty("shipping_cost")]
    public int? ShippingCost
    {
        get; set;
    } = 0;

    [JsonProperty("total_money")]
    public int? TotalMoney
    {
        get; set;
    } = 0;

    [JsonProperty("created_at")]
    public DateTime? CreatedAt
    {
        get; set;
    }

    [JsonProperty("updated_at")]
    public DateTime? UpdatedAt
    {
        get; set;
    }

    public override string ToString()
    {
        return $"Category(Id = {Id}, CustomerId = {CustomerId}, ShippingCost = {ShippingCost}, TotalMoney = {TotalMoney}, CreateAt = {CreatedAt ?? null}, UpdatedAt = {UpdatedAt ?? null})";
    }
}
