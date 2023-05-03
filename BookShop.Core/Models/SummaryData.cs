using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BookShop.Core.Models;
public class SummaryData
{
    [JsonProperty("timeline")]
    public DateTime Timeline
    {
        get; set; 
    }
    [JsonProperty("profit")]

    public int Profit
    {
        get; set; 
    }
    [JsonProperty("revenue")]

    public int Revenue
    {
        get; set;
    }
}
