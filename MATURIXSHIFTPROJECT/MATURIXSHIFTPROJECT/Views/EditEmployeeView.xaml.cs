using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    /// Interaction logic for EditEmployeeView.xaml
    /// </summary>
    public partial class EditEmployeeView : UserControl
    {
        public EditEmployeeView()
        {
            InitializeComponent();
        }
        #region PlaceholderText
        //private void Loaded_Department(object sender, RoutedEventArgs e)
        //{
        //    var comboBox = sender as ComboBox;
        //    comboBox.Foreground = Brushes.Gray;
        //    comboBox.SelectedItem = "--Select Department--";
        //    comboBox.IsEnabled = false;
        //}
        private void Loaded_User(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            comboBox.Foreground = Brushes.Gray;
            comboBox.SelectedItem = "--Select User--";
        }

        //private void SelectionChanged_Department(object sender, SelectionChangedEventArgs e)
        //{
        //    var comboBox = sender as ComboBox;
        //    var SelectedItem = comboBox.SelectedItem as string;

        //    if (SelectedItem != "--Select Department--")
        //    {
        //        comboBox.Foreground = Brushes.Black;
        //    }
        //    else
        //    {
        //        comboBox.Foreground = Brushes.Gray;
        //    }
        //}
        private void SelectionChanged_User(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var SelectedItem = comboBox.SelectedItem as string;

            if (SelectedItem != "--Select User--")
            {
                comboBox.Foreground = Brushes.Black;
                ChangeState(true);
            }
            else
            {
                comboBox.Foreground = Brushes.Gray;
                ChangeState(false);
            }
        }

        private void CB_GotFocus(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            comboBox.Foreground = Brushes.Black;
        }


        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var TBox = sender as TextBox;
            if (TBox != null)
            {
                LoadTB(TBox); // Call LoadTB to set the placeholder text and gray foreground
                TBox.IsEnabled = false;
            }
        }

        //private void TBoxClicked(object sender, RoutedEventArgs e)
        //{
        //    var TBox = sender as TextBox;
        //    if (TBox != null)
        //    {
        //        // Set text color to black when the user clicks inside the TextBox
        //        TBox.Foreground = Brushes.Black;

        //        // Clear the text if the user clicks (focuses) on the TextBox
        //        if (TBox.Text == GetPlaceholderText(TBox))
        //        {
        //            TBox.Text = string.Empty;
        //        }
        //    }
        //}

        //private void LostFocusTB(object sender, RoutedEventArgs e)
        //{
        //    var TBox = sender as TextBox;
        //    if (TBox != null && string.IsNullOrEmpty(TBox.Text))
        //    {
        //        // If text is empty, load the placeholder text again
        //        LoadTB(TBox);
        //    }
        //}

        public void LoadTB(TextBox TBox)
        {
            if (TBox != null)
            {
                // Set text color to gray for placeholder text

                // Load the correct placeholder text based on the TextBox name
                TBox.Text = GetPlaceholderText(TBox);
            }
        }

        private string GetPlaceholderText(TextBox TBox)
        {
            switch (TBox.Name)
            {
                case "InitialsTB":
                    return "Initials";
                case "NameTB":
                    return "Full Name";
                case "EmailTB":
                    return "Email";
                case "PhoneNumberTB":
                    return "Phone Number";
                default:
                    return string.Empty;
            }
        }

        private void ChangeState(bool b)
        {
            InitialsTB.IsEnabled = b;
            NameTB.IsEnabled = b;
            PhoneNumberTB.IsEnabled = b;
            EmailTB.IsEnabled = b;
            DepartmentsCB.IsEnabled = b;
        }

        private void ValidateForm()
        {
            // 1) gather all the fields you care about
            var textBoxes = new[] { InitialsTB, NameTB, EmailTB, PhoneNumberTB };
            var comboBoxes = new[] { DepartmentsCB, UsersCB };

            // 2) check each TextBox: non‑empty and not placeholder
            bool textsOk = textBoxes.All(tb =>
            {
                var txt = tb.Text ?? "";
                return !string.IsNullOrWhiteSpace(txt) && txt != GetPlaceholderText(tb);
            });

            // 3) check each ComboBox: has a selected item that isn’t the placeholder
            bool combosOk = comboBoxes.All(cb =>
            {
                var sel = cb.SelectedItem as string;
                return sel != null
                    && sel != "--Select Department--"
                    && sel != "--Select User--";
            });

            // 4) finally enable/disable your buttons
            bool enable = textsOk && combosOk;
            ConfirmBTN.IsEnabled = enable;
        }

        private void AnyField_Changed(object sender, RoutedEventArgs e)
        {
            ValidateForm();
        }


        #endregion
    }
}
