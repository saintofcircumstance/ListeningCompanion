using System;
using ListeningCompanionDataService.Models.MasterData;
using Microsoft.Data.SqlClient;

namespace ListeningCompanionDataService.Services.MasterData
{


    public class BandService
    {
        private readonly string _connectionString;

        public BandService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Band ReadBand(int bandId)
        {
            Band band = null;

            string query = "SELECT * FROM Band WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", bandId);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            band = new Band
                            {
                                ID = (int)reader["ID"],
                                BandName = reader["BandName"].ToString()
                            };
                        }
                    }
                }
            }

            return band;
        }

        public void CreateBand(Band band)
        {
            string query = "INSERT INTO Band (BandName) VALUES (@BandName)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BandName", band.BandName);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateBand(Band band)
        {
            string query = "UPDATE Band SET BandName = @BandName WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BandName", band.BandName);
                    command.Parameters.AddWithValue("@ID", band.ID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteBand(int bandId)
        {
            string query = "DELETE FROM Band WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", bandId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }

}
