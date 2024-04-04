using System;
using Microsoft.Data.SqlClient;

namespace ListeningCompanionDataService.Models.MasterData
{
    public class VenueService
    {
        private readonly string _connectionString;

        public VenueService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateVenue(Venue venue)
        {
            string query = @"INSERT INTO Venue (VenueUniqueID, VenueName, City, State, Country) 
                             VALUES (@VenueUniqueID, @VenueName, @City, @State, @Country)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VenueUniqueID", venue.VenueUniqueID);
                    command.Parameters.AddWithValue("@VenueName", venue.VenueName);
                    command.Parameters.AddWithValue("@City", venue.City);
                    command.Parameters.AddWithValue("@State", venue.State);
                    command.Parameters.AddWithValue("@Country", venue.Country);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public Venue ReadVenue(int venueId)
        {
            Venue venue = null;

            string query = "SELECT * FROM Venue WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", venueId);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            venue = new Venue
                            {
                                ID = (int)reader["ID"],
                                VenueUniqueID = (Guid)reader["VenueUniqueID"],
                                VenueName = reader["VenueName"].ToString(),
                                City = reader["City"].ToString(),
                                State = reader["State"].ToString(),
                                Country = reader["Country"].ToString()
                            };
                        }
                    }
                }
            }

            return venue;
        }

        public void UpdateVenue(Venue venue)
        {
            string query = @"UPDATE Venue 
                             SET VenueUniqueID = @VenueUniqueID, 
                                 VenueName = @VenueName, 
                                 City = @City, 
                                 State = @State, 
                                 Country = @Country 
                             WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VenueUniqueID", venue.VenueUniqueID);
                    command.Parameters.AddWithValue("@VenueName", venue.VenueName);
                    command.Parameters.AddWithValue("@City", venue.City);
                    command.Parameters.AddWithValue("@State", venue.State);
                    command.Parameters.AddWithValue("@Country", venue.Country);
                    command.Parameters.AddWithValue("@ID", venue.ID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteVenue(int venueId)
        {
            string query = "DELETE FROM Venue WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", venueId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
