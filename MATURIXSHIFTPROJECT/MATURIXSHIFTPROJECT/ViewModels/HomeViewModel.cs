using MATURIXSHIFTPROJECT.Commands;
using MATURIXSHIFTPROJECT.Models;
using MATURIXSHIFTPROJECT.Persistence;
using MATURIXSHIFTPROJECT.Stores;
using MATURIXSHIFTPROJECT.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MATURIXSHIFTPROJECT.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        #region Properties
        private Employee _selectedEmployee;
        public ObservableCollection<string> DayWeekMonth { get; set; }

        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;

                if (_selectedEmployee.EmployeeID == 0)
                {
                    showAllShift();
                    ApplyCombinedFilters(SelectedEmployee.EmployeeID);
                }
                else
                {
                    ApplyCombinedFilters(SelectedEmployee.EmployeeID);
                }
                OnPropertyChanged(nameof(SelectedEmployee));


            }
        }
        private int _selectedShiftCount;


        public int SelectedShiftCount
        {
            get { return _selectedShiftCount; }
            set
            {
                if (_selectedShiftCount != value)
                {
                    _selectedShiftCount = value;
                    OnPropertyChanged(nameof(SelectedShiftCount));

                }
            }
        }



        private string _SelectedFilter;
        public string SelectedFilter
        {
            get => _SelectedFilter;
            set
            {

                _SelectedFilter = value;


                if (!string.IsNullOrEmpty(_SelectedFilter))
                {
                    ApplyCombinedFilters(SelectedEmployee.EmployeeID);
                }
                else
                {
                    showAllShift();
                }

                OnPropertyChanged(nameof(SelectedFilter));
                OnPropertyChanged(nameof(CurrentSpanLabel)); // Notify UI

                ResetNavigationDate();


            }
        }

        private DateTime _navigationDate = DateTime.Today;
        public DateTime NavigationDate
        {
            get => _navigationDate;
            set
            {
                _navigationDate = value;
                OnPropertyChanged(nameof(NavigationDate));
                OnPropertyChanged(nameof(CurrentSpanLabel));
                ApplyCombinedFilters(SelectedEmployee.EmployeeID);
            }
        }


        private readonly NavigationStore _navigationStore;
        private ShiftRepository shiftRepo { get; }
        public Shift Shift { get; private set; }
        public ObservableCollection<Shift> Shifts { get; private set; }

        public List<Shift> AllShifts { get; set; } = new List<Shift>();


        private LogRepository logRepository { get; }
        public Log log { get; private set; }
        public ObservableCollection<Log> Logs { get; private set; }
        public List<Log> AllLogs { get; set; } = new List<Log>();

        private ObservableCollection<Shift> _selectedShifts;


        private Shift _selectedShift;
        public Shift SelectedShift
        {
            get => _selectedShift;
            set
            {
                _selectedShift = value;
                OnPropertyChanged(nameof(SelectedShift));
                //LoadLogForSelectedShift();

            }
        }
        //private LogRepository _logRepo = new LogRepository(); // Initialize as needed

        //private void LoadLogForSelectedShift()
        //{
        //    if (SelectedShift != null /*&& SelectedShiftCount !<= 1*/)
        //    {
        //        var log = _logRepo.GetLogByShiftID(SelectedShift.ShiftID); // This should return a Log object
        //        SelectedShiftLogDescription = log?.Description ?? "No log found for this shift.";
        //    }
        //    else
        //    {
        //        SelectedShiftLogDescription = string.Empty;
        //        //OnPropertyChanged(nameof(SelectedShiftLogDescription));
        //    }
        //}



        private Log _selectedLog;
        public Log SelectedLog
        {
            get => _selectedLog;
            set
            {
                _selectedLog = value;
                OnPropertyChanged();
            }
        }


        public List<Employee> Employees { get; set; }

        private EmployeeRepository employeeRepo;

        public ICommand NavigateToCreateLogView { get; }
        public ICommand NavigateToEditShiftView { get; }
        public ICommand PreviousCommand { get; }
        public ICommand NextCommand { get; }

        public RelayCommand DeleteBtnCommand { get; }

        public RelayCommand CreateLogCommand { get; }
        public RelayCommand EditShiftCommand { get; }
        public RelayCommand OpenLogDescriptionCommand { get; }
        #endregion

        public HomeViewModel(NavigationStore navigationStore)
        {
            #region Initializing
            EditShiftCommand = new RelayCommand(execute => EditShiftBtnClick(), canExecute => SelectedShift != null && SelectedShiftCount == 1);
            CreateLogCommand = new RelayCommand(execute => CreateLogeButtomClick(), canExecute => SelectedShift != null && SelectedShiftCount == 1 /*&& SelectedShiftLogDescription == "No log found for this shift."*/);
            NextCommand = new RelayCommand(_ => OnNext());
            PreviousCommand = new RelayCommand(_ => OnPrevious());
            OpenLogDescriptionCommand = new RelayCommand(_ => OpenLogDescription());
            DeleteBtnCommand = new RelayCommand(DeleteBtnClick, CanDelete);

            employeeRepo = new EmployeeRepository();
            shiftRepo = new ShiftRepository();
            logRepository = new LogRepository();
            log = new Log();
            Shift = new Shift();
            Shifts = shiftRepo.GetAllCollection();
            AllShifts = Shifts.ToList();
            AllLogs = logRepository.GetAll();
            Logs = new ObservableCollection<Log>(AllLogs);
            Employees = employeeRepo.GetAll();
            employeeRepo.EnsureDepartmentsSeeded();
            shiftRepo.EnsureCategoriesSeeded();

            Employees.Insert(0, new Employee
            {
                EmployeeID = 0,
                Initials = "Show All"

            });

            SelectedEmployee = Employees[0];

            _navigationStore = navigationStore;
            NavigateToCreateLogView = new NavigateCommand<CreateLogViewModel>(_navigationStore, () => new CreateLogViewModel(_navigationStore, Shift));
            NavigateToEditShiftView = new NavigateCommand<EditShiftViewModel>(_navigationStore, () => new EditShiftViewModel(_navigationStore, Shift));



            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Shifts);
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(nameof(Shift.Date), ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription(nameof(Shift.StartTime), ListSortDirection.Ascending));
            DayWeekMonth = new ObservableCollection<string>() { "Day", "Week", "Month" };


            SelectedFilter = "Week";

            foreach (var log in Logs)
            {
                Debug.WriteLine($"HOMEVIEW: {log.Description}");
            }
            #endregion
        }

        #region Methods

        public void DeleteBtnClick(object parameter)
        {


            if (parameter is ListView listView && listView.SelectedItems != null && listView.SelectedItems.Count > 0)
            {
                var shiftsToDelete = listView.SelectedItems.OfType<Shift>().ToList();
                var shiftIDsToDelete = shiftsToDelete.Select(s => s.ShiftID).ToList();

                if (shiftIDsToDelete.Any())
                {
                    shiftRepo.DeleteMultiple(shiftIDsToDelete);


                    foreach (var shiftToDelete in shiftsToDelete)
                    {
                        Shifts.Remove(shiftToDelete);

                        AllShifts.RemoveAll(s => s.ShiftID == shiftToDelete.ShiftID);
                    }

                    MessageBox.Show($"{shiftsToDelete.Count} shifts deleted!");
                }

                listView.SelectedItems.Clear();

            }
            else if (SelectedShift != null)
            {
                shiftRepo.Delete(SelectedShift.ShiftID);
                Shifts.Remove(SelectedShift);

                AllShifts.RemoveAll(s => s.ShiftID == SelectedShift.ShiftID);
                MessageBox.Show("Shift deleted!");
            }
            else
            {
                MessageBox.Show("Please select one or more shifts to delete.");
                return;
            }

            ApplyCombinedFilters(SelectedEmployee.EmployeeID);

        }
        public void CreateLogeButtomClick()
        {
            Shift selectedShift = SelectedShift;
            shiftRepo.Read(selectedShift.ShiftID);
            _navigationStore.CurrentViewModel = new CreateLogViewModel(_navigationStore, selectedShift);
        }
        public void EditShiftBtnClick()
        {
            Shift selectedShift = SelectedShift;
            shiftRepo.Read(selectedShift.ShiftID);
            _navigationStore.CurrentViewModel = new EditShiftViewModel(_navigationStore, selectedShift);

        }

        public void showAllShift()
        {
            Shifts.Clear();
            var showallshift = shiftRepo.GetAllCollection();
            foreach (Shift shift in showallshift)
            {
                Shifts.Add(shift);
            }
        }

        public void ApplyCombinedFilters(int employeeID)
        {
            DateOnly todayDate = DateOnly.FromDateTime(NavigationDate);
            IEnumerable<Shift> filtered = AllShifts;

            switch (SelectedFilter)
            {
                case "Day":
                    filtered = filtered.Where(x => x.Date == todayDate);
                    break;

                case "Week":
                    int diff = (7 + (NavigationDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                    DateOnly startOfWeek = DateOnly.FromDateTime(NavigationDate.AddDays(-diff));
                    DateOnly endOfWeek = startOfWeek.AddDays(6);
                    filtered = filtered.Where(x => x.Date >= startOfWeek && x.Date <= endOfWeek);
                    break;

                case "Month":
                    filtered = filtered.Where(x => x.Date.Month == todayDate.Month && x.Date.Year == todayDate.Year);
                    break;
            }


            if (employeeID != 0)
            {
                filtered = filtered.Where(x => x.Employee.EmployeeID == employeeID);
            }


            Shifts.Clear();
            foreach (var shift in filtered)
            {
                Shifts.Add(shift);
            }

            var filteredShiftIDs = Shifts.Select(s => s.ShiftID).ToHashSet();
            var filteredLogs = AllLogs.Where(log => filteredShiftIDs.Contains(log.ShiftID)).ToList();

            Logs.Clear();
            foreach (var log in filteredLogs)
            {
                Logs.Add(log);
            }
        }



        public void OnPrevious()
        {
            switch (SelectedFilter)
            {
                case "Day":
                    NavigationDate = NavigationDate.AddDays(-1);
                    ApplyCombinedFilters(SelectedEmployee.EmployeeID);
                    break;
                case "Week":
                    NavigationDate = NavigationDate.AddDays(-7);
                    ApplyCombinedFilters(SelectedEmployee.EmployeeID);
                    break;
                case "Month":
                    NavigationDate = NavigationDate.AddMonths(-1);
                    ApplyCombinedFilters(SelectedEmployee.EmployeeID);
                    break;
            }
        }

        public void OnNext()
        {
            switch (SelectedFilter)
            {
                case "Day":
                    NavigationDate = NavigationDate.AddDays(1);
                    ApplyCombinedFilters(SelectedEmployee.EmployeeID);
                    break;
                case "Week":
                    NavigationDate = NavigationDate.AddDays(7);
                    ApplyCombinedFilters(SelectedEmployee.EmployeeID);
                    break;
                case "Month":
                    NavigationDate = NavigationDate.AddMonths(1);
                    ApplyCombinedFilters(SelectedEmployee.EmployeeID);
                    break;
            }
        }
        public void UpdateShiftSelection(ListView listView)
        {
            if (listView.SelectedItem != null)
            {
                SelectedShiftCount = listView.SelectedItems.Count;
            }
            else
            {
                SelectedShiftCount = 0;
            }
        }

        public string CurrentSpanLabel
        {
            get
            {
                if (SelectedFilter == null) return "";

                switch (SelectedFilter)
                {
                    case "Day":
                        return NavigationDate.ToString("dddd, dd MMMM yyyy");

                    case "Week":
                        var diff = (7 + (NavigationDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                        var weekStart = NavigationDate.AddDays(-diff).Date;
                        var weekEnd = weekStart.AddDays(6);

                        int isoWeekNumber = ISOWeek.GetWeekOfYear(NavigationDate);

                        return $"Week {isoWeekNumber}: {weekStart:dd MMM} – {weekEnd:dd MMM yyyy}";


                    case "Month":
                        var monthStart = new DateTime(NavigationDate.Year, NavigationDate.Month, 1);
                        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                        return $"{monthStart:dd MMM} – {monthEnd:dd MMM yyyy}";

                    default:
                        return "";
                }
            }
        }
        private void ResetNavigationDate()
        {
            switch (SelectedFilter)
            {
                case "Day":
                    NavigationDate = DateTime.Today;
                    break;
                case "Week":
                    // Reset to start of current week (assuming week starts on Monday)
                    int diff = (7 + (DateTime.Today.DayOfWeek - DayOfWeek.Monday)) % 7;
                    NavigationDate = DateTime.Today.AddDays(-1 * diff).Date;
                    break;
                case "Month":
                    NavigationDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    break;
            }
        }

        private bool CanDelete(object parameter)
        {
            return SelectedShift != null;
        }

        private void OpenLogDescription()
        {
            if (SelectedLog == null) return;

            var window = new DescriptionWindow(SelectedLog);
            window.ShowDialog();
        }
        #endregion
    }
}
