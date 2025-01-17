using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using BenefitPortalServices.Models;

namespace BenefitPortalServices.Services
{
    public class ReimbursementService
    {
        private readonly string _connectionString;

        public ReimbursementService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public bool AddReimbursement(Reimbursement reimbursement)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Reimbursements (EmployeeId, Amount, Notes, Status, ReimbursementType) VALUES (@EmployeeId, @Amount, @Notes, @Status, @ReimbursementType)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@EmployeeId", reimbursement.EmployeeId);
                    cmd.Parameters.AddWithValue("@Amount", reimbursement.Amount);
                    cmd.Parameters.AddWithValue("@Notes", reimbursement.Notes);
                    cmd.Parameters.AddWithValue("@Status", reimbursement.Status);
                    cmd.Parameters.AddWithValue("@ReimbursementType", reimbursement.ReimbursementType);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public List<Reimbursement> GetReimbursementsByEmployeeId(int employeeId)
        {
            var reimbursements = new List<Reimbursement>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT ReimbursementId, EmployeeId, Amount, Notes, Status, ReimbursementType FROM Reimbursements WHERE EmployeeId = @EmployeeId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reimbursements.Add(new Reimbursement
                            {
                                EmployeeId = reader.GetInt32(1),
                                Amount = reader.GetDecimal(2),
                                Notes = reader.GetString(3),
                                Status = reader.GetString(4),
                                ReimbursementType = reader.GetString(5)
                            });
                        }
                    }
                }
            }
            catch
            {
                // Handle exception
            }

            return reimbursements;
        }
    }
}
