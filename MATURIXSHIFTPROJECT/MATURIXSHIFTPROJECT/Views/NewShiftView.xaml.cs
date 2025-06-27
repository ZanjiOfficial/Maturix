using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace MATURIXSHIFTPROJECT.Views
{
    /// <summary>
    /// Interaction logic for NewShiftView.xaml
    /// </summary>
    public partial class NewShiftView : UserControl
    {
        public NewShiftView()
        {
            InitializeComponent();
        }

        private void TimeMaskedTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as MaskedTextBox;
            if (tb != null)
            {
                tb.Dispatcher.BeginInvoke(new Action(() =>
                {
                    tb.Select(0, 0);  // Move caret to start
                }));
            }
        }
    }
}
