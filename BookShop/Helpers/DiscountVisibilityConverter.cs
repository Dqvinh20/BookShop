using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace BookShop.Helpers;
public class DiscountVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double data)
        {
            return data == 0 ? Visibility.Collapsed : Visibility.Visible;
        }
        return new ArgumentException("DiscountVisibilityConverter");
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language) 
    {
        return value; 
    }
}
