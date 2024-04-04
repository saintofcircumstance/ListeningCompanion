using System;
using Microsoft.Data.SqlClient;

namespace ListeningCompanionDataService.Models.User
{
    public class UserSongService
    {
        private readonly string _connectionString;

        public UserSongService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateUserSong(UserSong userSong)
        {
            string query = @"INSERT INTO UserSong (UserID, SongID, Rating, Notes, Liked, BookMarked) 
                             VALUES (@UserID, @SongID, @Rating, @Notes, @Liked, @BookMarked)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userSong.UserID);
                    command.Parameters.AddWithValue("@SongID", userSong.SongID);
                    command.Parameters.AddWithValue("@Rating", userSong.Rating);
                    command.Parameters.AddWithValue("@Notes", userSong.Notes);
                    command.Parameters.AddWithValue("@Liked", userSong.Liked);
                    command.Parameters.AddWithValue("@BookMarked", userSong.BookMarked);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public UserSong ReadUserSong(int userSongId)
        {
            UserSong userSong = null;

            string query = "SELECT * FROM UserSong WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", userSongId);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userSong = new UserSong
                            {
                                ID = (int)reader["ID"],
                                UserID = (int)reader["UserID"],
                                SongID = (int)reader["SongID"],
                                Rating = (int)reader["Rating"],
                                Notes = reader["Notes"].ToString(),
                                Liked = (bool)reader["Liked"],
                                BookMarked = (bool)reader["BookMarked"]
                            };
                        }
                    }
                }
            }

            return userSong;
        }

        public void UpdateUserSong(UserSong userSong)
        {
            string query = @"UPDATE UserSong 
                             SET UserID = @UserID, 
                                 SongID = @SongID, 
                                 Rating = @Rating, 
                                 Notes = @Notes, 
                                 Liked = @Liked, 
                                 BookMarked = @BookMarked 
                             WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userSong.UserID);
                    command.Parameters.AddWithValue("@SongID", userSong.SongID);
                    command.Parameters.AddWithValue("@Rating", userSong.Rating);
                    command.Parameters.AddWithValue("@Notes", userSong.Notes);
                    command.Parameters.AddWithValue("@Liked", userSong.Liked);
                    command.Parameters.AddWithValue("@BookMarked", userSong.BookMarked);
                    command.Parameters.AddWithValue("@ID", userSong.ID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUserSong(int userSongId)
        {
            string query = "DELETE FROM UserSong WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", userSongId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
