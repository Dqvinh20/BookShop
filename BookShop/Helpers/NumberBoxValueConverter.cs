using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace BookShop.Helpers;


public class NumberBoxValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return System.Convert.ToDouble(value);
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is double number)
        {
            if (double.IsNaN(number))
            {
                return 0;
            }
            return System.Convert.ToInt32(value);
        }
        throw new ArgumentException("ExceptionNumberBoxValueConverterMustBeAnDouble");
    }
}