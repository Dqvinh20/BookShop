using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace BookShop.Helpers;
public class DiscountToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double data)
        {
            return $"-{(int)(data * 100)}%";
        }
        return new ArgumentException("Argument DiscountToTextConverter must be double.e.g. 0.2");
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
}
