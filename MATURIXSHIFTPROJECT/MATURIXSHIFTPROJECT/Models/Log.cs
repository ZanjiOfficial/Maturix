using MATURIXSHIFTPROJECT.Persistence;

namespace MATURIXSHIFTPROJECT.Models
{
    public class Log
    {
        public int LogID { get; set; }
        public DateOnly Date { get; set; }
        public string? Description { get; set; }
        public int ShiftID { get; set; }
        public Shift? Shift { get; set; }
       


        //public Log()
        //{
        //    Shift = new Shift();
        //}
    }
}
