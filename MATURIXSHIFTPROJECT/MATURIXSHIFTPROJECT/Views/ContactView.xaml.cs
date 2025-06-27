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

namespace MATURIXSHIFTPROJECT.Views
{
    /// <summary>
    /// Interaction logic for ContactView.xaml
    /// </summary>
    public partial class ContactView : UserControl
    {
        public ContactView()
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
            ((GridView)myListView.View).Columns[4].Width = (totalWidth * 0.2) - padding;

        }


    }
}
