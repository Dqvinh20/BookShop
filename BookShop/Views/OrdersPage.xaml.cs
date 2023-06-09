﻿using BookShop.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace BookShop.Views;

public sealed partial class OrdersPage : Page
{
    public OrdersViewModel ViewModel
    {
        get;
    }

    public OrdersPage()
    {
        ViewModel = App.GetService<OrdersViewModel>();
        InitializeComponent();
    }
}
