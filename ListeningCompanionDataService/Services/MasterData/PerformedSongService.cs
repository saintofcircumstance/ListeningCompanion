using System;
using Microsoft.Data.SqlClient;
using ListeningCompanionDataService.Models.MasterData;
namespace ListeningCompanionDataService.Services.MasterData
{
    public class PerformedSongService
    {
        private readonly string _connectionString;

        public PerformedSongService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public PerformedSong ReadPerformedSong(int performedSongId)
        {
            PerformedSong performedSong = null;

            string query = "SELECT * FROM PerformedSong WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", performedSongId);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            performedSong = new PerformedSong
                            {
                                ID = (int)reader["ID"],
                                SetListID = (int)reader["SetListID"],
                                SongID = (int)reader["SongID"],
                                SongSequence = (int)reader["SongSequence"],
                                Segue = (bool)reader["Segue"],
                                DaysSincePlayed = (int)reader["DaysSincePlayed"],
                                LengthMMSS = reader["LengthMMSS"].ToString(),
                                LengthSeconds = (int)reader["LengthSeconds"],
                                Mp3Url = reader["Mp3Url"].ToString()
                            };
                        }
                    }
                }
            }

            return performedSong;
        }

        public void CreatePerformedSong(PerformedSong performedSong)
        {
            string query = @"INSERT INTO PerformedSong 
                            (SetListID, SongID, SongSequence, Segue, DaysSincePlayed, LengthMMSS, LengthSeconds, Mp3Url) 
                         VALUES 
                            (@SetListID, @SongID, @SongSequence, @Segue, @DaysSincePlayed, @LengthMMSS, @LengthSeconds, @Mp3Url)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SetListID", performedSong.SetListID);
                    command.Parameters.AddWithValue("@SongID", performedSong.SongID);
                    command.Parameters.AddWithValue("@SongSequence", performedSong.SongSequence);
                    command.Parameters.AddWithValue("@Segue", performedSong.Segue);
                    command.Parameters.AddWithValue("@DaysSincePlayed", performedSong.DaysSincePlayed);
                    command.Parameters.AddWithValue("@LengthMMSS", performedSong.LengthMMSS);
                    command.Parameters.AddWithValue("@LengthSeconds", performedSong.LengthSeconds);
                    command.Parameters.AddWithValue("@Mp3Url", performedSong.Mp3Url);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdatePerformedSong(PerformedSong performedSong)
        {
            string query = @"UPDATE PerformedSong 
                            SET SetListID = @SetListID, 
                                SongID = @SongID, 
                                SongSequence = @SongSequence, 
                                Segue = @Segue, 
                                DaysSincePlayed = @DaysSincePlayed, 
                                LengthMMSS = @LengthMMSS, 
                                LengthSeconds = @LengthSeconds ,
                                Mp3Url = @Mp3Url
                          WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SetListID", performedSong.SetListID);
                    command.Parameters.AddWithValue("@SongID", performedSong.SongID);
                    command.Parameters.AddWithValue("@SongSequence", performedSong.SongSequence);
                    command.Parameters.AddWithValue("@Segue", performedSong.Segue);
                    command.Parameters.AddWithValue("@DaysSincePlayed", performedSong.DaysSincePlayed);
                    command.Parameters.AddWithValue("@LengthMMSS", performedSong.LengthMMSS);
                    command.Parameters.AddWithValue("@LengthSeconds", performedSong.LengthSeconds);
                    command.Parameters.AddWithValue("@Mp3Url", performedSong.Mp3Url);
                    command.Parameters.AddWithValue("@ID", performedSong.ID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeletePerformedSong(int performedSongId)
        {
            string query = "DELETE FROM PerformedSong WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", performedSongId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }

}