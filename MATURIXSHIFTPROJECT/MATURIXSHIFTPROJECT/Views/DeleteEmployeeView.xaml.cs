﻿using System;
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
    /// Interaction logic for DeleteEmployeeView.xaml
    /// </summary>
    public partial class DeleteEmployeeView : UserControl
    {
        public DeleteEmployeeView()
        {
            InitializeComponent();
        }

        public void CBChange(object sender, RoutedEventArgs e)
        {
            if (CBChange != null) { DeleteBTN.IsEnabled = true; }
        }
    }
}
