using System;
using Microsoft.Data.SqlClient;

namespace ListeningCompanionDataService.Models.MasterData
{
    public class SongService
    {
        private readonly string _connectionString;

        public SongService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateSong(Song song)
        {
            string query = @"INSERT INTO Song (SongUniqueID, BandID, Title, FirstPlayed, FirstPlayedShowUniqueID, LastPlayed, LastPlayedShowUniqueID, CountPlayed) 
                             VALUES (@SongUniqueID, @BandID, @Title, @FirstPlayed, @FirstPlayedShowUniqueID, @LastPlayed, @LastPlayedShowUniqueID, @CountPlayed)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SongUniqueID", song.SongUniqueID);
                    command.Parameters.AddWithValue("@BandID", song.BandID);
                    command.Parameters.AddWithValue("@Title", song.Title);
                    command.Parameters.AddWithValue("@FirstPlayed", song.FirstPlayed);
                    command.Parameters.AddWithValue("@FirstPlayedShowUniqueID", song.FirstPlayedShowUniqueID);
                    command.Parameters.AddWithValue("@LastPlayed", song.LastPlayed);
                    command.Parameters.AddWithValue("@LastPlayedShowUniqueID", song.LastPlayedShowUniqueID);
                    command.Parameters.AddWithValue("@CountPlayed", song.CountPlayed);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public Song ReadSong(int songId)
        {
            Song song = null;

            string query = "SELECT * FROM Song WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", songId);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            song = new Song
                            {
                                ID = (int)reader["ID"],
                                SongUniqueID = (Guid)reader["SongUniqueID"],
                                BandID = (int)reader["BandID"],
                                Title = reader["Title"].ToString(),
                                FirstPlayed = reader["FirstPlayed"].ToString(),
                                FirstPlayedShowUniqueID = (Guid)reader["FirstPlayedShowUniqueID"],
                                LastPlayed = reader["LastPlayed"].ToString(),
                                LastPlayedShowUniqueID = (Guid)reader["LastPlayedShowUniqueID"],
                                CountPlayed = (int)reader["CountPlayed"]
                            };
                        }
                    }
                }
            }

            return song;
        }

        public void UpdateSong(Song song)
        {
            string query = @"UPDATE Song 
                             SET SongUniqueID = @SongUniqueID, 
                                 BandID = @BandID, 
                                 Title = @Title, 
                                 FirstPlayed = @FirstPlayed, 
                                 FirstPlayedShowUniqueID = @FirstPlayedShowUniqueID, 
                                 LastPlayed = @LastPlayed, 
                                 LastPlayedShowUniqueID = @LastPlayedShowUniqueID, 
                                 CountPlayed = @CountPlayed 
                             WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SongUniqueID", song.SongUniqueID);
                    command.Parameters.AddWithValue("@BandID", song.BandID);
                    command.Parameters.AddWithValue("@Title", song.Title);
                    command.Parameters.AddWithValue("@FirstPlayed", song.FirstPlayed);
                    command.Parameters.AddWithValue("@FirstPlayedShowUniqueID", song.FirstPlayedShowUniqueID);
                    command.Parameters.AddWithValue("@LastPlayed", song.LastPlayed);
                    command.Parameters.AddWithValue("@LastPlayedShowUniqueID", song.LastPlayedShowUniqueID);
                    command.Parameters.AddWithValue("@CountPlayed", song.CountPlayed);
                    command.Parameters.AddWithValue("@ID", song.ID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteSong(int songId)
        {
            string query = "DELETE FROM Song WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", songId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
