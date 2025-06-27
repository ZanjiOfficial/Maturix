using MATURIXSHIFTPROJECT.Commands;
using MATURIXSHIFTPROJECT.Models;
using MATURIXSHIFTPROJECT.Persistence;
using MATURIXSHIFTPROJECT.Stores;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace MATURIXSHIFTPROJECT.ViewModels
{
    public class CreateEmployeeViewModel : BaseViewModel
    {
        #region Properties
        private EmployeeRepository employeeRepository;
        public Employee Employee { get; set; }

        public Department Department { get; set; }

        private string? _selectedDepartment = "--Select Department--";

        public string? SelectedDepartment
        {
            get { return _selectedDepartment; }
            set
            {
                _selectedDepartment = value;
                OnPropertyChanged(nameof(SelectedDepartment));
                switch (value)
                {
                    case "Warehouse":
                        this.Department = Department.Warehouse;
                        Employee.Department = Department.Warehouse;
                        break;
                    case "Support":
                        this.Department = Department.Support;
                        Employee.Department = Department.Support;
                        break;
                    case "Marketing":
                        this.Department = Department.Marketing;
                        Employee.Department = Department.Marketing;
                        break;
                }


            }
        }

        public ObservableCollection<string> Departments { get; }

        private Window _window { get; }

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
        public RelayCommand SubmitClickCommand { get; }
        public ICommand NavigateToSettingsView { get; }
        #endregion

        public CreateEmployeeViewModel(NavigationStore navigationStore, Window window)
        {
            #region Initalising
            SubmitClickCommand = new RelayCommand(execute => SubmitClick());
            _window = window;
            Department = new Department();
            Employee = new Employee();

            Departments = new ObservableCollection<string>() { "--Select Department--", Department.Warehouse.Name, Department.Support.Name, Department.Marketing.Name };
            NavigateToSettingsView = new NavigateCommand<SettingViewModel>(navigationStore, () => new SettingViewModel(navigationStore, _window));
            employeeRepository = new EmployeeRepository();
            #endregion
        }

        #region Methods
        public void SubmitClick()
        {
            if (SelectedDepartment == "--Select Department--" || string.IsNullOrWhiteSpace(SelectedDepartment))
            {
                // Show error or block submit
                MessageBox.Show("Please select a department.");
                return;
            }
            if (Employee.Initials == "Initials" || Employee.Email == "Email" || Employee.Name == "Full Name" || Employee.PhoneNumber == "PhoneNumber")
            {
                MessageBox.Show("Invalid information. Try again.");
                return;
            }
            employeeRepository.Create(Employee);
            MessageBox.Show($"Employee has been Created. Department: {Employee.Department.Name}");
            //{Employee.Department.Department_ID} if needed
            ResetText(Employee);
        }

        //This is to reset all boxes when creating the user is done.
        public void ResetText(Employee employee)
        {
            employee.Initials = string.Empty;
            employee.Name = string.Empty;
            employee.PhoneNumber = string.Empty;
            employee.Email = string.Empty;
            SelectedDepartment = string.Empty;
        }

        #endregion
    }

}
