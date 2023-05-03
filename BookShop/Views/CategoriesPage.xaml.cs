using System.Reflection;
using BookShop.Core.Models;
using BookShop.ViewModels;
using ColorCode.Common;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Newtonsoft.Json.Linq;

namespace BookShop.Views;

public sealed partial class CategoriesPage : Page
{
    public CategoriesViewModel ViewModel
    {
        get;
    }

    public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(Categories), typeof(CategoriesPage), new PropertyMetadata(null));

    public Categories? SelectedItem
    {
        get
        {
            return (Categories)GetValue(SelectedItemProperty);
        }
        set
        {
            ViewModel.IsNotEmpty = value != null;
            SetValue(SelectedItemProperty, value);
        }
    }

    public CategoriesPage()
    {
        ViewModel = App.GetService<CategoriesViewModel>();
        InitializeComponent();
        ViewModel.IsNotEmpty = SelectedItem != null;
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

    private async void OnElementClicked(object sender, RoutedEventArgs e)
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
                ShowDialog_Clicked("Delete");
                break;
                
        }
    }
    private async void ShowDialog_Clicked(String title)
    {
        ContentDialog dialog = new ContentDialog();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = this.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = title == "Add" ? "Add new category" : title;
        TextBox textBox = null;
        if (title != "Delete")
        {
            dialog.PrimaryButtonText = "Save";
            textBox = new TextBox();
            textBox.Text = title == "Add" ? "" : SelectedItem.Name;
            dialog.Content = textBox;
        }
        else
        {
            dialog.PrimaryButtonText = "Yes, delete it";
            dialog.Content = "Are you sure you want to delete it?";
        }
        
        dialog.CloseButtonText = "Cancel";
        dialog.DefaultButton = ContentDialogButton.Primary;

        if (title != "Delete" || (title == "Delete" && SelectedItem != null))
        {
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {

                if (title != "Delete")
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
                }

                ViewModel.IsBusy = true;


                if (title != "Delete")
                {
                    var newCategory = new Categories()
                    {
                        Name = textBox.Text,
                        Id = title == "Add" ? null : SelectedItem.Id
                    };
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
                }
                else
                {
                    await App.Repository.Categories.DeleteCategoryAsync((int)SelectedItem.Id);
                    ViewModel.Source.Remove(SelectedItem);
                    SelectedItem = null;
                }

                ViewModel.IsBusy = false;
                //acrylicArea.Visibility = Visibility.Collapsed;
            }
        }
    }
}
