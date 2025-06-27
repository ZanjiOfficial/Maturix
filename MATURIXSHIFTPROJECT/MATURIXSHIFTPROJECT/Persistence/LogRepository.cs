using MATURIXSHIFTPROJECT.Interfaces;
using MATURIXSHIFTPROJECT.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Diagnostics;

namespace MATURIXSHIFTPROJECT.Persistence
{
    public class LogRepository : Irepository<Log>
    {

        #region Properties
        public List<Log> Logs { get; set; }
        private readonly string? ConnectionString;
        #endregion

        public LogRepository()
        {

            #region Initializing
            IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            ConnectionString = config.GetConnectionString("MyDBConnection");
            Logs = GetAll(); // Is used to sync local list with db
            #endregion

        }

        #region Irepository Signature
        public void Create(Log entity) // Ive changed a bit on the Create method so it follows the Irepository signature. Have added shift prop in log class.
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO LOG (Date,Description,Shift_ID) " + "VALUES(@Date,@Description,@Shift_ID)" + "SELECT @@IDENTITY", con))
                {
                    cmd.Parameters.Add("@Date", SqlDbType.Date).Value = entity.Date;
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = entity.Description;
                    cmd.Parameters.Add("@Shift_ID", SqlDbType.Int).Value = entity.ShiftID;
                    entity.LogID = Convert.ToInt32(cmd.ExecuteScalar());
                    Logs.Add(entity);
                }
            }
        }

        public void Update(Log entity)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Log SET Date = @Date, Description= @Description WHERE LogID = @LogID", con);
                cmd.Parameters.Add("@Date", SqlDbType.NVarChar).Value = entity.Date;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = entity.Description;
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(object id)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Log WHERE LogID = @LogID", con);
                cmd.Parameters.AddWithValue("@LogID", id);
                cmd.ExecuteNonQuery();
            }
        }

        public Log Read(object id)
        {
            Log log = new Log();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Log Where Log_ID = @LogID", con);
                cmd.Parameters.AddWithValue("@Log_ID", id);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        log.LogID = dr.GetInt32(0);

                        log.Date = DateOnly.FromDateTime(dr.GetDateTime(1));
                        log.Description = dr.GetString(2);
                        log.ShiftID = dr.GetInt32(3);

                    }
                }
            }
            return log;
        }

        public List<Log> GetAll()
        {
            List<Log> logs = new List<Log>();


            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Log", con))
                {

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            DateTime dt = dr.GetDateTime(1);

                            Log log = new Log()
                            {
                                LogID = dr.GetInt32(0),

                                Date = DateOnly.FromDateTime(dt),
                                Description = dr.GetString(2),
                                ShiftID = dr.GetInt32(3)
                            };


                            logs.Add(log);
                        }
                    }
                }
            }
            foreach (var logdd in logs)
            {
                Debug.WriteLine($"REPO: {logdd.Description}");
            }
            return logs;

        }


        public Log GetLogByShiftID(object id)
        {
            Log log = new Log();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Log Where Shift_ID = @ShiftID", con);
                cmd.Parameters.AddWithValue("@ShiftID", id);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        log.LogID = dr.GetInt32(0);
                        log.Date = DateOnly.FromDateTime(dr.GetDateTime(1));
                        log.Description = dr.GetString(2);
                        log.ShiftID = dr.GetInt32(3);
                    }
                }
            }
            return log;
        }
        #endregion
    }
}
