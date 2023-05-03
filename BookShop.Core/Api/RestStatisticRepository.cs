using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Core.Contracts.Services;
using BookShop.Core.Models;

namespace BookShop.Core.Api;
public class RestStatisticRepository : IStatisticRepository
{
    private readonly HttpHelper _http;
    private readonly string _accessToken;
    private readonly string _invoice_controller = "invoice";
    private readonly string _invoice_detail_controller = "invoice_detail";
    private readonly string _func_controller = "rpc";


    public RestStatisticRepository(string baseUrl, string accessToken)
    {
        _http = new HttpHelper(baseUrl);
        _accessToken = accessToken;
    }


    public async Task<IEnumerable<SummaryData>> GetSummaryByYearAsync(int year)
    {
        return await _http.GetAsync<IEnumerable<SummaryData>>($"{_func_controller}/summary_chart?year_query={year}", _accessToken);
    }
    public async Task<IEnumerable<SummaryData>> GetSummaryFromDateToDateAsync(DateTime fromDate, DateTime toDate)
    {
        return await _http.GetAsync<IEnumerable<SummaryData>>($"{_func_controller}/summary_chart_by_date?from_date={fromDate.ToString("yyyy-MM-dd")}&_to_date={toDate.ToString("yyyy-MM-dd")}", _accessToken);
    }

}
