using BookShop.Core.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace BookShop.ViewModels;

public class StatisticsViewModel : ObservableRecipient
{
    public ObservableCollection<SummaryData> SummaryYear
    {
        get; set;
    } = new ObservableCollection<SummaryData>();
    public ObservableCollection<SummaryData> SummaryMonth
    {
        get; set;
    } = new ObservableCollection<SummaryData>();    
    public ObservableCollection<SummaryData> SummaryWeek
    {
        get; set;
    } = new ObservableCollection<SummaryData>();    
    public ObservableCollection<SummaryData> SummaryDate
    {
        get; set;
    } = new ObservableCollection<SummaryData>();
    public ObservableCollection<int> Year
    {
        get; set;
    } = new ObservableCollection<int>();

    public ObservableCollection<string> Month
    {
        get; set;
    } = new ObservableCollection<string>();    
    public ObservableCollection<int> Week
    {
        get; set;
    } = new ObservableCollection<int>();
    public double YearInterval
    {
        get; set;
    } = 0;
    public double MonthInterval
    {
        get; set;
    } = 0;
    public double WeekInterval
    {
        get; set;
    } = 0;
    public double DateInterval
    {
        get; set;
    } = 0;

    public DateTimeOffset FromDate
    {
        get => _fromDate; 
        set => SetProperty(ref _fromDate, value);
    } 
    public DateTimeOffset ToDate
    {
        get => _toDate;
        set => SetProperty(ref _toDate, value);
    }
    public DateTimeOffset CurrentDate  = DateTime.Now;
    private DateTimeOffset _fromDate = DateTime.Now;
    private DateTimeOffset _toDate = DateTime.Now;
    private bool _isLoading = false;

    public bool IsLoading
    {
        set => SetProperty(ref _isLoading, value);
        get => _isLoading;  
    }
    public async Task LoadYearChart(int year)
    {
        SummaryYear.Clear();
        var stat = await App.Repository.Statistic.GetSummaryByYearAsync(year);

        foreach (var item in stat)
        {
            SummaryYear.Add(item);
            YearInterval = Math.Max(YearInterval, Math.Max(item.Profit, item.Revenue));
        }
        YearInterval = (double) Convert.ToInt32(YearInterval / 10);
    }
    public async Task LoadMonthChart(DateTime fromDate, DateTime toDate)
    {
        SummaryMonth.Clear();

        var stat = await App.Repository.Statistic.GetSummaryFromDateToDateAsync(fromDate, toDate);

        foreach (var item in stat)
        {
            SummaryMonth.Add(item);
            MonthInterval = Math.Max(MonthInterval, Math.Max(item.Profit, item.Revenue));
        }
        MonthInterval = (double)Convert.ToInt32(MonthInterval / 10);
    }

    public async Task LoadWeekChart(DateTime fromDate, DateTime toDate)
    {
        SummaryWeek.Clear();

        var stat = await App.Repository.Statistic.GetSummaryFromDateToDateAsync(fromDate, toDate);

        foreach (var item in stat)
        {
            SummaryWeek.Add(item);
            WeekInterval = Math.Max(WeekInterval, Math.Max(item.Profit, item.Revenue));
        }
        WeekInterval = (double)Convert.ToInt32(WeekInterval / 10);
    }
    public async Task LoadDateChart(DateTime fromDate, DateTime toDate)
    {
        SummaryDate.Clear();

        var stat = await App.Repository.Statistic.GetSummaryFromDateToDateAsync(fromDate, toDate);

        foreach (var item in stat)
        {
            Debug.WriteLine(item.Timeline);
            Debug.WriteLine(item.Revenue);
            Debug.WriteLine(item.Profit);

            SummaryDate.Add(item);
            DateInterval = Math.Max(DateInterval, Math.Max(item.Profit, item.Revenue));
        }
        DateInterval = (double)Convert.ToInt32(DateInterval / 10);
    }
    public StatisticsViewModel()
    {
        Year.Add(2023);
        Year.Add(2022);
        Month.Add("Jan");
        Month.Add("Feb");
        Month.Add("Mar");
        Month.Add("Apr");
        Month.Add("May");
        Month.Add("Jun");
        Month.Add("Jul");
        Month.Add("Aug");
        Month.Add("Sep");
        Month.Add("Oct");
        Month.Add("Nov");
        Month.Add("Dec");
    }

}
