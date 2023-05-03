using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;

namespace BookShop.Models;
public class DashboardItem
{
    public string Title
    {
        get; set; 
    } 
    public string Content
    {
        get; set; 
    }

    public SolidColorBrush BackgroundColor
    {
        get; set;
    } = new SolidColorBrush(Colors.SkyBlue);
}
