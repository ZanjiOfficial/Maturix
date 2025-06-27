using MATURIXSHIFTPROJECT.Models;
using MATURIXSHIFTPROJECT.Persistence;
using MATURIXSHIFTPROJECT.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATURIXSHIFTPROJECT.ViewModels
{
    public class ContactViewModel : BaseViewModel
    {
        private readonly NavigationStore _navigationStore;
        private EmployeeRepository employeeRepo { get; }
        public Employee Employee { get; private set; }
        public List<Employee> Employees { get; private set; }

        public ContactViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            employeeRepo = new EmployeeRepository();
            Employee = new Employee();
            Employees = employeeRepo.GetAll();

          
           
        }

    }
}
