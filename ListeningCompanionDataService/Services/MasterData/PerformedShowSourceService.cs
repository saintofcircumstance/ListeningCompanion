using System;
using Microsoft.Data.SqlClient;
using ListeningCompanionDataService.Models.MasterData;
namespace ListeningCompanionDataService.Services.MasterData
{
    public class PerformedShowSourceService
    {
        private readonly string connectionString;

        public PerformedShowSourceService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Create
        public int AddPerformedShowSource(PerformedShowSource source)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO PerformedShowSource (ShowID, SourceUniqueID, AverageRating, NumberOfRatings, IsSoundBoard, IsRemaster, ShowUrl) 
                             VALUES (@ShowID, @SourceUniqueID, @AverageRating, @NumberOfRatings, @IsSoundBoard, @IsRemaster, @ShowUrl);
                             SELECT SCOPE_IDENTITY();";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ShowID", source.ShowID);
                command.Parameters.AddWithValue("@SourceUniqueID", source.SourceUniqueID);
                command.Parameters.AddWithValue("@AverageRating", source.AverageRating);
                command.Parameters.AddWithValue("@NumberOfRatings", source.NumberOfRatings);
                command.Parameters.AddWithValue("@IsSoundBoard", source.IsSoundBoard);
                command.Parameters.AddWithValue("@IsRemaster", source.IsRemaster);
                command.Parameters.AddWithValue("@ShowUrl", source.ShowUrl);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        // Read
        public List<PerformedShowSource> GetAllPerformedShowSources()
        {
            List<PerformedShowSource> sources = new List<PerformedShowSource>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM PerformedShowSource";
                SqlCommand command = new SqlCommand(query, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sources.Add(new PerformedShowSource
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            ShowID = Convert.ToInt32(reader["ShowID"]),
                            SourceUniqueID = Guid.Parse(reader["SourceUniqueID"].ToString()),
                            AverageRating = Convert.ToDecimal(reader["AverageRating"]),
                            NumberOfRatings = Convert.ToInt32(reader["NumberOfRatings"]),
                            IsSoundBoard = Convert.ToBoolean(reader["IsSoundBoard"]),
                            IsRemaster = Convert.ToBoolean(reader["IsRemaster"]),
                            ShowUrl = reader["ShowUrl"].ToString()
                        });
                    }
                }
            }
            return sources;
        }

        // Update
        public void UpdatePerformedShowSource(PerformedShowSource source)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"UPDATE PerformedShowSource 
                             SET ShowID = @ShowID, 
                                 SourceUniqueID = @SourceUniqueID,
                                 AverageRating = @AverageRating,
                                 NumberOfRatings = @NumberOfRatings,
                                 IsSoundBoard = @IsSoundBoard,
                                 IsRemaster = @IsRemaster,
                                 ShowUrl = @ShowUrl
                             WHERE ID = @ID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ShowID", source.ShowID);
                command.Parameters.AddWithValue("@SourceUniqueID", source.SourceUniqueID);
                command.Parameters.AddWithValue("@AverageRating", source.AverageRating);
                command.Parameters.AddWithValue("@NumberOfRatings", source.NumberOfRatings);
                command.Parameters.AddWithValue("@IsSoundBoard", source.IsSoundBoard);
                command.Parameters.AddWithValue("@IsRemaster", source.IsRemaster);
                command.Parameters.AddWithValue("@ShowUrl", source.ShowUrl);
                command.Parameters.AddWithValue("@ID", source.ID);
                command.ExecuteNonQuery();
            }
        }

        // Delete
        public void DeletePerformedShowSource(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM PerformedShowSource WHERE ID = @ID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", id);
                command.ExecuteNonQuery();
            }
        }

    }
    }