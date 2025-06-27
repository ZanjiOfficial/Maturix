namespace MATURIXSHIFTPROJECT.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string? Initials { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public Department? Department { get; set; }

        public Shift? Shift { get; set; }

        public Employee()
        {
            Department = new Department();
            Shift = new Shift();
        }

    }


}
