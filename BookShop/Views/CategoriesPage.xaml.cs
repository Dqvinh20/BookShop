using BookShop.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace BookShop.Views;

// TODO: Change the grid as appropriate for your app. Adjust the column definitions on DataGridPage.xaml.
// For more details, see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid.
public sealed partial class CategoriesPage : Page
{
    public CategoriesViewModel ViewModel
    {
        get;
    }

    public CategoriesPage()
    {
        ViewModel = App.GetService<CategoriesViewModel>();
        InitializeComponent();
    }
}
