using System;
using System.Configuration;
using System.Data.SqlClient;

namespace BenefitPortalServices.Services
{
    public class BookmarkService
    {
        private readonly string _connectionString;

        public BookmarkService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public bool Bookmark(int employeeId, int benefitId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Check if the bookmark already exists
                    string checkQuery = "SELECT COUNT(*) FROM BookmarkedBenefit WHERE employeeId = @EmployeeId AND benefitId = @BenefitId";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        checkCmd.Parameters.AddWithValue("@BenefitId", benefitId);

                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // Remove bookmark if it exists
                            string deleteQuery = "DELETE FROM BookmarkedBenefit WHERE employeeId = @EmployeeId AND benefitId = @BenefitId";
                            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                            {
                                deleteCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                                deleteCmd.Parameters.AddWithValue("@BenefitId", benefitId);
                                deleteCmd.ExecuteNonQuery();
                            }
                            return false; // Bookmark removed
                        }
                        else
                        {
                            // Add bookmark if it does not exist
                            string insertQuery = "INSERT INTO BookmarkedBenefit (employeeId, benefitId) VALUES (@EmployeeId, @BenefitId)";
                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                                insertCmd.Parameters.AddWithValue("@BenefitId", benefitId);
                                insertCmd.ExecuteNonQuery();
                            }
                            return true; // Bookmark added
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (logging not implemented here)
                throw new Exception("An error occurred while updating bookmarks.", ex);
            }
        }
    }
}
