using MATURIXSHIFTPROJECT.Models;
using MATURIXSHIFTPROJECT.Persistence;
using MATURIXSHIFTPROJECT.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Primitives;

namespace MATURIXSHIFTPROJECT.Views
{
    /// <summary>
    /// Interaction logic for CreateEmployeeView.xaml
    /// </summary>
    public partial class CreateEmployeeView : UserControl
    {
        public CreateEmployeeView()
        {
            InitializeComponent();
        }

        #region PlaceHolderText
        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            comboBox.Foreground = Brushes.Gray;
            comboBox.SelectedItem = "--Select Department--";
        }

        private void DepartmentCB_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
            }
        }

        private void TBoxClicked(object sender, RoutedEventArgs e)
        {
            var TBox = sender as TextBox;
            if (TBox != null)
            {
                // Set text color to black when the user clicks inside the TextBox
                TBox.Foreground = Brushes.Black;

                // Clear the text if the user clicks (focuses) on the TextBox
                if (TBox.Text == GetPlaceholderText(TBox))
                {
                    TBox.Text = string.Empty;
                }
            }
        }

        private void LostFocusTB(object sender, RoutedEventArgs e)
        {
            var TBox = sender as TextBox;
            if (TBox != null && string.IsNullOrEmpty(TBox.Text))
            {
                // If text is empty, load the placeholder text again
                LoadTB(TBox);
            }
        }

        public void LoadTB(TextBox TBox)
        {
            if (TBox != null)
            {
                // Set text color to gray for placeholder text
                TBox.Foreground = Brushes.Gray;

                // Load the correct placeholder text based on the TextBox name
                TBox.Text = GetPlaceholderText(TBox);
            }
        }

        // Helper method to return the correct placeholder text
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

        private void ValidateForm()
        {
            // 1) gather all the fields you care about
            var textBoxes = new[] { InitialsTB, NameTB, EmailTB, PhoneNumberTB };
            var comboBoxes = new[] { DepartmentCB };
            var SelectedItem = comboBoxes[0].SelectedItem as string;

            if (SelectedItem != "--Select Department--")
            {
                comboBoxes[0].Foreground = Brushes.Black;
            }
            else
            {
                comboBoxes[0].Foreground = Brushes.Gray;
            }


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
                    && sel != "--Select Department--";
            });

            // 4) finally enable/disable your buttons
            bool enable = textsOk && combosOk;
            SubmitBTN.IsEnabled = enable;
        }

        private void AnyField_Changed(object sender, RoutedEventArgs e)
        {
            ValidateForm();
        }
        #endregion


    }
}
