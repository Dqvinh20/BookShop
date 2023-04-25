using System.Reflection;
using BookShop.Core.Models;
using BookShop.ViewModels;
using ColorCode.Common;
using Microsoft.UI.Text;
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
        var newItem = ViewModel.Source[args.Index];
        MoveToSelectionState(args.Element, newItem == SelectedItem);
    }

    private void OnElementClicked(object sender, RoutedEventArgs e)
    {
        switch ((sender as AppBarButton).Label)
        {
            case "Add":
                ShowDialog_Clicked("Add");
                break;
            case "Edit":
                ShowDialog_Clicked("Edit");
                break;
            case "Delete":
                
                break;
                
        }
    }
    private async void ShowDialog_Clicked(String title)
    {
        ContentDialog dialog = new ContentDialog();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = this.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = title == "Add" ? "Add new category" : "Edit";
        dialog.PrimaryButtonText = "Save";
        dialog.CloseButtonText = "Cancel";
        dialog.DefaultButton = ContentDialogButton.Primary;
        TextBox textBox = new TextBox();
        textBox.Text = title == "Add" ? "" : SelectedItem.Name;
        dialog.Content = textBox;

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            if (ViewModel.Source.FirstOrDefault(c => c.Name == textBox.Text) != null)
            {
                ContentDialog dialog1 = new ContentDialog();

                // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                dialog.XamlRoot = this.XamlRoot;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.Title = "Error";
                dialog.PrimaryButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.Content = "Category already existed";

                await dialog.ShowAsync();
                return;
            }
            var newCategory = new Categories() {
                Name = textBox.Text,
                Id = title == "Add" ? null : SelectedItem.Id
            };

            ViewModel.IsBusy = true;
            var newData = await App.Repository.Categories.UpsertCategoryAsync(newCategory);
            foreach (var category in newData)
            {
                if (title == "Add")
                {
                    ViewModel.Source.Add(category);
                }
                else
                {
                    var oldIndex = ViewModel.Source.IndexOf(SelectedItem);
                    ViewModel.Source[oldIndex] = category;
                    SelectedItem = ViewModel.Source[oldIndex];
                }
            }
            ViewModel.Source.SortStable((x, y) => string.Compare(x.Name, y.Name));
            ViewModel.IsBusy = false;
        }
    }
}
