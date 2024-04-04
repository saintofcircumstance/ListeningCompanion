using System;
using Microsoft.Data.SqlClient;

namespace ListeningCompanionDataService.Models.MasterData
{
    public class SetListService
    {
        private readonly string _connectionString;

        public SetListService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateSetList(SetList setList)
        {
            string query = @"INSERT INTO SetList (SetListUniqueID, ShowID, SetSequence, RunTime, RunTimeSeconds) 
                             VALUES (@SetListUniqueID, @ShowID, @SetSequence, @RunTime, @RunTimeSeconds)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SetListUniqueID", setList.SetListUniqueID);
                    command.Parameters.AddWithValue("@ShowID", setList.ShowID);
                    command.Parameters.AddWithValue("@SetSequence", setList.SetSequence);
                    command.Parameters.AddWithValue("@RunTime", setList.RunTime);
                    command.Parameters.AddWithValue("@RunTimeSeconds", setList.RunTimeSeconds);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public SetList ReadSetList(int setListId)
        {
            SetList setList = null;

            string query = "SELECT * FROM SetList WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", setListId);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            setList = new SetList
                            {
                                ID = (int)reader["ID"],
                                SetListUniqueID = (Guid)reader["SetListUniqueID"],
                                ShowID = (int)reader["ShowID"],
                                SetSequence = (int)reader["SetSequence"],
                                RunTime = reader["RunTime"].ToString(),
                                RunTimeSeconds = reader["RunTimeSeconds"].ToString()
                            };
                        }
                    }
                }
            }

            return setList;
        }

        public void UpdateSetList(SetList setList)
        {
            string query = @"UPDATE SetList 
                             SET SetListUniqueID = @SetListUniqueID, 
                                 ShowID = @ShowID, 
                                 SetSequence = @SetSequence, 
                                 RunTime = @RunTime, 
                                 RunTimeSeconds = @RunTimeSeconds 
                             WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SetListUniqueID", setList.SetListUniqueID);
                    command.Parameters.AddWithValue("@ShowID", setList.ShowID);
                    command.Parameters.AddWithValue("@SetSequence", setList.SetSequence);
                    command.Parameters.AddWithValue("@RunTime", setList.RunTime);
                    command.Parameters.AddWithValue("@RunTimeSeconds", setList.RunTimeSeconds);
                    command.Parameters.AddWithValue("@ID", setList.ID);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteSetList(int setListId)
        {
            string query = "DELETE FROM SetList WHERE ID = @ID";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", setListId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
