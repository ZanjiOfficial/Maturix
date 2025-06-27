using MATURIXSHIFTPROJECT.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MATURIXSHIFTPROJECT.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double totalWidth = myListView.ActualWidth;
            double padding = 0;

            ((GridView)myListView.View).Columns[0].Width = (totalWidth * 0.2) - padding;
            ((GridView)myListView.View).Columns[1].Width = (totalWidth * 0.2) - padding;
            ((GridView)myListView.View).Columns[2].Width = (totalWidth * 0.2) - padding;
            ((GridView)myListView.View).Columns[3].Width = (totalWidth * 0.2) - padding;
            ((GridView)myListView.View).Columns[4].Width = (totalWidth * 0.2) - 1;

        }
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView listView)
            {
                // Assuming DataContext is your view model
                HomeViewModel viewModel = DataContext as HomeViewModel;
                viewModel?.UpdateShiftSelection(listView);
            }

        }

    }


}
