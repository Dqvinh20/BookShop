using System.Collections.ObjectModel;
using BookShop.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BookShop.ViewModels;

public class DashboardViewModel : ObservableRecipient
{
    public ObservableCollection<Shoe> SneakersDetail
    {
        get;
    }
    private double startAngle;
    private double endAngle;

    public double StartAngle
    {
        get => startAngle;

        set
        {
            startAngle = value;
            //RaisePropertyChanged(nameof(this.StartAngle));
        }
    }

    public double EndAngle
    {
        get => endAngle;

        set
        {
            endAngle = value;
            //RaisePropertyChanged(nameof(this.EndAngle));
        }
    }
    public DashboardViewModel()
    {
        this.SneakersDetail = new ObservableCollection<Shoe>();
        SneakersDetail.Add(new Shoe() { Brand = "Adidas", ItemsCount = -24 });
        SneakersDetail.Add(new Shoe() { Brand = "Nike", ItemsCount = 17 });
        SneakersDetail.Add(new Shoe() { Brand = "Reebok", ItemsCount = 30 });
        SneakersDetail.Add(new Shoe() { Brand = "Fila", ItemsCount = 18 });
        SneakersDetail.Add(new Shoe() { Brand = "Puma", ItemsCount = 10 });
        SneakersDetail.Add(new Shoe() { Brand = "Clarks", ItemsCount = 15 });

        StartAngle = 180;
        EndAngle = 360;
    }
}
