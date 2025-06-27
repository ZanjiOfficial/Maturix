using MATURIXSHIFTPROJECT.Commands;
using MATURIXSHIFTPROJECT.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MATURIXSHIFTPROJECT.ViewModels
{
    public class MainViewModel : BaseViewModel 
    {
        private Window _window;
        private readonly NavigationStore _navStore;
        public BaseViewModel CurrentViewModel => _navStore.CurrentViewModel;

        public ICommand NavigateToHomeView { get; }
        public ICommand NavigateToNewShiftView { get; }
        public ICommand NavigateToContactView { get; }
        public ICommand NavigateToSettingsView { get; }


        public MainViewModel(NavigationStore navigationStore, Window window)
        {
            _navStore = navigationStore;
            _navStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _window = window;
            NavigateToHomeView = new NavigateCommand<HomeViewModel>(_navStore, () => new HomeViewModel(_navStore));
            NavigateToNewShiftView = new NavigateCommand<NewShiftViewModel>(_navStore, () => new NewShiftViewModel(_navStore));
            NavigateToContactView = new NavigateCommand<ContactViewModel>(_navStore, () => new ContactViewModel(_navStore));
            NavigateToSettingsView = new NavigateCommand<SettingViewModel>(_navStore, () => new SettingViewModel(_navStore,_window));


        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        public void Dispose()
        {
            _navStore.CurrentViewModelChanged -= OnCurrentViewModelChanged;
        }
    }
}
