using MATURIXSHIFTPROJECT.Commands;
using MATURIXSHIFTPROJECT.Persistence;
using MATURIXSHIFTPROJECT.Stores;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace MATURIXSHIFTPROJECT.ViewModels
{
    public class DeleteEmployeeViewModel : BaseViewModel
    {
        private Window _window;
        private string _SelectedEmployee;

        public string SelectedEmployee
        {
            get { return _SelectedEmployee; }
            set { _SelectedEmployee = value; OnPropertyChanged(nameof(SelectedEmployee)); }
        }

        public ObservableCollection<string> Employee { get; set; }

        private EmployeeRepository employeeRepo;

        private BaseViewModel? _currentViewModel;
        public BaseViewModel? CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }



        public ICommand NavigateToSettingsView { get; }

        public RelayCommand DeleteBtnCommand { get; }

        public DeleteEmployeeViewModel(NavigationStore navigationStore, Window window)
        {
            DeleteBtnCommand = new RelayCommand(execute => DeleteBtnClick());
            _window = window;
            employeeRepo = new EmployeeRepository();
            Employee = employeeRepo.GetAllNames();
            NavigateToSettingsView = new NavigateCommand<SettingViewModel>(navigationStore, () => new SettingViewModel(navigationStore, _window));
        }

        public void DeleteBtnClick()
        {
            try
            {
                int id = employeeRepo.GetId(SelectedEmployee);
                employeeRepo.Delete(id);
                MessageBox.Show($"{SelectedEmployee} deleted!");
                Employee.Remove(SelectedEmployee);
            }
            catch (Exception) { MessageBox.Show(@"Error when deleting user"); }
        }
    }
}
