using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using BenefitPortalServices.Models;

namespace BenefitPortalServices.Services
{
    public class AvailedBenefitService
    {
        private readonly string _connectionString;

        public AvailedBenefitService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public List<Availedbenefit> AvailedBenefit(int employeeId, int benefitId)
        {
            List<Availedbenefit> availedBenefits = new List<Availedbenefit>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Get the MinEligibilityCriteria of the benefit
                    string eligibilityQuery = @"
                        SELECT b.MinEligibilityCriteria
                        FROM Benefit b
                        WHERE b.benefitId = @benefitId;
                    ";

                    int minEligibilityCriteria = 0;

                    using (SqlCommand eligibilityCommand = new SqlCommand(eligibilityQuery, connection))
                    {
                        eligibilityCommand.Parameters.AddWithValue("@benefitId", benefitId);

                        using (SqlDataReader reader = eligibilityCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                minEligibilityCriteria = Convert.ToInt32(reader["MinEligibilityCriteria"]);
                            }
                        }
                    }

                    // Get the employee's band from the Employees table
                    string bandQuery = @"
                        SELECT e.band
                        FROM Employee e
                        WHERE e.employeeId = @employeeId;
                    ";

                    int employeeBand = 0;

                    using (SqlCommand bandCommand = new SqlCommand(bandQuery, connection))
                    {
                        bandCommand.Parameters.AddWithValue("@employeeId", employeeId);

                        using (SqlDataReader reader = bandCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                employeeBand = Convert.ToInt32(reader["band"]);
                            }
                        }
                    }

                    // Check if the employee's band is greater than or equal to the min eligibility criteria
                    if (employeeBand >= minEligibilityCriteria)
                    {
                        // Check if the record already exists in AvailedBenefits
                        string checkQuery = @"
                            SELECT COUNT(1)
                            FROM AvailedBenefits 
                            WHERE benefitId = @benefitId AND employeeId = @employeeId;
                        ";

                        using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@benefitId", benefitId);
                            checkCommand.Parameters.AddWithValue("@employeeId", employeeId);

                            int recordCount = (int)checkCommand.ExecuteScalar();

                            // If the record already exists, just retrieve the availed benefits for the employee
                            if (recordCount > 0)
                            {
                                // Retrieve the availed benefits for the employee
                                string selectQuery = @"
                                    SELECT ab.availedId, ab.benefitId, b.Name, b.Description, b.ImagePath, b.MinEligibilityCriteria, b.Category, b.AdminId
                                    FROM AvailedBenefits ab
                                    JOIN Benefit b ON ab.benefitId = b.benefitId
                                    WHERE ab.employeeId = @employeeId;
                                ";

                                using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                                {
                                    selectCommand.Parameters.AddWithValue("@employeeId", employeeId);

                                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            Availedbenefit availed = new Availedbenefit
                                            {

                                                Id = Convert.ToInt32(reader["benefitId"]),
                                                Name = reader["name"].ToString(),
                                                Description = reader["description"].ToString(),
                                                ImagePath = reader["imagePath"].ToString(),
                                                MinEligibilityCriteria = Convert.ToInt32(reader["minEligibilityCriteria"]),
                                                Category = reader["category"].ToString(),
                                                AdminId = Convert.ToInt32(reader["adminId"]),
                                            };
                                            availedBenefits.Add(availed);
                                        }
                                    }
                                }
                                return availedBenefits;  // Return existing availed benefits if record exists
                            }
                        }

                        // Insert new record into AvailedBenefits table if eligibility criteria is met
                        string insertQuery = @"
                            INSERT INTO AvailedBenefits (benefitId, employeeId)
                            VALUES (@benefitId, @employeeId);
                        ";

                        using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@benefitId", benefitId);
                            insertCommand.Parameters.AddWithValue("@employeeId", employeeId);

                            insertCommand.ExecuteNonQuery();
                        }

                        // Retrieve the availed benefits for the employee after insertion
                        string selectQueryAfterInsert = @"
                            SELECT ab.availedId, ab.benefitId, b.name, b.description, b.imagePath, b.minEligibilityCriteria, b.category, b.adminId
                            FROM AvailedBenefits ab
                            JOIN Benefit b ON ab.benefitId = b.benefitId
                            WHERE ab.employeeId = @employeeId;
                        ";

                        using (SqlCommand selectCommandAfterInsert = new SqlCommand(selectQueryAfterInsert, connection))
                        {
                            selectCommandAfterInsert.Parameters.AddWithValue("@employeeId", employeeId);

                            using (SqlDataReader reader = selectCommandAfterInsert.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Availedbenefit availed = new Availedbenefit
                                    {

                                        Id = Convert.ToInt32(reader["benefitId"]),
                                        Name = reader["name"].ToString(),
                                        Description = reader["description"].ToString(),
                                        ImagePath = reader["imagePath"].ToString(),
                                        MinEligibilityCriteria = Convert.ToInt32(reader["minEligibilityCriteria"]),
                                        Category = reader["category"].ToString(),
                                        AdminId = Convert.ToInt32(reader["adminId"]),
                                    };
                                    availedBenefits.Add(availed);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Employee's band is below the minimum eligibility criteria for this benefit.");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Log the SQL exception or handle accordingly
                throw new Exception("An error occurred while accessing the database.", sqlEx);
            }
            catch (Exception ex)
            {
                // Log the general exception or handle accordingly
                throw new Exception("An unexpected error occurred.", ex);
            }

            return availedBenefits;
        }




        public List<Availedbenefit> Avail(int employeeId)
        {
            List<Availedbenefit> availedBenefits = new List<Availedbenefit>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Retrieve the availed benefits for the employee
                    string selectQuery = @"
                                    SELECT ab.availedId, ab.benefitId, b.Name, b.Description, b.ImagePath, b.MinEligibilityCriteria, b.Category, b.AdminId
                                    FROM AvailedBenefits ab
                                    JOIN Benefit b ON ab.benefitId = b.benefitId
                                    WHERE ab.employeeId = @employeeId;
                                ";

                    using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@employeeId", employeeId);

                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Availedbenefit availed = new Availedbenefit
                                {

                                    Id = Convert.ToInt32(reader["benefitId"]),
                                    Name = reader["name"].ToString(),
                                    Description = reader["description"].ToString(),
                                    ImagePath = reader["imagePath"].ToString(),
                                    MinEligibilityCriteria = Convert.ToInt32(reader["minEligibilityCriteria"]),
                                    Category = reader["category"].ToString(),
                                    AdminId = Convert.ToInt32(reader["adminId"]),
                                };
                                availedBenefits.Add(availed);
                            }
                        }
                    }
                    return availedBenefits;  // Return existing availed benefits if record exists
                }
            }

            catch (SqlException sqlEx)
            {
                // Log the SQL exception or handle accordingly
                throw new Exception("An error occurred while accessing the database.", sqlEx);
            }
            catch (Exception ex)
            {
                // Log the general exception or handle accordingly
                throw new Exception("An unexpected error occurred.", ex);
            }

            
        }
    }
}

