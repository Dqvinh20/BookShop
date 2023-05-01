using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
using Windows.Globalization;
using Windows.Globalization.NumberFormatting;

namespace BookShop.Helpers;
public class NumberToCurrencyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return $"{value:n0}đ";
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return new ArgumentException();
    }
}
