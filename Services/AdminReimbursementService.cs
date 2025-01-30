using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using BenefitPortalServices.Models;

namespace BenefitPortalServices.Services
{
    public class AdminReimbursementService
    {
        private readonly string _connectionString;

        public AdminReimbursementService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        // Get all reimbursements
        public List<AdminReimbursement> GetAllReimbursements()
        {
            var reimbursements = new List<AdminReimbursement>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT ReimbursementId, EmployeeId, Amount, Notes, Status, ReimbursementType,CreatedDate,billresponse FROM Reimbursements";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reimbursements.Add(new AdminReimbursement
                            {
                                ReimbursementId = reader.GetInt32(0),
                                EmployeeId = reader.GetInt32(1),
                                Amount = reader.GetDecimal(2),
                                Notes = reader.GetString(3),
                                Status = reader.GetString(4),
                                ReimbursementType = reader.GetString(5),
                                Date = reader.GetDateTime(6),
                                BillResponse = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine(ex.Message);
            }
            return reimbursements;
        }

        // Update reimbursement status
        public bool UpdateReimbursementStatus(int reimbursementId, string status,string responseByadmin)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Reimbursements SET Status = @Status,billresponse=@responseByadmin WHERE ReimbursementId = @ReimbursementId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@ReimbursementId", reimbursementId);
                    cmd.Parameters.AddWithValue("@responseByadmin", responseByadmin);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
