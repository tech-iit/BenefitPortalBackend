using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BenefitPortalServices.Models;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using System.Configuration;

namespace BenefitPortalServices.Services
{
    public class UploadBenefit
    {
        private readonly string _connectionString;

        public UploadBenefit()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public bool AddBenefit(int adminId, string name, string description, string imagePath, string category, string minEligibilityCriteria)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    int employeeBand = GetBandPriority(minEligibilityCriteria);
                    conn.Open();
                    string query = @"INSERT INTO Benefit (name, description, imagePath, minEligibilityCriteria, category, adminId) 
    VALUES (@name, @description, @imagePath, @minEligibilityCriteria, @category, @adminId);";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    // Add parameters to the query
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@imagePath", imagePath);
                    cmd.Parameters.AddWithValue("@minEligibilityCriteria", employeeBand);
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.Parameters.AddWithValue("@adminId", adminId);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if(rowsAffected > 0){
                        string eligibleEmployeesQuery = @"
                    SELECT emailId 
                    FROM employee 
                    WHERE band >= @minEligibilityCriteria and subscribe=1";
                        SqlCommand eligibleCmd = new SqlCommand(eligibleEmployeesQuery, conn);
                        eligibleCmd.Parameters.AddWithValue("@minEligibilityCriteria", employeeBand);

                        SqlDataReader reader = eligibleCmd.ExecuteReader();

                        List<string> eligibleEmails = new List<string>();
                        while (reader.Read())
                        {
                            string email = reader["emailId"].ToString();
                            if (!string.IsNullOrEmpty(email))
                            {
                                eligibleEmails.Add(email);
                            }
                        }
                        reader.Close();

                        // Send emails to eligible employees
                        foreach (string email in eligibleEmails)
                        {
                            SendBenefitEmail(email, name, description);
                        }

                        return true;
                    }
                    return false;
                } // Return true if a row was inserted
            }
            catch (Exception ex)
            {
                // Log the error (you can replace this with proper logging)
                Console.WriteLine("Error adding benefit: " + ex.Message);
                return false; // Return false if an error occurred
            }

        }
        public List<Benefit> ViewBenefits(int employeeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Get the employee's band from the employee table
                    string employeeBandQuery = "SELECT band FROM employee WHERE employeeId = @employeeId";
                    SqlCommand employeeBandCmd = new SqlCommand(employeeBandQuery, conn);
                    employeeBandCmd.Parameters.AddWithValue("@employeeId", employeeId);
                    var employeeBand = (int?)employeeBandCmd.ExecuteScalar();

                    string benefitsQuery = @"
            SELECT b.benefitId, 
                   b.name, 
                   b.description, 
                   b.imagePath, 
                   b.minEligibilityCriteria, 
                   b.category, 
                   b.adminId,
                   CASE WHEN bb.employeeId IS NOT NULL THEN 1 ELSE 0 END AS isBookmarked
            FROM Benefit b
            LEFT JOIN bookmarkedBenefit bb ON b.benefitId = bb.benefitId AND bb.employeeId = @employeeId
            WHERE b.minEligibilityCriteria <= @employeeBand";

                    SqlCommand benefitsCmd = new SqlCommand(benefitsQuery, conn);
                    benefitsCmd.Parameters.AddWithValue("@employeeId", employeeId);
                    benefitsCmd.Parameters.AddWithValue("@employeeBand", employeeBand.Value);

                    SqlDataReader reader = benefitsCmd.ExecuteReader();

                    List<Benefit> benefitsList = new List<Benefit>();

                    while (reader.Read())
                    {
                        Benefit benefit = new Benefit
                        {
                            Id = Convert.ToInt32(reader["benefitId"]),
                            Name = reader["name"].ToString(),
                            Description = reader["description"].ToString(),
                            ImagePath = reader["imagePath"].ToString(),
                            MinEligibilityCriteria = Convert.ToInt32(reader["minEligibilityCriteria"]),
                            Category = reader["category"].ToString(),
                            AdminId = Convert.ToInt32(reader["adminId"]),
                            IsBookmarked = Convert.ToBoolean(reader["isBookmarked"])  // Set the IsBookmarked flag
                        };
                        benefitsList.Add(benefit);
                    }

                    return benefitsList; // Return the list of benefits including the bookmark status
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine("Error fetching benefits: " + ex.Message);
                return new List<Benefit>(); // Return an empty list in case of error
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



        private void SendBenefitEmail(string recipientEmail, string benefitName, string benefitDescription)
        {
            try
            {
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string smtpUsername = "techiit.techiit@gmail.com"; // Your email
                string smtpPassword = "vfkf oizf iwdy txax"; // Your email app password
                string subject = $"New Benefit Available: {benefitName}";
                string body = $"A new benefit has been added to the portal:\n\nName: {benefitName}\nDescription: {benefitDescription}\n\nCheck it out in the Benefits Portal.";

                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Benefit Portal", smtpUsername));
                emailMessage.To.Add(new MailboxAddress("", recipientEmail));
                emailMessage.Subject = subject;
                var bodyBuilder = new BodyBuilder { TextBody = body };
                emailMessage.Body = bodyBuilder.ToMessageBody();

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Connect(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                    smtpClient.Authenticate(smtpUsername, smtpPassword);
                    smtpClient.Send(emailMessage);
                    smtpClient.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending benefit email: {ex.Message}");
            }
        }
    }
}


