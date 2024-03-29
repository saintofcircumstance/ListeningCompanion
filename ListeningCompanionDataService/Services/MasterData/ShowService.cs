using System;
using Microsoft.Data.SqlClient;

namespace ListeningCompanionDataService.Models.MasterData
{
    public class ShowService
    {
        private readonly string _connectionString;

        public ShowService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateShow(Show show)
        {
            string query = @"INSERT INTO Show (ShowUniqueID, BandID, VenueID, ShowDate, ShowDayName, City, [State], Country, RunTime) 
                             VALUES (@ShowUniqueID, @BandID, @VenueID, @ShowDate, @ShowDayName, @City, @State, @Country, @RunTime)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ShowUniqueID", show.ShowUniqueID);
                    command.Parameters.AddWithValue("@BandID", show.BandID);
                    command.Parameters.AddWithValue("@VenueID", show.VenueID);
                    command.Parameters.AddWithValue("@ShowDate", show.ShowDate);
                    command.Parameters.AddWithValue("@ShowDayName", show.ShowDayName);
                    command.Parameters.AddWithValue("@City", show.City);
                    command.Parameters.AddWithValue("@State", show.State);
                    command.Parameters.AddWithValue("@Country", show.Country);
                    command.Parameters.AddWithValue("@RunTime", show.RunTime);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public Show ReadShow(int showId)
        {
            Show show = null;

            string query = "SELECT * FROM Show WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", showId);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            show = new Show
                            {
                                ID = (int)reader["ID"],
                                ShowUniqueID = (Guid)reader["ShowUniqueID"],
                                BandID = (int)reader["BandID"],
                                VenueID = (int)reader["VenueID"],
                                ShowDate = (DateTime)reader["ShowDate"],
                                ShowDayName = reader["ShowDayName"].ToString(),
                                City = reader["City"].ToString(),
                                State = reader["State"].ToString(),
                                Country = reader["Country"].ToString(),
                                RunTime = reader["RunTime"].ToString()
                            };
                        }
                    }
                }
            }

            return show;
        }

        public void UpdateShow(Show show)
        {
            string query = @"UPDATE Show 
                             SET ShowUniqueID = @ShowUniqueID, 
                                 BandID = @BandID, 
                                 VenueID = @VenueID, 
                                 ShowDate = @ShowDate, 
                                 ShowDayName = @ShowDayName, 
                                 City = @City, 
                                 [State] = @State, 
                                 Country = @Country, 
                                 RunTime = @RunTime 
                             WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ShowUniqueID", show.ShowUniqueID);
                    command.Parameters.AddWithValue("@BandID", show.BandID);
                    command.Parameters.AddWithValue("@VenueID", show.VenueID);
                    command.Parameters.AddWithValue("@ShowDate", show.ShowDate);
                    command.Parameters.AddWithValue("@ShowDayName", show.ShowDayName);
                    command.Parameters.AddWithValue("@City", show.City);
                    command.Parameters.AddWithValue("@State", show.State);
                    command.Parameters.AddWithValue("@Country", show.Country);
                    command.Parameters.AddWithValue("@RunTime", show.RunTime);
                    command.Parameters.AddWithValue("@ID", show.ID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteShow(int showId)
        {
            string query = "DELETE FROM Show WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", showId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
