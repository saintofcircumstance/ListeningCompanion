using System;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;


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
            string query = @"INSERT INTO ApplicationUser (UserName, UserPassword,EmailAddress) 
                             VALUES (@UserName, @UserPassword, @EmailAddress)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@UserPassword", HashString(user.UserPassword));
                    command.Parameters.AddWithValue("@EmailAddress", user.EmailAddress);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void CreateSavedDevice(int userId, string deviceId)
        {
            string query = @"INSERT INTO UserDevice (UserID, DeviceID) 
                             VALUES (@UserID, @DeviceID)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@DeviceID", HashString(deviceId));
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public ApplicationUser IsSavedDevice(string deviceId) {
            ApplicationUser user = null;
            string query = "SELECT * FROM UserDevice WHERE DeviceID = @DeviceID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DeviceID", HashString(deviceId));
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userID = (int)reader["UserID"];
                            user = ReadApplicationUser(userID);
                        }
                    }
                }
            }
            return user;
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
                                UserPassword = reader["UserPassword"].ToString(),
                                EmailAddress = reader["EmailAddress"].ToString()
                            };
                        }
                    }
                }
            }

            return user;
        }

        public bool EmailExists(string emailAddress)
        {
            if(emailAddress == null) { return false; }
            string query = "SELECT * FROM ApplicationUser WHERE LOWER(EmailAddress) = LOWER(@EmailAddress)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmailAddress", emailAddress);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public ApplicationUser AuthenticateUser(string username, string password)
        {
            ApplicationUser user = new ApplicationUser() { UserName = "", ID = -1 };
            if (username.IsNullOrEmpty() || password.IsNullOrEmpty()) { return user; }
            string query = "SELECT TOP 1 * FROM ApplicationUser WHERE LOWER(Username) = LOWER(@Username) and UserPassword = @Password";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", HashString(password));
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read() && reader.HasRows)
                        {
                            int loggedInUserID = (int)reader["ID"];
                            string loggedInUsername= reader["UserName"].ToString();
                            user.UserName = loggedInUsername;
                            user.ID = loggedInUserID;
                            return user;
                        }
                    }
                }
            }
            return user;
        }

        public bool UsernameExists(string username)
        {
            if(username == null) { return false; }
            string query = "SELECT * FROM ApplicationUser WHERE LOWER(Username) = LOWER(@Username)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void UpdateApplicationUser(ApplicationUser user)
        {
            string query = @"UPDATE ApplicationUser 
                             SET UserName = @UserName, 
                                 UserPassword = @UserPassword,
                                 EmailAddress = @EmailAddress
                             WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@UserPassword", HashString(user.UserPassword));
                    command.Parameters.AddWithValue("@EmailAddress", user.EmailAddress);
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

        public void RemoveSavedDevice(int userId, string deviceId)
        {
            string query = "DELETE FROM UserDevice WHERE UserID = @UserID and DeviceID = @DeviceID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@DeviceID", HashString(deviceId));
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static string HashString(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // Convert the byte array to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2")); // "x2" means hexadecimal format with 2 digits
                }
                return builder.ToString();
            }
        }
    }
}
