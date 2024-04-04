using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ListeningCompanionDataService.Models.MasterData;
using ListeningCompanionDataService.Services.MasterData;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace ListeningCompanionDataService.Logic
{
    public class GratefulStatsImport
    {
        #region Fields 
        private const string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=ListeningCompanion;Integrated Security=True;";
        #endregion

        #region Import Performed Songs
        public async Task<string> ImportPerformedSongs()
        {
            try
            {
                PerformedSongService performedSongService = new PerformedSongService(connectionString);
                string getPerformedSongsSql = @"select ps.*, song.Id [SongPk], setList.Id [SetListPk]
                from _performedsongsapilandingtable ps
                join song song on song.SongUniqueID = ps.songId
                join SetList setList on setList.SetListUniqueID = ps.setId
                ";
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();
                SqlCommand getPerformedSongsCommand = new SqlCommand(getPerformedSongsSql, connection);
                SqlDataReader getPerformedSongsReader= await getPerformedSongsCommand.ExecuteReaderAsync();
                while (getPerformedSongsReader.Read())
                {


                    PerformedSong newPerformedSong = new PerformedSong();
                    newPerformedSong.SetListID = (int)getPerformedSongsReader["SetListPk"];
                    newPerformedSong.SongID = (int)getPerformedSongsReader["SongPk"];
                    newPerformedSong.SongSequence = (int)getPerformedSongsReader["songSequence"];
                    newPerformedSong.Segue = (bool)getPerformedSongsReader["segue"];
                    newPerformedSong.DaysSincePlayed = (int)getPerformedSongsReader["daysSincePlayed"];
                    newPerformedSong.LengthMMSS = getPerformedSongsReader["lengthMMSS"].ToString();
                    newPerformedSong.LengthSeconds = (int)getPerformedSongsReader["lengthSeconds"];


                    performedSongService.CreatePerformedSong(newPerformedSong);

                }
                connection.Close();
                return "Complete";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        #endregion

        #region Import Sets
        public async Task<string> ImportSets()
        {
            try
            {
                SetListService setListService = new SetListService(connectionString);
                string getSetListsSql= @"select * from _SetsAPILandingTable";
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();
                SqlCommand getSetListsCommand= new SqlCommand(getSetListsSql, connection);
                SqlDataReader getSetListsReader = await getSetListsCommand.ExecuteReaderAsync();
                while (getSetListsReader.Read())
                {


                    SetList newSetList = new SetList();
                    newSetList.SetListUniqueID= new Guid(getSetListsReader["setId"].ToString());

                    //get show id 
                    SqlConnection connectionShow = new SqlConnection();
                    connectionShow.ConnectionString = connectionString;
                    connectionShow.Open();
                    string showUniqueID = getSetListsReader["showId"].ToString();
                    string getShowIdSql = $"select top 1 [ID] from Show where ShowUniqueID = '{showUniqueID}'";
                    SqlCommand getShowIdCommand = new SqlCommand(getShowIdSql, connectionShow);
                    SqlDataReader getShowIdReader = await getShowIdCommand.ExecuteReaderAsync();
                    while (getShowIdReader.Read())
                    {
                        newSetList.ShowID = (int)getShowIdReader["ID"];
                    }
                    connectionShow.Close();
                    //end get show venue 

                    newSetList.SetSequence = (int)getSetListsReader["sequence"];
                    newSetList.RunTime = getSetListsReader["runningLength"].ToString();
                    newSetList.RunTimeSeconds = getSetListsReader["runningLengthSeconds"].ToString();

                    setListService.CreateSetList(newSetList);

                }
                connection.Close();
                return "Complete";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        #endregion

        #region Import Songs
        public async Task<string> ImportSongs()
        {
            try
            {
                SongService songService= new SongService(connectionString);
                string getSongListSql = @"select * from _SongsAPILandingTable";
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();
                SqlCommand getSongListCommand = new SqlCommand(getSongListSql, connection);
                SqlDataReader getSongListReader = await getSongListCommand.ExecuteReaderAsync();
                List<string> venueList = new List<string>();
                while (getSongListReader.Read())
                {


                    Song newSong = new Song();
                    newSong.SongUniqueID = new Guid(getSongListReader["songId"].ToString());
                    newSong.BandID = 1; //hardcoded to grateful dead for now
                    newSong.Title = getSongListReader["title"].ToString();
                    newSong.FirstPlayed = getSongListReader["firstPlayed"].ToString();

                    string firstShowUniqueID = getSongListReader["firstPlayedShowId"].ToString();
                    newSong.FirstPlayedShowUniqueID= new Guid(getSongListReader["songId"].ToString());
                    if (!firstShowUniqueID.IsNullOrEmpty())
                    {
                        newSong.FirstPlayedShowUniqueID = new Guid(firstShowUniqueID);
                    }
                    
                    newSong.LastPlayed = getSongListReader["lastPlayed"].ToString();

                    string lastShowUniqueID = getSongListReader["lastPlayedShowId"].ToString();
                    newSong.LastPlayedShowUniqueID = new Guid(getSongListReader["songId"].ToString());
                    if(!lastShowUniqueID.IsNullOrEmpty())
                    {
                        newSong.LastPlayedShowUniqueID = new Guid(lastShowUniqueID);
                    }
                    
                    newSong.CountPlayed = (int)getSongListReader["countPlayed"];

                    songService.CreateSong(newSong);

                }
                connection.Close();
                return "Complete";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        #endregion

        #region Import Venues
        public async Task<string> ImportVenues()
        {
            try
            {
                VenueService venueService = new VenueService(connectionString);
                string getVenueListSql = @"select * from _VenuesAPILandingTable";
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();
                SqlCommand getVenueListCommand = new SqlCommand(getVenueListSql, connection);
                SqlDataReader getVenueListReader = await getVenueListCommand.ExecuteReaderAsync();
                List<string> venueList = new List<string>();
                while (getVenueListReader.Read())
                {


                    Venue newVenue = new Venue();
                    newVenue.VenueUniqueID = new Guid(getVenueListReader["venueId"].ToString());
                    newVenue.VenueName = getVenueListReader["venue"].ToString();
                    newVenue.City = getVenueListReader["city"].ToString();
                    newVenue.State = getVenueListReader["state"].ToString();
                    newVenue.Country = getVenueListReader["country"].ToString();
                    venueService.CreateVenue(newVenue);

                }
                connection.Close();
                return "Complete";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }
        #endregion

        #region Import Shows
        public async Task<string> ImportShows()
        {
            try
            {
                ShowService showService = new ShowService(connectionString);
                string getShowListSql = @"select sd.*, s.[DayName] from _ShowDetailsAPILandingTable sd join _ShowsAPILandingTable s on s.showId = sd.showId";
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();
                SqlCommand getShowListCommand= new SqlCommand(getShowListSql, connection);
                SqlDataReader getShowListReader = await getShowListCommand.ExecuteReaderAsync();
                while (getShowListReader.Read())
                {


                    Show newShow = new Show();
                    newShow.ShowUniqueID= new Guid(getShowListReader["showId"].ToString());
                    newShow.BandID = 1; //hardcoded for Grateful Dead right now 

                    //get show venue
                    SqlConnection connectionVenue = new SqlConnection();
                    connectionVenue.ConnectionString = connectionString;
                    connectionVenue.Open();
                    string venueUniqueID = getShowListReader["venueId"].ToString();
                    string getVenueIdSql = $"select top 1 [ID] from Venue where VenueUniqueID = '{venueUniqueID}'";
                    SqlCommand getVenueIdCommand = new SqlCommand(getVenueIdSql, connectionVenue);
                    SqlDataReader getVenueIdReader = await getVenueIdCommand.ExecuteReaderAsync();
                    while (getVenueIdReader.Read())
                    {
                        newShow.VenueID = (int)getVenueIdReader["ID"];
                    }
                    connectionVenue.Close();
                    //end get show venue 
                    
                    newShow.ShowDate = (DateTime)getShowListReader["showDate"];
                    newShow.ShowDayName = getShowListReader["DayName"].ToString();
                    newShow.City = getShowListReader["city"].ToString();
                    newShow.State= getShowListReader["state"].ToString();
                    newShow.Country = getShowListReader["country"].ToString();
                    newShow.RunTime = getShowListReader["runningLength"].ToString();

                    showService.CreateShow(newShow);

                }
                connection.Close();
                return "Complete";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        #endregion

    }
}
