using MATURIXSHIFTPROJECT.Commands;
using MATURIXSHIFTPROJECT.Models;
using MATURIXSHIFTPROJECT.Persistence;
using MATURIXSHIFTPROJECT.Stores;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;


namespace MATURIXSHIFTPROJECT.ViewModels
{
    public class EditShiftViewModel : BaseViewModel
    {
        // private readonly NavigationStore _navigationStore; JUST COMMENTED OUT

        private EmployeeRepository EmployeeRepository;
        private Window _window { get; }

        private ShiftRepository ShiftRepository;
        private CategoryRepository CategoryRepository;

        public Employee Employee;
        public ObservableCollection<string> Categories { get; set; }
        public ObservableCollection<string> Employees { get; set; }
        public string SelectEmployee { get; set; }

        public ICommand NavigateToHomeView { get; }
        public RelayCommand ConfirmBtnCommand { get; }

        public Category Category { get; set; }
        public Shift Shift { get; set; }

        private string _selectedCategory;

        public string SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                OnPropertyChanged(nameof(Shift.Category));
                switch (value)
                {
                    case "Job":
                        this.Category = Category.Job;
                        Shift.Category = Category.Job;
                        break;
                    case "Event":
                        this.Category = Category.Event;
                        Shift.Category = Category.Event;
                        break;
                    case "Workshop":
                        this.Category = Category.Workshop;
                        Shift.Category = Category.Workshop;
                        break;
                }
            }
        }

        public EditShiftViewModel(NavigationStore navigationStore, Shift selectedShift)
        {   // // Initialize dependencies
            ConfirmBtnCommand = new RelayCommand(execute => ConfirmBtnClick());
            ShiftRepository = new ShiftRepository();
            EmployeeRepository = new EmployeeRepository();
            CategoryRepository = new CategoryRepository();
            // Initialize Models 

            Shift = selectedShift ?? new Shift();
            Category = new Category();
            Employee = new Employee();

            LoadCategories();
            LoadEmployeesToList();

            SelectedCategory = selectedShift.Category?.Name;
            SelectEmployee = selectedShift.Employee.Name;
            NavigateToHomeView = new NavigateCommand<HomeViewModel>(navigationStore, () => new HomeViewModel(navigationStore));

        }



        public DateOnly Date
        {
            get => Shift.Date;
            set
            {
                if (Shift != null)
                {
                    Shift.Date = value;
                    OnPropertyChanged(nameof(Date));

                }
            }
        }
        public DateTime? DateTimeWrapper
        {
            get => Date == default ? (DateTime?)null : Date.ToDateTime(TimeOnly.MinValue);
            set
            {
                if (value.HasValue)
                    Date = DateOnly.FromDateTime(value.Value);
                else
                    Date = default;

                OnPropertyChanged(nameof(DateTimeWrapper));
                OnPropertyChanged(nameof(Date)); // if you're binding to Date anywhere else
            }
        }

        public TimeOnly EndTime
        {
            get => Shift.EndTime;
            set
            {
                if (Shift != null)
                {
                    Shift.EndTime = value;
                    OnPropertyChanged(nameof(EndTime));

                }
            }
        }
        public string EndTimeString
        {
            get => EndTime.ToString("HH\\:mm");
            set
            {
                if (TimeOnly.TryParse(value, out var parsed))
                {
                    EndTime = parsed;
                    OnPropertyChanged(nameof(EndTime));
                }

                OnPropertyChanged(nameof(EndTimeString));
            }
        }
        public TimeOnly StartTime
        {
            get => Shift.StartTime;
            set
            {
                if (Shift != null)
                {
                    Shift.StartTime = value;
                    OnPropertyChanged(nameof(StartTime));

                }
            }
        }
        public string StartTimeString
        {
            get => StartTime.ToString("HH\\:mm");
            set
            {
                if (TimeOnly.TryParse(value, out var parsed))
                {
                    StartTime = parsed;
                    OnPropertyChanged(nameof(StartTime));
                }

                OnPropertyChanged(nameof(StartTimeString));
            }
        }


        private void LoadEmployeesToList()
        {
            Employees = EmployeeRepository.GetAllNames();
        }
        private void LoadCategories()
        {
            Categories = new ObservableCollection<string>
            {
                Category.Job.Name,
                Category.Event.Name,
                Category.Workshop.Name,
            };

        }
        public void ConfirmBtnClick()
        {
            int employeeID = EmployeeRepository.GetId(SelectEmployee);
            int Category_ID = CategoryRepository.GetCategoryIDByName(SelectedCategory);

            Shift.Employee.EmployeeID = employeeID;
            Shift.Category.Category_ID = Category_ID;

            // ShiftRepository.Update(Shift, employeeID, Category_ID); OLD
            ShiftRepository.Update(Shift); // NEW

            MessageBox.Show("Shift updated!");

        }

    }
}
