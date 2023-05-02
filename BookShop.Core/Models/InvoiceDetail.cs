using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
﻿using Newtonsoft.Json;

namespace BookShop.Core.Models;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class InvoiceDetail
{
    [JsonProperty("ordinal_number")]
    public int? Id
    {
    get; set; }

    [JsonProperty("invoice_id")]
    public int? InvoiceId
    {
        get; set;
    }
    [JsonProperty("product_id")]
    public int? ProductId
    {
    get; set; }

    [JsonProperty("quantity")]
    public long Quantity
    {
        get; set;
    }

    [JsonProperty("unit_sell_price")]
    public long UnitSellPrice
    {
        get; set;
    }


    [JsonProperty("total_money")]
    public long TotalMoney
    {
        get; set;
    }

    [JsonProperty("created_at")]
    public DateTime? CreatedAt
    {
        get; set;
    }

    [JsonProperty("products")]
    public Product Product
    {
    get; set; }
}
