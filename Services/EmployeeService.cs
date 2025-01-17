using System;
using System.Configuration;
using System.Data.SqlClient;

namespace BenefitPortalServices.Services
{
    public class EmployeeService
    {
        private readonly string _connectionString;

        public EmployeeService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public bool AddEmployee(int employeeId, string password,string name, string emailId,string contactNo, string band)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    int employeeBand = GetBandPriority(band);
                    conn.Open();
                    string query = @"INSERT INTO employee (employeeId, password, name, emailId, contactNo, band) 
    VALUES (@employeeId, @password, @name, @emailId, @contactNo, @band);";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@emailId", emailId);
                    cmd.Parameters.AddWithValue("@contactNo", contactNo);
                    cmd.Parameters.AddWithValue("@band", employeeBand);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@employeeId", employeeId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding employee: " + ex.Message);
                return false;
            }
        }

        private int GetBandPriority(string band)
        {
            switch (band.ToLower())
            {
                case "e0": return 0;
                case "e1": return 1;
                case "e2": return 2;
                case "e3": return 3;
                case "e4": return 4;
                case "e5": return 5;
                default:
                    throw new ArgumentException("Invalid band value");
            }
        }
    }

}
