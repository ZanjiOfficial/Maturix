using MATURIXSHIFTPROJECT.Interfaces;
using MATURIXSHIFTPROJECT.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq.Expressions;
using Xceed.Wpf.AvalonDock.Themes;

namespace MATURIXSHIFTPROJECT.Persistence
{
    public class EmployeeRepository : Irepository<Employee>
    {

        #region Properties
        private List<Employee> Employees { get; set; }
        private readonly string? ConnectionString;
        #endregion

        public EmployeeRepository()
        {

            #region Initializing 
            IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            ConnectionString = config.GetConnectionString("MyDBConnection");
            Employees = GetAll();
            #endregion

        }

        #region Interface Signature
        public void Create(Employee entity)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO EMPLOYEE (Initials, Name, PhoneNumber, Email, Department_ID) "
                    + "VALUES(@Initials,@Name,@PhoneNumber,@Email,@Department_ID)"
                    + "SELECT @@IDENTITY", con))
                {
                    cmd.Parameters.Add("@Initials", SqlDbType.NVarChar).Value = entity.Initials;
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = entity.Name;
                    cmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = entity.PhoneNumber;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = entity.Email;
                    cmd.Parameters.Add("@Department_ID", SqlDbType.Int).Value = entity.Department.Department_ID;
                    entity.EmployeeID = Convert.ToInt32(cmd.ExecuteScalar());

                    Employees.Add(entity);

                }

            }
        }

        public void Update(Employee entity)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE Employee SET Initials = @Initials, Name = @Name, PhoneNumber = @PhoneNumber, Email = @Email, Department_ID = @Department_ID WHERE Employee_ID = @Employee_ID", con))
                {
                    cmd.Parameters.Add("@Initials", SqlDbType.NVarChar).Value = entity.Initials;
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = entity.Name;
                    cmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = entity.PhoneNumber;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = entity.Email;
                    cmd.Parameters.Add("@Department_ID", SqlDbType.Int).Value = entity.Department.Department_ID;
                    cmd.Parameters.Add("@Employee_ID", SqlDbType.Int).Value = entity.EmployeeID;

                    int result = cmd.ExecuteNonQuery();
                }
            }
        }

        //public void Delete(object id)
        //{
        //    using (SqlConnection con = new SqlConnection(ConnectionString))
        //    {
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("DELETE FROM Employee WHERE Employee_ID = @Employee_ID", con);
        //        cmd.Parameters.AddWithValue("@Employee_ID", id);
        //        cmd.ExecuteNonQuery();
        //    }
        //}

        public void Delete(object id)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                //transaction is used to make sure if there is a partial deletion, we can roll back
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        // First delete logs
                        SqlCommand deleteLogsCmd = new SqlCommand(@"DELETE FROM Log WHERE Shift_ID IN (SELECT Shift_ID FROM Shift WHERE Employee_ID = @Employee_ID)", con, transaction);
                        deleteLogsCmd.Parameters.AddWithValue("@Employee_ID", id);
                        deleteLogsCmd.ExecuteNonQuery();

                        // Secondly, delete all Shifts associated with the Employee
                        SqlCommand deleteShiftsCmd = new SqlCommand("DELETE FROM Shift WHERE Employee_ID = @Employee_ID", con, transaction);
                        deleteShiftsCmd.Parameters.AddWithValue("@Employee_ID", id);
                        deleteShiftsCmd.ExecuteNonQuery();

                        // Then, delete the Employee
                        SqlCommand deleteEmployeeCmd = new SqlCommand("DELETE FROM Employee WHERE Employee_ID = @Employee_ID", con, transaction);
                        deleteEmployeeCmd.Parameters.AddWithValue("@Employee_ID", id);
                        deleteEmployeeCmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Deletion failed and was rolled back. Inner error: " + ex.Message);
                    }

                }
            }
        }

        public List<Employee> GetAll()
        {
            List<Employee> employees = new List<Employee>();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Employee", con);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Employee employee = new Employee
                        {
                            EmployeeID = dr.GetInt32(0),
                            Initials = dr.GetString(1),
                            Name = dr.GetString(2),
                            PhoneNumber = dr.GetString(3),
                            Email = dr.GetString(4)
                        };

                        switch (dr.GetInt32(5))
                        {
                            case 1: employee.Department = Department.Warehouse; break;
                            case 2: employee.Department = Department.Support; break;
                            case 3: employee.Department = Department.Marketing; break;
                        }

                        employees.Add(employee);
                    }
                }
            }
            return employees;
        }

        public Employee Read(object id)
        {
            Employee employee = new Employee();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Employee WHERE Employee_ID = @Employee_ID", con))
                {
                    cmd.Parameters.AddWithValue("@Employee_ID", id);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {

                        while (dr.Read())
                        {
                            {
                                employee.EmployeeID = dr.GetInt32(0);
                                employee.Initials = dr.GetString(1);
                                employee.Name = dr.GetString(2);
                                employee.PhoneNumber = dr.GetString(3);
                                employee.Email = dr.GetString(4);



                                switch (dr.GetInt32(5))
                                {
                                    case 1: employee.Department = Department.Warehouse; break;
                                    case 2: employee.Department = Department.Support; break;
                                    case 3: employee.Department = Department.Marketing; break;
                                }

                            }
                            ;
                        }
                    }
                }
            }
            return employee;
        }

        public void EnsureDepartmentsSeeded()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM DEPARTMENT", con))
                {
                    int deptCount = (int)checkCmd.ExecuteScalar();
                    if (deptCount == 0)
                    {
                        using (SqlCommand seedCmd = new SqlCommand(@"
                            SET IDENTITY_INSERT DEPARTMENT ON;
                        INSERT INTO DEPARTMENT(Department_ID, DepartmentName) VALUES
                        (1, 'Warehouse'),
                        (2, 'Support'),
                        (3, 'Marketing');
                        SET IDENTITY_INSERT DEPARTMENT OFF; ", con))
                        {
                            seedCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        #endregion


        #region EmployeeRepository Methods
        public ObservableCollection<string> GetAllNames()
        {
            var list = new ObservableCollection<string>();
            foreach (var employee in Employees)
            {
                list.Add(employee.Name);
            }
            return list;
        }

        public int GetId(string Name)
        {
            const string query = @"
        SELECT Employee_ID
        FROM EMPLOYEE
        WHERE Name = @Name";

            using (var con = new SqlConnection(ConnectionString))
            {
                using (var cmd = new SqlCommand(query, con))
                {

                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 255).Value = Name;

                    con.Open();


                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                        return Convert.ToInt32(result);

                    return 0;
                }
            }
        }
        #endregion

    }
}
