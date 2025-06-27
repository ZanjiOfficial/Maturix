using MATURIXSHIFTPROJECT.Stores;
using MATURIXSHIFTPROJECT.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MATURIXSHIFTPROJECT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button? _activeButton;
        private readonly NavigationStore _navigationStore;
        public MainViewModel mvm { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            BtnBackground(HomeBtn);
            _navigationStore = new NavigationStore();
            _navigationStore.CurrentViewModel = new HomeViewModel(_navigationStore);
            mvm = new MainViewModel(_navigationStore,this);
            DataContext = mvm;
            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
        }

        // This is just to change visuals when button is clicked.
        #region button visuals
        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            BtnBackground(HomeBtn);
        }
        private void NewShiftBtn_Click(object sender, RoutedEventArgs e)
        {
            BtnBackground(NewShiftBtn);
        }
        private void ContactBtn_Click(object sender, RoutedEventArgs e)
        {
            BtnBackground(ContactBtn);
        }
        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            BtnBackground(SettingsBtn);
        }

        private void Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;  

            btn.RenderTransformOrigin = new Point(0.5, 0.5);

            // Check if it already has a ScaleTransform
            if (btn.RenderTransform == null || !(btn.RenderTransform is ScaleTransform))
                btn.RenderTransform = new ScaleTransform(1, 1);

            var anim = new DoubleAnimation()
            {
                To = 0.95, 
                Duration = TimeSpan.FromSeconds(0.2)
            };

            ((ScaleTransform)btn.RenderTransform).BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            ((ScaleTransform)btn.RenderTransform).BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }


        private void Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;

            btn.RenderTransformOrigin = new Point(0.5, 0.5);


            if (btn.RenderTransform == null || !(btn.RenderTransform is ScaleTransform))
                btn.RenderTransform = new ScaleTransform(1, 1);

            var anim = new DoubleAnimation()
            {
                To = 1,  
                Duration = TimeSpan.FromSeconds(0.2)
            };

            ((ScaleTransform)btn.RenderTransform).BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            ((ScaleTransform)btn.RenderTransform).BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }


        public void BtnBackground(Button button)
        {
      
            if (_activeButton != null)
            {
                _activeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1c0f00"));
            }


            button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Black"));
            _activeButton = button;
        }
        #endregion




    }
}