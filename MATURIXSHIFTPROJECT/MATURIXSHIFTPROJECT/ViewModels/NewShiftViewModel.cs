using MATURIXSHIFTPROJECT.Commands;
using MATURIXSHIFTPROJECT.Models;
using MATURIXSHIFTPROJECT.Persistence;
using MATURIXSHIFTPROJECT.Stores;
using System.Collections.ObjectModel;
using System.Windows;

namespace MATURIXSHIFTPROJECT.ViewModels
{
    public class NewShiftViewModel : BaseViewModel
    {
        #region Properties
        private readonly NavigationStore _navigationStore;

        private readonly ShiftRepository ShiftRepository = new ShiftRepository();

        private readonly EmployeeRepository EmployeeRepository = new EmployeeRepository();

        private readonly CategoryRepository CategoryRepository = new CategoryRepository();
        public ObservableCollection<string> Employees { get; set; }
        public string SelectEmployee { get; set; }

        public RelayCommand SubmitClickCommand { get; }
        public TimeOnly StartTime
        {
            get => CurrentShift.StartTime;
            set
            {
                if (CurrentShift.StartTime != value)
                {
                    CurrentShift.StartTime = value;
                    OnPropertyChanged(nameof(StartTime));
                }
            }
        }

        public TimeOnly EndTime
        {
            get => CurrentShift.EndTime;
            set
            {
                if (CurrentShift.EndTime != value)
                {
                    CurrentShift.EndTime = value;
                    OnPropertyChanged(nameof(EndTime));
                }
            }
        }

        public DateTime? DateTimeWrapper
        {
            get => CurrentShift.Date == default ? (DateTime?)null : CurrentShift.Date.ToDateTime(TimeOnly.MinValue);
            set
            {
                if (value.HasValue)
                    CurrentShift.Date = DateOnly.FromDateTime(value.Value);
                else
                    CurrentShift.Date = default;

                OnPropertyChanged(nameof(DateTimeWrapper));
                OnPropertyChanged(nameof(CurrentShift.Date)); // if you're binding to Date anywhere else
            }
        }

        //IsFormValid checks if all the required information for the creation of a new shift is present, before enabling the Confirm button
        private bool IsFormValid()
        {
            return !string.IsNullOrEmpty(SelectEmployee) &&
                   !string.IsNullOrEmpty(SelectedCategory) &&
                   CurrentShift != null &&
                   CurrentShift.Date != default &&
                   CurrentShift.StartTime != default &&
                   CurrentShift.EndTime != default &&
                   (!CheckBoxIsChecked || (CheckBoxIsChecked && SelectedRepeatEndDateTime.HasValue && SelectedRepeatEndDateTime.Value.Date > CurrentShift.Date.ToDateTime(TimeOnly.MinValue).Date));
        }

        public Category Category { get; set; }
        public Shift Shift { get; }
        //public bool RepeatEveryWeek { get; set; } = false;
        //public DateTime RepeatUntil { get; set; } = DateTime.Now;
        public ObservableCollection<string> Categories { get; set; }

        private DateOnly _selectedRepeatEnd;
        public DateOnly SelectedRepeatEnd
        {
            get => _selectedRepeatEnd;
            set
            {
                _selectedRepeatEnd = value;
                OnPropertyChanged(nameof(SelectedRepeatEnd));
            }
        }

        private Shift _currentShift;
        public Shift CurrentShift
        {
            get => _currentShift;
            set
            {
                _currentShift = value;
                OnPropertyChanged(nameof(CurrentShift));
                OnPropertyChanged(nameof(SubmitClickCommand));
            }
        }

        private bool _checkBoxIsChecked;
        public bool CheckBoxIsChecked
        {
            get => _checkBoxIsChecked;
            set
            {
                _checkBoxIsChecked = value;
                OnPropertyChanged(nameof(CheckBoxIsChecked));
                OnPropertyChanged(nameof(SubmitClickCommand));
            }
        }

        private string _selectedCategory = "--Select Department--";

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

        private DateTime? _selectedRepeatEndDateTime = DateTime.Today;


        public DateTime? SelectedRepeatEndDateTime
        {
            get => _selectedRepeatEndDateTime;
            set
            {
                if (_selectedRepeatEndDateTime != value)
                {
                    _selectedRepeatEndDateTime = value;
                    OnPropertyChanged(nameof(SelectedRepeatEndDateTime));
                    OnPropertyChanged(nameof(SubmitClickCommand));
                }
            }

        }


        public string StartTimeString
        {
            get => CurrentShift.StartTime.ToString("HH\\:mm");
            set
            {
                if (TimeOnly.TryParse(value, out var parsed))
                {
                    CurrentShift.StartTime = parsed;
                    OnPropertyChanged(nameof(StartTimeString));
                    OnPropertyChanged(nameof(StartTime)); // in case StartTime is also bound
                }
            }
        }

        public string EndTimeString
        {
            get => CurrentShift.EndTime.ToString("HH\\:mm");
            set
            {
                if (TimeOnly.TryParse(value, out var parsed))
                {
                    CurrentShift.EndTime = parsed;
                    OnPropertyChanged(nameof(EndTimeString));
                    OnPropertyChanged(nameof(EndTime));
                }
            }
        }

        public string ShiftTime => $"{StartTimeString} - {EndTimeString}";

        public string DateString => CurrentShift.Date.ToString("yyyy-MM-dd"); // Or another format

        #endregion

        public NewShiftViewModel(NavigationStore navigationStore)
        {
            #region Initalising 

            CurrentShift = new Shift();
            {
                CurrentShift.Date = DateOnly.FromDateTime(DateTime.Today);
            }
            SubmitClickCommand = new RelayCommand(execute => SubmitClick(), canExecute => IsFormValid());
            _navigationStore = navigationStore;
            Category = new Category();
            Categories = new ObservableCollection<string> { Category.Job.Name, Category.Event.Name, Category.Workshop.Name };
            EmployeeRepository = new EmployeeRepository();
            Employees = EmployeeRepository.GetAllNames();
            Shift = new Shift();
            Shift.Category = new Category();

            #endregion
        }
        #region Methods


        private void CreateSingleShift(int employeeID, int CategoryID)
        {
            CurrentShift.Employee = EmployeeRepository.Read(employeeID);

            switch (CategoryID)
            {
                case 1:
                    CurrentShift.Category = Category.Job;
                    break;
                case 2:
                    CurrentShift.Category = Category.Event;
                    break;
                case 3:
                    CurrentShift.Category = Category.Workshop;
                    break;
            }

            //ShiftRepository.Create(CurrentShift, employeeID, CategoryID); // OLD
            ShiftRepository.Create(CurrentShift);

            MessageBox.Show("Shift created");
        }

        private void CreateRepeatShifts(int employeeID, int CategoryID)
        {
            DateOnly currentDate = CurrentShift.Date;

            while (currentDate <= SelectedRepeatEnd)
            {
                Shift repeatShift = new Shift(
                    currentDate,
                    CurrentShift.StartTime,
                    CurrentShift.EndTime
                );

                repeatShift.Employee = EmployeeRepository.Read(employeeID);

                switch (CategoryID)
                {
                    case 1:
                        repeatShift.Category = Category.Job;
                        break;
                    case 2:
                        repeatShift.Category = Category.Event;
                        break;
                    case 3:
                        repeatShift.Category = Category.Workshop;
                        break;
                }

                // ShiftRepository.Create(repeatShift, employeeID, CategoryID); OLD
                ShiftRepository.Create(repeatShift); // NEW

                currentDate = currentDate.AddDays(7);
            }

            MessageBox.Show($"Shifts created up to and including {SelectedRepeatEnd}");
        }
        public void SubmitClick()
        {
            try
            {
                int employeeID = EmployeeRepository.GetId(SelectEmployee);
                int CategoryID = CategoryRepository.GetCategoryIDByName(SelectedCategory);

                if (!CheckBoxIsChecked)
                {
                    CreateSingleShift(employeeID, CategoryID);
                    return;
                }

                if (!SelectedRepeatEndDateTime.HasValue ||
                    SelectedRepeatEndDateTime.Value.Date <= CurrentShift.Date.ToDateTime(TimeOnly.MinValue).Date)
                {
                    MessageBox.Show("Please select a valid end date that is after the shift start date.");
                    return;
                }

                SelectedRepeatEnd = DateOnly.FromDateTime(SelectedRepeatEndDateTime.Value);

                CreateRepeatShifts(employeeID, CategoryID);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong while creating shift(s).");
            }
        }
        #endregion
    }
}
