using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BookShop.Core.Models;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class InvoiceDetail
{
    [JsonProperty("invoice_id")]
    public int? InvoiceId
    {
        get; set;
    }

    [JsonProperty("ordinal_number")]
    public int? OrdinalNumber
    {
        get; set;
    }

    [JsonProperty("product_id")]
    public int? ProductId
    {
        get; set;
    }

    [JsonProperty("quantity")]
    public int? Quantity
    {
        get; set;
    } = 0;

    public override string ToString()
    {
        return $"Category(InvoiceId = {InvoiceId}, OrdinalNumber = {OrdinalNumber}, ProductId = {ProductId}, Quantity = {Quantity})";
    }
}
