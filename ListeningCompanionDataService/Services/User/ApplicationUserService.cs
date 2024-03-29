using System;
using Microsoft.Data.SqlClient;

namespace ListeningCompanionDataService.Models.User
{
    public class ApplicationUserService
    {
        private readonly string _connectionString;

        public ApplicationUserService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateApplicationUser(ApplicationUser user)
        {
            string query = @"INSERT INTO ApplicationUser (UserName, UserPassword) 
                             VALUES (@UserName, @UserPassword)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@UserPassword", user.UserPassword);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public ApplicationUser ReadApplicationUser(int userId)
        {
            ApplicationUser user = null;

            string query = "SELECT * FROM ApplicationUser WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", userId);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new ApplicationUser
                            {
                                ID = (int)reader["ID"],
                                UserName = reader["UserName"].ToString(),
                                UserPassword = reader["UserPassword"].ToString()
                            };
                        }
                    }
                }
            }

            return user;
        }

        public void UpdateApplicationUser(ApplicationUser user)
        {
            string query = @"UPDATE ApplicationUser 
                             SET UserName = @UserName, 
                                 UserPassword = @UserPassword 
                             WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@UserPassword", user.UserPassword);
                    command.Parameters.AddWithValue("@ID", user.ID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteApplicationUser(int userId)
        {
            string query = "DELETE FROM ApplicationUser WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", userId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
