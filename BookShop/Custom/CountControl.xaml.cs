// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization.NumberFormatting;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BookShop.Custom;
public sealed class CountControlValueChangedEventArgs
{
    public CountControlValueChangedEventArgs(int oldValue, int newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    public int OldValue
    {
        get;
    }

    public int NewValue
    {
        get;
    }
}


public sealed partial class CountControl : UserControl
{
    public CountControl()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty MaxProductCountProperty = DependencyProperty.Register(
        nameof(MaxProductCount),
        typeof(int),
        typeof(CountControl),
        new PropertyMetadata(1));

    public static readonly DependencyProperty MinProductCountProperty = DependencyProperty.Register(
        nameof(MinProductCount),
        typeof(int),
        typeof(CountControl),
        new PropertyMetadata(1));

    public static readonly DependencyProperty CurrentProductCountProperty = DependencyProperty.Register(
        nameof(CurrentProductCount),
        typeof(int),
        typeof(CountControl),
        new PropertyMetadata(1));

    public static readonly DependencyProperty NumberFormatterProperty = DependencyProperty.Register(
        nameof(NumberFormatter),
        typeof(INumberFormatter2),
        typeof(CountControl),
        new PropertyMetadata(new DecimalFormatter()
        {
            FractionDigits = 0
        }));

    public static readonly DependencyProperty PrefixTextProperty = DependencyProperty.Register(
        nameof(PrefixText),
        typeof(string),
        typeof(CountControl),
        new PropertyMetadata(string.Empty));

    public int CurrentProductCount
    {
        get => (int)GetValue(CurrentProductCountProperty);
        set => SetValue(CurrentProductCountProperty, value);
    }

    public int MaxProductCount
    {
        get => (int)GetValue(MaxProductCountProperty);
        set => SetValue(MaxProductCountProperty, value);
    }

    public int MinProductCount
    {
        get => (int)GetValue(MinProductCountProperty);
        set => SetValue(MinProductCountProperty, value);
    }

    public INumberFormatter2 NumberFormatter
    {
        get => (INumberFormatter2)GetValue(NumberFormatterProperty);
        set => SetValue(NumberFormatterProperty, value);
    }

    public string PrefixText
    {
        get => (string)GetValue(PrefixTextProperty);
        set => SetValue(PrefixTextProperty, value);
    }

    public event TypedEventHandler<CountControl, CountControlValueChangedEventArgs>? ProductCountChanged;

    private void CurrentCountNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        ProductCountChanged?.Invoke(this, new CountControlValueChangedEventArgs((int) args.OldValue, (int) args.NewValue));
    }
}
