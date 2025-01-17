using System;
using System.Configuration;
using System.Data.SqlClient;
using BenefitPortalServices.Models;

namespace BenefitPortalServices.Services
{
    public class FeedbackService
    {
        private readonly string _connectionString;

        public FeedbackService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public bool CheckBenefitExists(int benefitId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Benefit WHERE benefitId = @BenefitId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BenefitId", benefitId);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        public bool CheckEmployeeExists(int employeeId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM employee WHERE employeeId = @EmployeeId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        public (bool, string) InsertFeedback(Feedback feedback)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string bandQuery = "SELECT band FROM employee WHERE employeeId = @EmployeeId";
                    SqlCommand bandCmd = new SqlCommand(bandQuery, conn);
                    bandCmd.Parameters.AddWithValue("@EmployeeId", feedback.EmployeeId);
                    int employeeBand = Convert.ToInt32(bandCmd.ExecuteScalar());

                    string eligibilityQuery = "SELECT minEligibilityCriteria FROM Benefit WHERE benefitId = @BenefitId";
                    SqlCommand eligibilityCmd = new SqlCommand(eligibilityQuery, conn);
                    eligibilityCmd.Parameters.AddWithValue("@BenefitId", feedback.BenefitId);
                    int minEligibility = Convert.ToInt32(eligibilityCmd.ExecuteScalar());

                   
                    if (employeeBand >= minEligibility)
                    {
                       
                        string insertQuery = "INSERT INTO feedback (benefitId, employeeId, feedback, suggestion) " +
                                             "VALUES (@BenefitId, @EmployeeId, @Feedback, @Suggestion)";
                        SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                        insertCmd.Parameters.AddWithValue("@BenefitId", feedback.BenefitId);
                        insertCmd.Parameters.AddWithValue("@EmployeeId", feedback.EmployeeId);
                        insertCmd.Parameters.AddWithValue("@Feedback", feedback.FeedbackText);
                        insertCmd.Parameters.AddWithValue("@Suggestion", feedback.Suggestion);

                        return (insertCmd.ExecuteNonQuery() > 0, "1");  // Returns true if the insert was successful
                    }
                    else
                    {
                        return (false, "2");  
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}, StackTrace: {ex.StackTrace}");
            }
        }
    }
}
