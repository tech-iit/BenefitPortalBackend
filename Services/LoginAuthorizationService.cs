using System;
using System.Configuration;
using System.Data.SqlClient;

namespace BenefitPortalServices.Services
{
    public class LoginAuthorizationService
    {
        private readonly string _connectionString;

        public LoginAuthorizationService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public (string,string,string) Authorize(int username, string password)  // Username as int
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(employeeId), COUNT(adminid), employee.name,employee.emailId FROM    Employee LEFT JOIN     Admin ON employeeid = adminid WHERE   employeeId = @username AND password COLLATE SQL_Latin1_General_CP1_CS_AS = @password GROUP BY employee.name, employee.emailId;";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);  // Username as int
                    cmd.Parameters.AddWithValue("@password", password);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int employeeCount = reader.GetInt32(0);
                            int adminCount = reader.GetInt32(1);
                            string name = reader["name"].ToString();
                            string emailId = reader["emailId"].ToString();
                            if (employeeCount > 0 && adminCount == 0)
                            {
                                return ("Employee",name,emailId);
                            }
                            else if (employeeCount > 0 && adminCount > 0)
                            {
                                return ("Admin",name,emailId);
                            }
                        }
                    }
                }
            }
            catch
            {
                return ("Error",null,null);
            }

            return ("Invalid",null,null);
        }
    }
}
