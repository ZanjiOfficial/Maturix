using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;


namespace MATURIXSHIFTPROJECT.Views
{
    /// <summary>
    /// Interaction logic for EditShiftView.xaml
    /// </summary>
    public partial class EditShiftView : UserControl
    {
        public EditShiftView()
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
