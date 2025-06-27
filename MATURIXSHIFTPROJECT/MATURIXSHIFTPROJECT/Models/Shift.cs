namespace MATURIXSHIFTPROJECT.Models
{
    public class Shift
    {
        public int ShiftID { get; set; }

        private DateTime _date = DateTime.Now;

        public DateOnly Date { get; set; }

        public Category Category { get; set; }

        public Employee Employee { get; set; }
        //public DateTime? DateTimeWrapper
        //{
        //    get => Date == default ? (DateTime?)null : Date.ToDateTime(TimeOnly.MinValue);
        //    set
        //    {
        //        if (value.HasValue)
        //            Date = DateOnly.FromDateTime(value.Value);
        //        else
        //            Date = default;

        //        OnPropertyChanged(nameof(DateTimeWrapper));
        //        OnPropertyChanged(nameof(Date)); // if you're binding to Date anywhere else
        //    }
        //}

        //private DateTime _endDate = DateTime.Now;

        //public DateTime EndDate
        //{
        //    get { return _endDate; }
        //    set { _endDate = value; }
        //}

        public TimeOnly EndTime { get; set; } = TimeOnly.MinValue;

        public string EndTimeString
        {
            get => EndTime.ToString("HH\\:mm");
            set
            {
                if (TimeOnly.TryParse(value, out var parsed))
                {
                    EndTime = parsed;
                    //OnPropertyChanged(nameof(EndTime));
                }

                //OnPropertyChanged(nameof(EndTimeString));
            }
        }


        public TimeOnly StartTime { get; set; } = TimeOnly.MinValue;

        //public string StartTimeString { get; set; }
        //public string EndTimeString { get; set; }

        public string StartTimeString
        {
            get => StartTime.ToString("HH\\:mm");
            set
            {
                if (TimeOnly.TryParse(value, out var parsed))
                {
                    StartTime = parsed;
                    //OnPropertyChanged(nameof(StartTime));
                }

                //OnPropertyChanged(nameof(StartTimeString));
            }
        }

        //public string ShiftTime;

        public string DateString;
        //public override string ToString()
        //{
        //    return $"Date for shift: {Date.ToShortDateString()}, Start time: {StartTime} End time: {EndTime}, End date: {EndDate.ToShortDateString()}";
        //}

        //public event PropertyChangedEventHandler? PropertyChanged;

        //protected void OnPropertyChanged([CallerMemberName] string name = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //}

        public Shift(DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            //ShiftTime = $"{StartTimeString} - {EndTimeString}";
            //DateString = new string($"{Date.ToString()}");
        }


        public Shift()
        {
            //ShiftTime = new string($"{StartTimeString} - {EndTimeString}");
            //DateString = $"{Date.ToString()}";
        }
    }
}
