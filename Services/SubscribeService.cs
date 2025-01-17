using System;
using System.Configuration;
using System.Data.SqlClient;
using BenefitPortalServices.Models;

namespace BenefitPortalServices.Services
{
    public class SubscribeService
    {
        private readonly string _connectionString;

        public SubscribeService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
        public bool Subscribe(string emailId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string insertQuery = @"update employee set subscribe=1 where emailId=@emailId";

                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@emailId", emailId);
                    int recordCount = (int)insertCommand.ExecuteNonQuery();
                    if (recordCount > 0)
                    {
                        connection.Close();
                        return true;
                    }
                }
                return false;
            }
        }
        public bool CheckSubscription(string emailId) {

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                bool SubscribeStatus = false;
                conn.Open();
                string query = "SELECT subscribe FROM employee WHERE emailId = @emailId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@emailId", emailId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        SubscribeStatus = reader.GetBoolean(0);
                        conn.Close();
                    }
                    if (SubscribeStatus)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        public bool UnSubscribe(string emailId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE employee SET subscribe = 0 WHERE emailId = @emailId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@emailId", emailId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }   
  }