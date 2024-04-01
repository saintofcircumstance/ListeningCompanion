using System;
using Microsoft.Data.SqlClient;

namespace ListeningCompanionDataService.Models.User
{
    public class UserShowService
    {
        private readonly string _connectionString;

        public UserShowService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveUserShow(UserShow userShow)
        {
            string userShowExistsSql = $"select top 1 ID from UserShow where UserID = {userShow.UserID} and ShowID = {userShow.ShowID}";
            SqlConnection existsSqlConnection = new SqlConnection(_connectionString);
            SqlCommand existsSqlCommand = new SqlCommand(userShowExistsSql, existsSqlConnection);
            existsSqlConnection.Open();
            SqlDataReader existsReader = existsSqlCommand.ExecuteReader();
            if (existsReader.HasRows)
            {
                UpdateUserShow(userShow);
            }
            else
            {
                CreateUserShow(userShow);
            }
        }

        public void CreateUserShow(UserShow userShow)
        {
            

            string query = @"INSERT INTO UserShow (UserID, ShowID, Rating, Notes, InteractionStatus, Liked, BookMarked) 
                             VALUES (@UserID, @ShowID, @Rating, @Notes, @InteractionStatus, @Liked, @BookMarked)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userShow.UserID);
                    command.Parameters.AddWithValue("@ShowID", userShow.ShowID);
                    command.Parameters.AddWithValue("@Rating", userShow.Rating);
                    command.Parameters.AddWithValue("@Notes", userShow.Notes);
                    command.Parameters.AddWithValue("@InteractionStatus", userShow.InteractionStatus);
                    command.Parameters.AddWithValue("@Liked", userShow.Liked);
                    command.Parameters.AddWithValue("@BookMarked", userShow.BookMarked);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public UserShow ReadUserShow(int userShowId)
        {
            UserShow userShow = null;

            string query = "SELECT * FROM UserShow WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", userShowId);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userShow = new UserShow
                            {
                                ID = (int)reader["ID"],
                                UserID = (int)reader["UserID"],
                                ShowID = (int)reader["ShowID"],
                                Rating = (int)reader["Rating"],
                                Notes = reader["Notes"].ToString(),
                                InteractionStatus = reader["InteractionStatus"].ToString(),
                                Liked = (bool)reader["Liked"],
                                BookMarked = (bool)reader["BookMarked"]
                            };
                        }
                    }
                }
            }

            return userShow;
        }

        public void UpdateUserShow(UserShow userShow)
        {
            string query = @"UPDATE UserShow 
                             SET UserID = @UserID, 
                                 ShowID = @ShowID, 
                                 Rating = @Rating, 
                                 Notes = @Notes, 
                                 InteractionStatus = @InteractionStatus, 
                                 Liked = @Liked, 
                                 BookMarked = @BookMarked 
                             WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userShow.UserID);
                    command.Parameters.AddWithValue("@ShowID", userShow.ShowID);
                    command.Parameters.AddWithValue("@Rating", userShow.Rating);
                    command.Parameters.AddWithValue("@Notes", userShow.Notes);
                    command.Parameters.AddWithValue("@InteractionStatus", userShow.InteractionStatus);
                    command.Parameters.AddWithValue("@Liked", userShow.Liked);
                    command.Parameters.AddWithValue("@BookMarked", userShow.BookMarked);
                    command.Parameters.AddWithValue("@ID", userShow.ID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUserShow(int userShowId)
        {
            string query = "DELETE FROM UserShow WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", userShowId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
