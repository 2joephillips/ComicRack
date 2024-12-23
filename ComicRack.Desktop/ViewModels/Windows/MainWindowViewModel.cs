using ComicRack.Core;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace ComicRack.Desktop.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {


        [ObservableProperty]
        private string _applicationTitle = "WPF UI - ComicRack.Desktop";

        [ObservableProperty]
        private bool _paneOpen = false;

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = GetMenuItems();



        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = GetFooterMenuItems();

      

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };



        private static ObservableCollection<object> GetMenuItems()
        {
            return ApplicationSettings.IsSetUpComplete ?
              new() {
                new NavigationViewItem()
                {
                    Content = "Home",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                    TargetPageType = typeof(Views.Pages.DashboardPage)
                },
                new NavigationViewItem()
                {
                    Content = "Data",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                    TargetPageType = typeof(Views.Pages.DataPage)
                },
             } :
             new();
        }


        private static ObservableCollection<object> GetFooterMenuItems()
        {
            return ApplicationSettings.IsSetUpComplete ? new()
            {
                new NavigationViewItem()
                {
                    Content = "Settings",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                    TargetPageType = typeof(Views.Pages.SettingsPage)
                },
            } :
            new();
        }
    }
}
