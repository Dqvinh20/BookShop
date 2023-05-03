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
    get; set; }

    [JsonProperty("customer_id")]
    public int? CustomerId
    {
        get; set;
    } = null;

    [JsonProperty("total_money")]
    public int TotalMoney
    {
        get; set;
    } = 0;

    [JsonProperty("created_at")] 
    public DateTime? CreatedAt { get; set; } = null;

    [JsonProperty("customers")]
    public Customer? Customer
    {
        get; set;
    } = null;

    [JsonProperty("invoice_detail")]
    public List<InvoiceDetail>? InvoiceDetails { get; set; } = null;
}
