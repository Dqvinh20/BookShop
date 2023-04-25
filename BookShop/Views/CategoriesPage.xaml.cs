using BookShop.Core.Models;
using BookShop.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace BookShop.Views;

// TODO: Change the grid as appropriate for your app. Adjust the column definitions on DataGridPage.xaml.
// For more details, see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid.
public sealed partial class CategoriesPage : Page
{
    public CategoriesViewModel ViewModel
    {
        get;
    }

    public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(Categories), typeof(CategoriesPage), new PropertyMetadata(null));

    public Categories SelectedItem
    {
        get
        {
            return (Categories)GetValue(SelectedItemProperty);
        }
        set
        {
            SetValue(SelectedItemProperty, value);
        }
    }

    public CategoriesPage()
    {
        ViewModel = App.GetService<CategoriesViewModel>();
        InitializeComponent();

    }

    private void Category_TemplatePointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var oldIndex = ViewModel.Source.IndexOf(SelectedItem);
        var previousItem = ItemRepeater.TryGetElement(oldIndex);
        if (previousItem != null)
        {
            MoveToSelectionState(previousItem, false);
        }

        var itemIndex = ItemRepeater.GetElementIndex(sender as UIElement);
        SelectedItem = ViewModel.Source[itemIndex != -1 ? itemIndex : 0];
        MoveToSelectionState(sender as UIElement, true);
    }

    private static void MoveToSelectionState(UIElement previousItem, bool isSelected)
    {
        VisualStateManager.GoToState(previousItem as Control, isSelected ? "Selected" : "Default", false);
    }

    private void ItemRepeater_ElementIndexChanged(ItemsRepeater sender, ItemsRepeaterElementIndexChangedEventArgs args)
    {
        var newItem = ViewModel.Source[args.NewIndex];
        MoveToSelectionState(args.Element, newItem == SelectedItem);
    }

    private void ItemRepeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
    {
        Console.WriteLine(ViewModel.IsBusy);
        var newItem = ViewModel.Source[args.Index];
        SelectedItem = ViewModel.Source[0];
        MoveToSelectionState(args.Element, newItem == SelectedItem);
    }
}
