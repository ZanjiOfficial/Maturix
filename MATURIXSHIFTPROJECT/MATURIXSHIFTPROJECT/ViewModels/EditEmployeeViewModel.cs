using MATURIXSHIFTPROJECT.Commands;
using MATURIXSHIFTPROJECT.Models;
using MATURIXSHIFTPROJECT.Persistence;
using MATURIXSHIFTPROJECT.Stores;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace MATURIXSHIFTPROJECT.ViewModels
{
    public class EditEmployeeViewModel : BaseViewModel
    {
        #region Properties
        #region ViewDatabinding
        public string Initials
        {
            get => Employee?.Initials ?? "";
            set
            {
                if (Employee != null)
                {
                    Employee.Initials = value;
                    OnPropertyChanged(nameof(Initials));
                }
            }
        }

        public string Name
        {
            get => Employee?.Name ?? "";
            set
            {
                if (Employee != null)
                {
                    Employee.Name = value;
                    OnPropertyChanged(nameof(Name));

                }
            }
        }

        public string PhoneNumber
        {
            get => Employee?.PhoneNumber ?? "";
            set
            {
                if (Employee != null)
                {
                    Employee.PhoneNumber = value;
                    OnPropertyChanged(nameof(PhoneNumber));
                }
            }
        }

        public string Email
        {
            get => Employee?.Email ?? "";
            set
            {
                if (Employee != null)
                {
                    Employee.Email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }
        #endregion
        private EmployeeRepository employeeRepository;
        private Employee _employee;

        public Employee Employee
        {
            get { return _employee; }
            set { _employee = value; OnPropertyChanged(nameof(Employee)); }
        }


        public Department Department { get; set; }

        private string _selectedEmployee = "--Select Employee--";
        public string SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));

                if (value != "--Select User--")
                {
                    var loadedEmployee = employeeRepository.Read(employeeRepository.GetId(value));

                    if (loadedEmployee != null)
                    {
                        Employee.EmployeeID = loadedEmployee.EmployeeID;
                        Employee.Initials = loadedEmployee.Initials;
                        Employee.Name = loadedEmployee.Name;
                        Employee.PhoneNumber = loadedEmployee.PhoneNumber;
                        Employee.Email = loadedEmployee.Email;
                        Employee.Department = loadedEmployee.Department;

                        Debug.WriteLine(Employee.Name);
                        Debug.WriteLine(Employee.EmployeeID);


                        OnPropertyChanged(nameof(Initials));
                        OnPropertyChanged(nameof(Name));
                        OnPropertyChanged(nameof(PhoneNumber));
                        OnPropertyChanged(nameof(Email));


                        SelectedDepartment = loadedEmployee.Department?.Name;
                    }
                }
            }
        }


        private string? _selectedDepartment;

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
                Debug.WriteLine($"EMPLOYEE DEPARTMENT NAME {Employee.Department.Name} {Employee.Department.Department_ID}");


            }
        }

        public ObservableCollection<string> Departments { get; }
        public ObservableCollection<string> Employees { get; set; }

        private Window _window { get; }

        public ICommand NavigateToSettingsView { get; }

        public RelayCommand ConfirmBtnCommand { get; }
        //public ICommand ConfirmBtnCmd { get; }
        private bool _isConfirmButtonEnabled = true;
        public bool IsConfirmButtonEnabled
        {
            get => _isConfirmButtonEnabled;
            set
            {
                _isConfirmButtonEnabled = value;
                OnPropertyChanged(nameof(IsConfirmButtonEnabled));
            }
        }
        #endregion
        public EditEmployeeViewModel(NavigationStore navigationStore, Window window)
        {
            ConfirmBtnCommand = new RelayCommand(execute => ConfirmBtnClick());
            _window = window;
            Employee = new Employee();
            Department = new Department();
            Departments = new ObservableCollection<string>() { Department.Warehouse.Name, Department.Support.Name, Department.Marketing.Name };
            employeeRepository = new EmployeeRepository();
            Employees = employeeRepository.GetAllNames();
            Employees.Add("--Select Employee--");
            NavigateToSettingsView = new NavigateCommand<SettingViewModel>(navigationStore, () => new SettingViewModel(navigationStore, _window));
        }

        public void ConfirmBtnClick()
        {
            employeeRepository.Update(Employee);
            MessageBox.Show("Employee updated!");
            IsConfirmButtonEnabled = false;
        }

    }
}
