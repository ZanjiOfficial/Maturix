using CsvHelper;
using CsvHelper.Configuration;
using MATURIXSHIFTPROJECT.Commands;
using MATURIXSHIFTPROJECT.Models;
using MATURIXSHIFTPROJECT.Persistence;
using MATURIXSHIFTPROJECT.Stores;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace MATURIXSHIFTPROJECT.ViewModels
{
    public class SettingViewModel : BaseViewModel
    {
        private Window _mainWindow;
        public ICommand NavigateToCreateEmployeeView { get; }
        public ICommand NavigateToEditEmployeeView { get; }
        public ICommand NavigateToDeleteEmployeeView { get; }
        public RelayCommand ExitAppCommand { get; }
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "shifts.csv");
        public RelayCommand CreateDataFileCommand { get; }
        private ShiftRepository shiftRepo { get; }
        public Shift Shift { get; private set; }
        public ObservableCollection<Shift> Shifts { get; private set; }
        public static List<Shift> AllShifts { get; set; } = new List<Shift>();

        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public SettingViewModel(NavigationStore navigationStore, Window window)
        {
            CreateDataFileCommand = new RelayCommand(execute => WriteToCsv(filePath));
            ExitAppCommand = new RelayCommand(execute => ExitAppClick());
            _mainWindow = window;
            NavigateToCreateEmployeeView = new NavigateCommand<CreateEmployeeViewModel>(navigationStore, () => new CreateEmployeeViewModel(navigationStore, _mainWindow));
            NavigateToEditEmployeeView = new NavigateCommand<EditEmployeeViewModel>(navigationStore, () => new EditEmployeeViewModel(navigationStore, _mainWindow));
            NavigateToDeleteEmployeeView = new NavigateCommand<DeleteEmployeeViewModel>(navigationStore, () => new DeleteEmployeeViewModel(navigationStore, _mainWindow));
            shiftRepo = new ShiftRepository();
            Shifts = shiftRepo.GetAllCollection();
            AllShifts = Shifts.ToList();

        }

        static void WriteToCsv(string baseFilePath)
        {
            try
            {
                
                string directory = Path.GetDirectoryName(baseFilePath);
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(baseFilePath);
                string extension = Path.GetExtension(baseFilePath);
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string uniqueFilePath = Path.Combine(directory, $"{filenameWithoutExtension}_{timestamp}{extension}");

                var configShifts = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    ShouldQuote = (args) =>
                    {
                        var fieldString = args.Field.ToString();
                        return fieldString.Contains(",") ||
                               fieldString.Contains("\"") ||
                               fieldString.Contains("\n");
                    },
                };

                using (StreamWriter streamWriter = new StreamWriter(uniqueFilePath))
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, configShifts))
                {
                    csvWriter.WriteHeader<Shift>();
                    csvWriter.NextRecord();

                    foreach (var shift in AllShifts)
                    {
                        csvWriter.WriteField(shift.ShiftID);
                        csvWriter.WriteField(shift.Employee.Name);
                        csvWriter.WriteField(shift.Category);
                        csvWriter.WriteField(shift.Date.ToString("dd-MM-yyyy"));
                        csvWriter.WriteField(shift.StartTimeString);
                        csvWriter.WriteField(shift.EndTimeString);
                        csvWriter.NextRecord();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing CSV: {ex.Message}");
            }
        }




        public void ExitAppClick()
        {
            //Closing MainWindow. Maybe add some "Window will close, are you sure? Yes/No" function.
            _mainWindow.Close();
        }
    }
}
