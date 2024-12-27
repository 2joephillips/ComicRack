using ComicRack.Desktop.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace ComicRack.Desktop.Views.Pages
{
    public partial class StartUpPage : INavigableView<StartUpViewModel>
    {
        public StartUpViewModel ViewModel { get; }

 
        public StartUpPage(StartUpViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        private void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.ViewModel.OpenSelectedComicCommand.Execute(this);
        }

        private void Button_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

        }
    }
}
