﻿using System;
using Microsoft.Data.SqlClient;

namespace ListeningCompanionDataService.Models.User
{
    public class UserPerformedSongService
    {
        private readonly string _connectionString;

        public UserPerformedSongService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveUserPerformedSong(UserPerformedSong song)
        {
            if(song.UserID < 1) { return; }
            string userPerformedSongExistsSql = $"select top 1 ID from UserPerformedSong where UserID = {song.UserID} and PerformedSongID = {song.PerformedSongID}";
            SqlConnection existsSqlConnection = new SqlConnection(_connectionString);
            SqlCommand existsSqlCommand = new SqlCommand(userPerformedSongExistsSql, existsSqlConnection);
            existsSqlConnection.Open();
            SqlDataReader existsReader = existsSqlCommand.ExecuteReader();
            if (existsReader.HasRows)
            {
                UpdateUserPerformedSong(song);
            }
            else
            {
                CreateUserPerformedSong(song);
            }

        }

        public void CreateUserPerformedSong(UserPerformedSong userPerformedSong)
        {
            string query = @"INSERT INTO UserPerformedSong (UserID, PerformedSongID, Rating, Notes, Liked, BookMarked) 
                             VALUES (@UserID, @PerformedSongID, @Rating, @Notes, @Liked, @BookMarked)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userPerformedSong.UserID);
                    command.Parameters.AddWithValue("@PerformedSongID", userPerformedSong.PerformedSongID);
                    command.Parameters.AddWithValue("@Rating", userPerformedSong.Rating);
                    command.Parameters.AddWithValue("@Notes", userPerformedSong.Notes);
                    command.Parameters.AddWithValue("@Liked", userPerformedSong.Liked);
                    command.Parameters.AddWithValue("@BookMarked", userPerformedSong.Bookmarked);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public UserPerformedSong ReadUserPerformedSong(int userPerformedSongId)
        {
            UserPerformedSong userPerformedSong = null;

            string query = "SELECT * FROM UserPerformedSong WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", userPerformedSongId);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userPerformedSong = new UserPerformedSong
                            {
                                ID = (int)reader["ID"],
                                UserID = (int)reader["UserID"],
                                PerformedSongID = (int)reader["PerformedSongID"],
                                Rating = (int)reader["Rating"],
                                Notes = reader["Notes"].ToString(),
                                Liked = (bool)reader["Liked"],
                                Bookmarked = (bool)reader["BookMarked"]
                            };
                        }
                    }
                }
            }

            return userPerformedSong;
        }

        public void UpdateUserPerformedSong(UserPerformedSong userPerformedSong)
        {
            string query = @"UPDATE UserPerformedSong 
                             SET UserID = @UserID, 
                                 PerformedSongID = @PerformedSongID, 
                                 Rating = @Rating, 
                                 Notes = @Notes, 
                                 Liked = @Liked, 
                                 BookMarked = @BookMarked 
                             WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userPerformedSong.UserID);
                    command.Parameters.AddWithValue("@PerformedSongID", userPerformedSong.PerformedSongID);
                    command.Parameters.AddWithValue("@Rating", userPerformedSong.Rating);
                    command.Parameters.AddWithValue("@Notes", userPerformedSong.Notes);
                    command.Parameters.AddWithValue("@Liked", userPerformedSong.Liked);
                    command.Parameters.AddWithValue("@BookMarked", userPerformedSong.Bookmarked);
                    command.Parameters.AddWithValue("@ID", userPerformedSong.ID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUserPerformedSong(int userPerformedSongId)
        {
            string query = "DELETE FROM UserPerformedSong WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", userPerformedSongId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
