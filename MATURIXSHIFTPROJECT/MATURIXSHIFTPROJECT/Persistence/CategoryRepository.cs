using MATURIXSHIFTPROJECT.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace MATURIXSHIFTPROJECT.Persistence
{
    public class CategoryRepository // This repo cant implement the interface since it isnt made very traditional. 
    {

        #region Properties
        private readonly string? ConnectionString;
        #endregion

        public CategoryRepository()
        {
            #region Initializing 
            IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            ConnectionString = config.GetConnectionString("MyDBConnection");
            #endregion
        }

        #region Category Repository Methods
        public int GetCategoryIDByName(string Name)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Category_ID FROM CATEGORY WHERE Name = @Name", con))
                {
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = Name;



                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }


                }
            }
            throw new Exception($"Category with Name '{Name}' not found.");
        }
        #endregion
    }
}
