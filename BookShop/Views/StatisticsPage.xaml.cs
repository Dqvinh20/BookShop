using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using BookShop.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.Activation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookShop.Views;

public sealed partial class StatisticsPage : Page
{
    public StatisticsViewModel ViewModel
    {
        get;
    }
    

    public StatisticsPage()
    {
        ViewModel = App.GetService<StatisticsViewModel>();
        InitializeComponent();
        Loaded += OnLoaded;
    }
    public int GetWeekNumbers(int year)
    {
        CultureInfo culture = CultureInfo.CurrentCulture;
        Calendar calendar = culture.Calendar;

        DateTime firstDayOfYear = new DateTime(year, 1, 1);
        int firstWeekOfYear = calendar.GetWeekOfYear(firstDayOfYear, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);

        // Find the last day of the year
        DateTime lastDayOfYear = new DateTime(year, 12, 31);

        // Get the week number of the last day of the year
        int lastWeekOfYear = calendar.GetWeekOfYear(lastDayOfYear, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);

        return lastWeekOfYear - firstWeekOfYear + 1;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        int year = Convert.ToInt32(YearCombo.SelectedItem.ToString());
        LoadWeekList(year);
        //CultureInfo culture = CultureInfo.CurrentCulture;
        //Calendar calendar = culture.Calendar;
        //// Get the week number of the first day of the year
        //int numWeeks = GetWeekNumbers(year);
        //// Generate a list of week numbers from 1 to the number of weeks in the year
        //List<int> weekNumbers = Enumerable.Range(1, numWeeks).ToList();

        //foreach(int weekNumber in weekNumbers)
        //{
        //    ViewModel.Week.Add(weekNumber);
        //}
    }

    public void LoadWeekList(int year)
    {
        CultureInfo culture = CultureInfo.CurrentCulture;
        Calendar calendar = culture.Calendar;
        // Get the week number of the first day of the year
        int numWeeks = GetWeekNumbers(year);
        // Generate a list of week numbers from 1 to the number of weeks in the year
        List<int> weekNumbers = Enumerable.Range(1, numWeeks).ToList();
        ViewModel.Week.Clear();

        foreach (int weekNumber in weekNumbers)
        {
            ViewModel.Week.Add(weekNumber);
        }
    }

    private async void YearComboBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        int year = Convert.ToInt32(YearCombo.SelectedItem);
        await ViewModel.LoadYearChart(year);
        int month = MonthCombo.SelectedIndex + 1;
        DateTime startMonthDate = new DateTime(year, month, 1);
        DateTime endMonthDate = startMonthDate.AddMonths(1).AddDays(-1);
        await ViewModel.LoadMonthChart(startMonthDate, endMonthDate);
        LoadWeekList(year);
        WeekCombo.SelectedIndex = 0;
    }
    private async void MonthComboBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        int month = MonthCombo.SelectedIndex + 1;
        DateTime startMonthDate = new DateTime(Convert.ToInt32(YearCombo.SelectedItem), month, 1);
        DateTime endMonthDate = startMonthDate.AddMonths(1).AddDays(-1);
        await ViewModel.LoadMonthChart(startMonthDate, endMonthDate);
    }

    private Tuple<DateTime, DateTime> GetWeekDateByWeekIndex(int year, int weekInYear)
    {
        DateTime startDate = DateTime.MinValue;
        DateTime endDate = DateTime.MinValue;
        CultureInfo culture = CultureInfo.CurrentCulture;
        Calendar calendar = culture.Calendar;
        startDate = new DateTime(year, 1, 1).AddDays((weekInYear - 1) * 7 - (int)calendar.GetDayOfWeek(new DateTime(year, 1, 1)) + 1);
        if (weekInYear == GetWeekNumbers(year))
        {
            Console.WriteLine("FINAL WEEK:" + Convert.ToInt32(WeekCombo.SelectedItem.ToString()));
            endDate = new DateTime(year, 12, 31);
        }
        else
        {
            Console.WriteLine("START DATE:" + startDate.ToString());
            endDate = startDate.AddDays(6);
        }
        return Tuple.Create(startDate, endDate);
    }

    private async void WeekComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int year = Convert.ToInt32(YearCombo.SelectedItem.ToString());
        int month = MonthCombo.SelectedIndex + 1;
        if (WeekCombo.SelectedItem != null )
        {
            int week = Convert.ToInt32(WeekCombo.SelectedItem.ToString());
            (var startDate, var endDate) = GetWeekDateByWeekIndex(year, week);
            await ViewModel.LoadWeekChart(startDate, endDate);
        }
    }

    private async void OnApplyClick(object sender, RoutedEventArgs e)
    {
        DateTime start = ViewModel.FromDate.DateTime;
        DateTime end = ViewModel.ToDate.DateTime;
        Debug.WriteLine(start);
        Debug.WriteLine(end);

        await ViewModel.LoadDateChart(start, end);
        
    }

    private async void OnResetClick(object sender, RoutedEventArgs e)
    {
        ViewModel.FromDate = DateTime.Now;
        ViewModel.ToDate = DateTime.Now;
        DateTime start = ViewModel.FromDate.DateTime;
        DateTime end = ViewModel.ToDate.DateTime;
        await ViewModel.LoadDateChart(start, end);
    }
}
