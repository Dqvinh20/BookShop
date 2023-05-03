using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Core.Models;

namespace BookShop.Core.Contracts.Services;
public interface IStatisticRepository
{
    Task<IEnumerable<SummaryData>> GetSummaryByYearAsync(int year);
    Task<IEnumerable<SummaryData>> GetSummaryFromDateToDateAsync(DateTime fromDate, DateTime toDate);

    
}
