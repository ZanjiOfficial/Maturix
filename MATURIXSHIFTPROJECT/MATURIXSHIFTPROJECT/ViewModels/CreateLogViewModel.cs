using MATURIXSHIFTPROJECT.Commands;
using MATURIXSHIFTPROJECT.Models;
using MATURIXSHIFTPROJECT.Persistence;
using MATURIXSHIFTPROJECT.Stores;
using System.Windows;
using System.Windows.Input;

namespace MATURIXSHIFTPROJECT.ViewModels
{
    public class CreateLogViewModel : BaseViewModel
    {
        private readonly NavigationStore _navigationStore;
        private LogRepository LogRepository;
        private ShiftRepository ShiftRepository;
        public Log log { get; }
        public string Description { get; set; }
        private Shift _SelectedShiftID;
        public DateOnly SelectedShiftDate { get; set; }
        public DateOnly SelectedLogDate { get; set; }
        public RelayCommand CreateCommand { get; }
        public ICommand NavigateToHomeView { get; }

        public CreateLogViewModel(NavigationStore navigationStore, Shift selectedShift)
        {
            CreateCommand = new RelayCommand(execute => CreateLog());
            _navigationStore = navigationStore;
            LogRepository = new LogRepository();
            log = new Log();
            _SelectedShiftID = selectedShift;
            SelectedShiftDate = selectedShift.Date;
            SelectedLogDate = selectedShift.Date;
            log.Date = SelectedLogDate;
            NavigateToHomeView = new NavigateCommand<HomeViewModel>(navigationStore, () => new HomeViewModel(navigationStore));
            ShiftRepository = new ShiftRepository();

        }

        public CreateLogViewModel()
        {

        }
        public void CreateLog()
        {
            log.ShiftID = _SelectedShiftID.ShiftID;

            // LogRepository.Create(log, _SelectedShiftID.ShiftID); OLD
            LogRepository.Create(log); // NEW

            MessageBox.Show("Log Created");

        }
    }
}
