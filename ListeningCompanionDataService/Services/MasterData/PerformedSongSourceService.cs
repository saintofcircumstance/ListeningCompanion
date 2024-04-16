using System;
using Microsoft.Data.SqlClient;
using ListeningCompanionDataService.Models.MasterData;
namespace ListeningCompanionDataService.Services.MasterData
{
    public class PerformedSongSourceService
    {
        private readonly string connectionString;

        public PerformedSongSourceService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Create
        public int AddPerformedSongSource(PerformedSongSource source)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO PerformedSongSource (PerformedSongID, ShowID, SourceUniqueID, TrackPosition, Title, Mp3Url, Mp3Mu5) 
                             VALUES (@PerformedSongID, @ShowID, @SourceUniqueID, @TrackPosition, @Title, @Mp3Url, @Mp3Mu5);
                             SELECT SCOPE_IDENTITY();";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PerformedSongID", source.PerformedSongID);
                command.Parameters.AddWithValue("@ShowID", source.ShowID);
                command.Parameters.AddWithValue("@SourceUniqueID", source.SourceUniqueID);
                command.Parameters.AddWithValue("@TrackPosition", source.TrackPosition);
                command.Parameters.AddWithValue("@Title", source.Title);
                command.Parameters.AddWithValue("@Mp3Url", source.Mp3Url);
                command.Parameters.AddWithValue("@Mp3Mu5", source.Mp3Mu5 == null ? "" : source.Mp3Mu5);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        // Read
        public List<PerformedSongSource> GetAllPerformedSongSources()
        {
            List<PerformedSongSource> sources = new List<PerformedSongSource>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM PerformedSongSource";
                SqlCommand command = new SqlCommand(query, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sources.Add(new PerformedSongSource
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            PerformedSongID = Convert.ToInt32(reader["PerformedSongID"]),
                            ShowID = Convert.ToInt32(reader["ShowID"]),
                            SourceUniqueID = Guid.Parse(reader["SourceUniqueID"].ToString()),
                            TrackPosition = Convert.ToInt32(reader["TrackPosition"]),
                            Title = reader["Title"].ToString(),
                            Mp3Url = reader["Mp3Url"].ToString(),
                            Mp3Mu5 = reader["Mp3Mu5"].ToString()
                        });
                    }
                }
            }
            return sources;
        }
        public PerformedSongSource GetPerformedSongSource(int id)
        {
            PerformedSongSource source = new PerformedSongSource();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT * FROM PerformedSongSource where ID = {id}";
                SqlCommand command = new SqlCommand(query, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        source = new PerformedSongSource
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            PerformedSongID = Convert.ToInt32(reader["PerformedSongID"]),
                            ShowID = Convert.ToInt32(reader["ShowID"]),
                            SourceUniqueID = Guid.Parse(reader["SourceUniqueID"].ToString()),
                            TrackPosition = Convert.ToInt32(reader["TrackPosition"]),
                            Title = reader["Title"].ToString(),
                            Mp3Url = reader["Mp3Url"].ToString(),
                            Mp3Mu5 = reader["Mp3Mu5"].ToString()
                        };
                    }
                }
            }
            return source;
        }

        // Update
        public void UpdatePerformedSongSource(PerformedSongSource source)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"UPDATE PerformedSongSource 
                             SET PerformedSongID = @PerformedSongID, 
                                 ShowID = @ShowID,
                                 SourceUniqueID = @SourceUniqueID,
                                 TrackPosition = @TrackPosition,
                                 Title = @Title,
                                 Mp3Url = @Mp3Url,
                                 Mp3Mu5 = @Mp3Mu5
                             WHERE ID = @ID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PerformedSongID", source.PerformedSongID);
                command.Parameters.AddWithValue("@ShowID", source.ShowID);
                command.Parameters.AddWithValue("@SourceUniqueID", source.SourceUniqueID);
                command.Parameters.AddWithValue("@TrackPosition", source.TrackPosition);
                command.Parameters.AddWithValue("@Title", source.Title);
                command.Parameters.AddWithValue("@Mp3Url", source.Mp3Url);
                command.Parameters.AddWithValue("@Mp3Mu5", source.Mp3Mu5);
                command.Parameters.AddWithValue("@ID", source.ID);
                command.ExecuteNonQuery();
            }
        }

        // Delete
        public void DeletePerformedSongSource(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM PerformedSongSource WHERE ID = @ID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", id);
                command.ExecuteNonQuery();
            }
        }
    }

}