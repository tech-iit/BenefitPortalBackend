using System;
using System.Data.SqlClient;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Configuration;

namespace BenefitPortalServices.Services
{
    public class ResetService
    {
        private readonly string _connectionString;

        public ResetService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }


        public bool SendOtp(int employeeId)
        {
            try
            {
                int otpCode = new Random().Next(100000, 999999);
                DateTime expiry = DateTime.Now.AddMinutes(10);

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Fetch employee email
                    string emailQuery = "SELECT emailId FROM employee WHERE employeeId = @employeeId";
                    SqlCommand emailCmd = new SqlCommand(emailQuery, conn);
                    emailCmd.Parameters.AddWithValue("@employeeId", employeeId);

                    string email = emailCmd.ExecuteScalar()?.ToString();
                    if (string.IsNullOrEmpty(email)) return false;

                    // Insert or update OTP in the otp table
                    string otpQuery = @"
                        IF EXISTS (SELECT 1 FROM otp WHERE employeeId = @employeeId)
                        BEGIN
                            UPDATE otp SET otpCode = @otpCode, expiry = @expiry WHERE employeeId = @employeeId
                        END
                        ELSE
                        BEGIN
                            INSERT INTO otp (employeeId, otpCode, expiry) VALUES (@employeeId, @otpCode, @expiry)
                        END";

                    SqlCommand otpCmd = new SqlCommand(otpQuery, conn);
                    otpCmd.Parameters.AddWithValue("@employeeId", employeeId);
                    otpCmd.Parameters.AddWithValue("@otpCode", otpCode);
                    otpCmd.Parameters.AddWithValue("@expiry", expiry);

                    otpCmd.ExecuteNonQuery();

                    // Send OTP email
                    SendOtpEmail(email, otpCode.ToString());
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool VerifyOtp(int employeeId, int otpCode)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = "SELECT otpCode,expiry FROM otp WHERE employeeId = @employeeId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@employeeId", employeeId);
                    int storedOtp = -1;
                    DateTime expiry=DateTime.Now;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            storedOtp = Convert.ToInt32(reader["otpCode"]);
                            expiry = Convert.ToDateTime(reader["expiry"]);
                        }
                    }

                    if (storedOtp == otpCode && DateTime.Now<=expiry)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

        }


        public bool ResetPassword(int employeeId, string newPassword)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Update password for the employee
                    string query = "UPDATE employee SET password = @newPassword WHERE employeeId = @employeeId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@newPassword", newPassword); // Hash the password in production!
                    cmd.Parameters.AddWithValue("@employeeId", employeeId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private void SendOtpEmail(string recipientEmail, string otp)
        {
            try
            {
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string smtpUsername = "techiit.techiit@gmail.com"; // Your email
                string smtpPassword = "vfkf oizf iwdy txax"; // Your email app password
                string subject = "Your OTP Code";
                string body = $"Your OTP code is: {otp}. It will expire in 10 minutes.";

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
                Console.WriteLine($"Error sending OTP email: {ex.Message}");
            }
        }
    }
}