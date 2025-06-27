using MATURIXSHIFTPROJECT.Interfaces;
using MATURIXSHIFTPROJECT.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.Data;

namespace MATURIXSHIFTPROJECT.Persistence
{
    public class ShiftRepository : Irepository<Shift>
    {

        #region Properties

        private readonly string? ConnectionString;
        private ObservableCollection<Shift> shiftsCollection { get; set; }
        public List<Shift> Shifts { get; set; } // Local list with all shifts. Gets initialized in constructor.
        private EmployeeRepository employeeRepo { get; set; }

        #endregion

        public ShiftRepository()
        {

            #region Initalizing

            employeeRepo = new EmployeeRepository();
            IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            ConnectionString = config.GetConnectionString("MyDBConnection");
            shiftsCollection = GetAllCollection();
            Shifts = GetAll();

            #endregion

        }

        #region New Methods for interface signature. Needs testing.
        public void Create(Shift entity) // New Create
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO SHIFT (ShiftDate,StartTime,EndTime,Employee_ID,Category_ID) "
                    + "VALUES(@ShiftDate,@StartTime,@EndTime,@Employee_ID,@Category_ID)" + "SELECT @@IDENTITY", con))
                {
                    cmd.Parameters.Add("@ShiftDate", SqlDbType.Date).Value = entity.Date;
                    cmd.Parameters.Add("@StartTime", SqlDbType.Time).Value = entity.StartTime;
                    cmd.Parameters.Add("@EndTime", SqlDbType.Time).Value = entity.EndTime;
                    cmd.Parameters.Add("@Employee_ID", SqlDbType.Int).Value = entity.Employee.EmployeeID;
                    cmd.Parameters.Add("@Category_ID", SqlDbType.Int).Value = entity.Category.Category_ID;
                    entity.ShiftID = Convert.ToInt32(cmd.ExecuteScalar());

                    Shifts.Add(entity);
                    shiftsCollection.Add(entity);
                }
            }
        }

        public void Update(Shift entity) // New Update
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("UPDATE Shift SET ShiftDate = @ShiftDate, StartTime = @StartTime, EndTime =@EndTime , Employee_ID = @Employee_ID, Category_ID =@Category_ID  WHERE Shift_ID = @Shift_ID", con))
                {
                    cmd.Parameters.Add("@ShiftDate", SqlDbType.Date).Value = entity.Date;
                    cmd.Parameters.Add("@StartTime", SqlDbType.Time).Value = entity.StartTime;
                    cmd.Parameters.Add("@EndTime", SqlDbType.Time).Value = entity.EndTime;
                    cmd.Parameters.Add("@Employee_ID", SqlDbType.Int).Value = entity.Employee.EmployeeID;
                    cmd.Parameters.Add("@Category_ID", SqlDbType.Int).Value = entity.Category.Category_ID;
                    cmd.Parameters.Add("@Shift_ID", SqlDbType.Int).Value = entity.ShiftID;

                    cmd.ExecuteNonQuery();
                }
                ;
            }
        }

        public void Delete(object id) // New Delete
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Shift WHERE Shift_ID = @Shift_ID", con))
                {
                    cmd.Parameters.AddWithValue("@Shift_ID", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Shift> GetAll() // Used for initializing local list
        {
            Shift shift = new Shift();
            List<Shift> shifts = new List<Shift>();
            Employee employee = new Employee();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Shift", con);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {

                    while (dr.Read())
                    {

                        employee = employeeRepo.Read(dr.GetInt32(4));
                        shift.ShiftID = dr.GetInt32(0);
                        shift.Date = DateOnly.FromDateTime((DateTime)dr.GetDateTime(1));
                        shift.StartTime = TimeOnly.FromTimeSpan((TimeSpan)dr.GetTimeSpan(2));
                        shift.EndTime = TimeOnly.FromTimeSpan((TimeSpan)dr.GetTimeSpan(3));
                        shift.Employee = employee;
                        shift.Employee.EmployeeID = dr.GetInt32(4);

                        switch (dr.GetInt32(5))
                        {
                            case 1: shift.Category = Category.Job; break;
                            case 2: shift.Category = Category.Event; break;
                            case 3: shift.Category = Category.Workshop; break;
                        }

                        shifts.Add(shift);
                    }
                }
            }
            return shifts;
        }

        public Shift Read(object id)
        {
            Shift shift = new Shift();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Shift_ID, ShiftDate,StartTime,EndTime,Employee_ID,Category_ID FROM Shift Where Shift_ID = @Shift_ID", con))
                {
                    cmd.Parameters.AddWithValue("@Shift_ID", id);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            shift.ShiftID = dr.GetInt32(0);
                            shift.Date = DateOnly.FromDateTime(dr.GetDateTime(1));
                            shift.StartTime = TimeOnly.FromTimeSpan((TimeSpan)dr.GetTimeSpan(2));
                            shift.EndTime = TimeOnly.FromTimeSpan((TimeSpan)dr.GetTimeSpan(3));
                            shift.Employee = employeeRepo.Read(dr.GetInt32(4));

                            switch (dr.GetInt32(5))
                            {
                                case 1: shift.Category = Category.Job; break;
                                case 2: shift.Category = Category.Event; break;
                                case 3: shift.Category = Category.Workshop; break;

                            }
                        }
                    }
                }
            }
            return shift;
        }

        public ObservableCollection<Shift> GetAllCollection() // Not really part of interface signature. But i think it belongs here.
        {
            ObservableCollection<Shift> shifts = new ObservableCollection<Shift>();


            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Shift", con);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {

                    while (dr.Read())
                    {
                        Shift shift = new Shift();
                        Employee employee = new Employee();
                        employee = employeeRepo.Read(dr.GetInt32(4));
                        shift.ShiftID = dr.GetInt32(0);
                        shift.Date = DateOnly.FromDateTime((DateTime)dr.GetDateTime(1));
                        shift.StartTime = TimeOnly.FromTimeSpan((TimeSpan)dr.GetTimeSpan(2));
                        shift.EndTime = TimeOnly.FromTimeSpan((TimeSpan)dr.GetTimeSpan(3));
                        shift.Employee = employee;
                        shift.Employee.EmployeeID = dr.GetInt32(4);

                        switch (dr.GetInt32(5))
                        {
                            case 1: shift.Category = Category.Job; break;
                            case 2: shift.Category = Category.Event; break;
                            case 3: shift.Category = Category.Workshop; break;
                        }

                        shifts.Add(shift);
                    }
                }
            }
            return shifts;
        }
        #endregion

        #region Shift Repository Methods
        public ObservableCollection<Shift> FilterShiftByEmployyeID(int? employeeID)
        {
            ObservableCollection<Shift> shifts = new ObservableCollection<Shift>();
            Employee employee = new Employee();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select * From Shift Where Employee_ID = @Employee_ID", con);
                cmd.Parameters.AddWithValue("@Employee_ID", employeeID);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {

                    while (dr.Read())
                    {
                        Shift shift = new Shift();
                        employee = employeeRepo.Read(dr.GetInt32(4));
                        shift.ShiftID = dr.GetInt32(0);
                        shift.Date = DateOnly.FromDateTime((DateTime)dr.GetDateTime(1));
                        shift.StartTime = TimeOnly.FromTimeSpan((TimeSpan)dr.GetTimeSpan(2));
                        shift.EndTime = TimeOnly.FromTimeSpan((TimeSpan)dr.GetTimeSpan(3));
                        shift.Employee = employee;
                        shift.Employee.EmployeeID = dr.GetInt32(4);

                        switch (dr.GetInt32(5))
                        {
                            case 1: shift.Category = Category.Job; break;
                            case 2: shift.Category = Category.Event; break;
                            case 3: shift.Category = Category.Workshop; break;
                        }

                        shifts.Add(shift);
                    }
                }
            }
            return shifts;
        }
        public void DeleteMultiple(List<int> shiftIDs)
        {
            if (shiftIDs != null && shiftIDs.Any())
            {

                using (SqlConnection con = new SqlConnection(ConnectionString))
                {


                    con.Open();

                    //Delete Log Record
                    string deleteLogQuery = "DELETE FROM Log WHERE Shift_ID IN (" + string.Join(",", shiftIDs) + ")";
                    using (SqlCommand cmd = new SqlCommand(deleteLogQuery, con))
                    {
                        cmd.ExecuteNonQuery();
                    }


                    //Delete Shift Record
                    string query = "DELETE FROM Shift WHERE Shift_ID IN (" + string.Join(",", shiftIDs) + ")";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }



        public void EnsureCategoriesSeeded()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM CATEGORY", con))
                {
                    int deptCount = (int)checkCmd.ExecuteScalar();
                    if (deptCount == 0)
                    {
                        using (SqlCommand seedCmd = new SqlCommand(@"
                            SET IDENTITY_INSERT CATEGORY ON;
                            INSERT INTO CATEGORY (Category_ID, Name) VALUES
                            (1, 'Job'),
                            (2, 'Event'),
                            (3, 'Workshop');
                            SET IDENTITY_INSERT CATEGORY OFF; ", con))
                        {
                            seedCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        #endregion

        //Commented out until new methods has been tested.
        #region Old Interface Method Signature. Commented Out for now.
        //public void Create(Shift shift, int employeeID, int Category_ID)
        //{
        //    using (SqlConnection con = new SqlConnection(ConnectionString))
        //    {
        //        con.Open();
        //        using (SqlCommand cmd = new SqlCommand("INSERT INTO SHIFT (ShiftDate,StartTime,EndTime,Employee_ID,Category_ID) "
        //            + "VALUES(@ShiftDate,@StartTime,@EndTime,@Employee_ID,@Category_ID)" + "SELECT @@IDENTITY", con))
        //        {
        //            cmd.Parameters.Add("@ShiftDate", SqlDbType.Date).Value = shift.Date;
        //            cmd.Parameters.Add("@StartTime", SqlDbType.Time).Value = shift.StartTime;
        //            cmd.Parameters.Add("@EndTime", SqlDbType.Time).Value = shift.EndTime;
        //            cmd.Parameters.Add("@Employee_ID", SqlDbType.Int).Value = employeeID;
        //            cmd.Parameters.Add("@Category_ID", SqlDbType.Int).Value = Category_ID;
        //            shift.ShiftID = Convert.ToInt32(cmd.ExecuteScalar());

        //            Shifts.Add(shift);
        //            shiftsCollection.Add(shift);
        //        }
        //    }
        //}

        //public Shift Read(int ShiftID)
        //{
        //    Employee employee = new Employee();
        //    Shift shift = new Shift();
        //    using (SqlConnection con = new SqlConnection(ConnectionString))
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("SELECT Shift_ID, ShiftDate,StartTime,EndTime,Employee_ID,Category_ID FROM Shift Where Shift_ID = @Shift_ID", con);
        //        cmd.Parameters.AddWithValue("@Shift_ID", ShiftID);

        //        using (SqlDataReader dr = cmd.ExecuteReader())
        //        {
        //            while (dr.Read())
        //            {
        //                shift.ShiftID = dr.GetInt32(0);
        //                shift.Date = DateOnly.FromDateTime(dr.GetDateTime(1));
        //                shift.StartTime = TimeOnly.FromTimeSpan((TimeSpan)dr.GetTimeSpan(2));
        //                shift.EndTime = TimeOnly.FromTimeSpan((TimeSpan)dr.GetTimeSpan(3));
        //                employee = employeeRepo.Read(dr.GetInt32(4));

        //                switch (dr.GetInt32(5))
        //                {
        //                    case 1: shift.Category = Category.Job; break;
        //                    case 2: shift.Category = Category.Event; break;
        //                    case 3: shift.Category = Category.Workshop; break;

        //                }
        //            }
        //        }
        //    }
        //    return shift;
        //}

        //public void Update(Shift shift, int employeeID, int Category_ID)
        //{
        //    using (SqlConnection con = new SqlConnection(ConnectionString))
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("UPDATE Shift SET ShiftDate = @ShiftDate, StartTime = @StartTime, EndTime =@EndTime , Employee_ID = @Employee_ID, Category_ID =@Category_ID  WHERE Shift_ID = @Shift_ID", con);
        //        cmd.Parameters.Add("@ShiftDate", SqlDbType.Date).Value = shift.Date;
        //        cmd.Parameters.Add("@StartTime", SqlDbType.Time).Value = shift.StartTime;
        //        cmd.Parameters.Add("@EndTime", SqlDbType.Time).Value = shift.EndTime;
        //        cmd.Parameters.Add("@Employee_ID", SqlDbType.Int).Value = employeeID;
        //        cmd.Parameters.Add("@Category_ID", SqlDbType.Int).Value = Category_ID;
        //        cmd.Parameters.Add("@Shift_ID", SqlDbType.Int).Value = shift.ShiftID;


        //        cmd.ExecuteNonQuery();
        //    }
        //}

        //public void Delete(int? ShiftID)
        //{
        //    using (SqlConnection con = new SqlConnection(ConnectionString))
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("DELETE FROM Shift WHERE Shift_ID = @Shift_ID", con);
        //        cmd.Parameters.AddWithValue("@Shift_ID", ShiftID);
        //        cmd.ExecuteNonQuery();
        //    }
        //}
        #endregion // 

    }
}
