namespace MATURIXSHIFTPROJECT.Models
{
    public class Department
    {
        public int Department_ID { get; set; }
        public string? Name { get; set; }

        public static Department Warehouse => new Department { Department_ID = 1, Name = "Warehouse" };
        public static Department Support => new Department { Department_ID = 2, Name = "Support" };
        public static Department Marketing => new Department { Department_ID = 3, Name = "Marketing" };

        public override string ToString()
        {
            return Name;
        }
    }
}
