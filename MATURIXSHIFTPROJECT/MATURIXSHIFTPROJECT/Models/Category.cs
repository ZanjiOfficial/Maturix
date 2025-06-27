namespace MATURIXSHIFTPROJECT.Models
{
    public class Category
    {
        public int Category_ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static Category Job => new Category { Category_ID = 1, Name = "Job" };
        public static Category Event => new Category { Category_ID = 2, Name = "Event" };
        public static Category Workshop => new Category { Category_ID = 3, Name = "Workshop" };

        public override string ToString()
        {
            return Name;
        }
    }
}
