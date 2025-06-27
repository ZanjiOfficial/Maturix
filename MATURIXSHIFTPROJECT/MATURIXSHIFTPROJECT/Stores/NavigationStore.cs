using MATURIXSHIFTPROJECT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATURIXSHIFTPROJECT.Stores
{
    public class NavigationStore
    {
        private BaseViewModel? _currentViewModel;

        public event Action? CurrentViewModelChanged;
        public BaseViewModel CurrentViewModel
        {
            get { return _currentViewModel ?? throw new InvalidOperationException("CurrentViewModel is not set."); }
            set { _currentViewModel = value; OnCurrentViewModelChanged(); }
        }

        public void OnCurrentViewModelChanged() => CurrentViewModelChanged?.Invoke();
    }

}
