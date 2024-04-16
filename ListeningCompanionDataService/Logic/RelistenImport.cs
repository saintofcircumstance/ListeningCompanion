using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ListeningCompanionAPIService.Models;
using ListeningCompanionDataService.Models.MasterData;
using ListeningCompanionDataService.Models.View;
using ListeningCompanionDataService.Services.MasterData;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ListeningCompanionDataService.Logic
{
    public class RelistenImport
    {
        private readonly string _connectionString;

        public RelistenImport(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Get Show Date List
        public async Task<Dictionary<int,string>> GetShowDateList()
        {
            Dictionary<int,string> showDateList = new Dictionary<int, string>();
            try
            {
                string showDateIDSql = @"select distinct ID, ShowDate from Show where ShowDate > '06-09-1991' order by ShowDate";

                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqlCommand getShowDateIDCommand = new SqlCommand(showDateIDSql, connection);
                SqlDataReader getShowDateIDReader = await getShowDateIDCommand.ExecuteReaderAsync();
                while (getShowDateIDReader.Read())
                {

                    int showID = (int)getShowDateIDReader["ID"];
                    string showDate = (DateTime.Parse(getShowDateIDReader["ShowDate"].ToString()).ToString("yyyy-MM-dd"));
                    showDateList.Add(showID, showDate);
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                return showDateList;
            }
            return showDateList;
        }

        public async Task<Dictionary<int, string>> UpdatePerformedSongSource()
        {
            Dictionary<int, string> showDateList = new Dictionary<int, string>();
            try
            {
                string sourceIDSql = @"select ID from PerformedSongSource where ID > 349852";

                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqlCommand getSourceIDCommand = new SqlCommand(sourceIDSql, connection);
                SqlDataReader getSourceIDReader = await getSourceIDCommand.ExecuteReaderAsync();
                while (getSourceIDReader.Read())
                {

                    int sourceID = (int)getSourceIDReader["ID"];
                    PerformedSongSourceService service = new PerformedSongSourceService(_connectionString);
                    PerformedSongSource source = service.GetPerformedSongSource(sourceID);
                    source.PerformedSongID = await GetPerformedSongID(source.ShowID, source.TrackPosition, source.Title);
                    service.UpdatePerformedSongSource(source);
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                return showDateList;
            }
            return showDateList;
        }

        public async Task<Dictionary<int, string>> UpdatePerformedSong()
        {
            Dictionary<int, string> showDateList = new Dictionary<int, string>();
            try
            {
                string performedSongSql = @"select ID from PerformedSong where ID > 35153";
                string sourceIDSql = @"select top 1 pssource.ID from PerformedSongSource";

                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqlCommand getPerformedSongCommand= new SqlCommand(performedSongSql, connection);
                SqlDataReader getPerformedSongReader = await getPerformedSongCommand.ExecuteReaderAsync();
                while (getPerformedSongReader.Read())
                {

                    int performedSongID = (int)getPerformedSongReader["ID"];
                    string performedSongSourceSql = $"select top 1 pss.Mp3Url from PerformedSongSource pss join PerformedShowSource s on s.SourceUniqueID = pss.SourceUniqueID where pss.PerformedSongID = {performedSongID} order by s.AverageRating desc";

                    SqlConnection _conn = new SqlConnection();
                    _conn.ConnectionString = _connectionString;
                    _conn.Open();
                    SqlCommand getSourceCommand = new SqlCommand(performedSongSourceSql, _conn);
                    SqlDataReader getSourceReader = await getSourceCommand.ExecuteReaderAsync();
                    if (getSourceReader.Read()) {
                        string url = getSourceReader["Mp3Url"].ToString();
                        PerformedSongService psService = new PerformedSongService(_connectionString);
                        Models.MasterData.PerformedSong ps = psService.ReadPerformedSong(performedSongID);
                        ps.Mp3Url = url;
                        psService.UpdatePerformedSong(ps);

                    }
                    _conn.Close();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                return showDateList;
            }
            return showDateList;
        }

        #endregion

        #region Get Performed Song ID 
        public async Task<int> GetPerformedSongID(int showID, int trackPosition, string title)
        {
            int performedSongID = 38155;
            try
            {
                string performedSongIDSql = @"select 
                top 1 
                ps.ID 
                from PerformedSong ps
                join SetList setList on setList.ID = ps.SetListID
                join Show s on s.ID = setList.ShowID
                join Song song on song.ID = ps.SongID
                where 
                s.ID = @ShowID
                and (LOWER(song.Title) like  '%'+LOWER(@SongTitle) +'%'
                or LOWER(@SongTitle) like  '%'+LOWER(song.Title) +'%')";

                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqlCommand getPerformedSongIDCommand = new SqlCommand(performedSongIDSql, connection);
                getPerformedSongIDCommand.Parameters.AddWithValue("@ShowID", showID);
                getPerformedSongIDCommand.Parameters.AddWithValue("@SongTitle", title);
                SqlDataReader getPerformedSongIDReader = await getPerformedSongIDCommand.ExecuteReaderAsync();
                while (getPerformedSongIDReader.Read())
                {
                    performedSongID = (int)getPerformedSongIDReader["ID"];
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                return performedSongID;
            }
            return performedSongID;
        }
        #endregion
    }
}
