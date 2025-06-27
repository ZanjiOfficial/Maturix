//using MATURIXSHIFTPROJECT.Models;
//using MATURIXSHIFTPROJECT.Persistence;
//using MATURIXSHIFTPROJECT.ViewModels;
//using System.Windows.Input;

//namespace MATURIXSHIFTPROJECT.Commands
//{
//    public class NewShiftCommand : ICommand
//    {
//        private readonly NewShiftViewModel _makeNewShiftViewModel;
//        private readonly ShiftRepository _makeNewShiftRepository;
//        public event EventHandler? CanExecuteChanged;

//        public bool CanExecute(object? parameter)
//        {
//            return true;
//        }

//        public NewShiftCommand(NewShiftViewModel makeNewShiftViewModel, ShiftRepository makeNewShiftRepository)
//        {
//            _makeNewShiftViewModel = makeNewShiftViewModel;
//            _makeNewShiftRepository = makeNewShiftRepository ?? throw new ArgumentNullException(nameof(makeNewShiftRepository));
//        }

//        public void Execute(object? parameter)
//        {
//            if (parameter is NewShiftViewModel mvm)
//            {

//                Shift shift = new Shift(
//                    _makeNewShiftViewModel.CurrentShift.Date,
//                    _makeNewShiftViewModel.CurrentShift.StartTime,
//                    _makeNewShiftViewModel.CurrentShift.EndTime
//                    );

//                if (mvm.CheckBoxIsChecked == false)
//                {

//                    _makeNewShiftRepository.Create(shift);
//                }
//                else
//                    while (_makeNewShiftViewModel.CurrentShift.Date < _makeNewShiftViewModel.SelectedRepeatEnd)
//                    {
//                        _makeNewShiftViewModel.CurrentShift.Date = _makeNewShiftViewModel.CurrentShift.Date.AddDays(7);
//                        int i = 7;
//                        Shift repeatShift = new Shift(
//                            _makeNewShiftViewModel.CurrentShift.Date.AddDays(i),
//                            _makeNewShiftViewModel.CurrentShift.StartTime,
//                           _makeNewShiftViewModel.CurrentShift.EndTime
//                            );
//                        _makeNewShiftRepository.Create(repeatShift);
//                        i = +7;
//                    }

//            }


//        }
//    }
//}
