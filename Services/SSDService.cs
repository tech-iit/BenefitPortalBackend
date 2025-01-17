using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using BenefitPortalServices.Models;

namespace BenefitPortalServices.Services
{
    public class SSDService
    {
        private readonly string _connectionString;

        public SSDService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public bool RaiseSSD(SSD ssd)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO SSD (EmployeeId, Title, Description, Status) VALUES (@EmployeeId, @Title, @Description, @Status)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@EmployeeId", ssd.EmployeeId);
                    cmd.Parameters.AddWithValue("@Title", ssd.Title);
                    cmd.Parameters.AddWithValue("@Description", ssd.Description);
                    cmd.Parameters.AddWithValue("@Status", "Requested"); // Default status

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public List<SSD> GetSSDHistoryByEmployeeId(int employeeId)
        {
            var ssdHistory = new List<SSD>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT SSDId, EmployeeId, Title, Description, Status,date FROM SSD WHERE EmployeeId = @EmployeeId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ssdHistory.Add(new SSD
                            {
                                SSDId = reader.GetInt32(0),
                                EmployeeId = reader.GetInt32(1),
                                Title = reader.GetString(2),
                                Description = reader.GetString(3),
                                Status = reader.GetString(4),
                                Date=reader.GetDateTime(5),
                            });
                        }
                    }
                }
            }
            catch
            {
                // Handle exception
            }

            return ssdHistory;
        }


        public List<SSD> GetAllSsdRequests()
        {
            var ssdRequests = new List<SSD>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT SSDId, EmployeeId, Title, Description, Status,date FROM SSD";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ssdRequests.Add(new SSD
                            {
                                SSDId = reader.GetInt32(0),
                                EmployeeId = reader.GetInt32(1),
                                Title = reader.GetString(2),
                                Description = reader.GetString(3),
                                Status = reader.GetString(4),
                                Date=reader.GetDateTime(5),
                            });
                        }
                    }
                }
            }
            catch
            {
                // Handle exception
            }

            return ssdRequests;
        }

        // Method to Update SSD Status
        public bool UpdateSsdStatus(int ssdId, string status)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE SSD SET Status = @Status WHERE SSDId = @SSDId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SSDId", ssdId);
                    cmd.Parameters.AddWithValue("@Status", status);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}