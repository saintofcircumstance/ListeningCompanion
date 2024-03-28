using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;
using ListeningCompanionAPIService.Models;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Net.NetworkInformation;

namespace ListeningCompanionAPIService.API
{

    public class GratefulStatsIntegration
    {
        #region Fields 
        //TODO - load this from a resource file 
        public static string _apiKey = "$2a$11$J5A/l4BiXoSD72p5ELJKKuKdCdIJZvdXy.9xnPUyUJB2SDkwB8Pie";
        public static string _apiUserId = "bcieplic@gmail.com";

        private static HttpClient _sharedClient = new HttpClient()
        {
            BaseAddress = new Uri("https://api.gratefulstats.com"),
        };
        #endregion

        #region Test API
        ///keytest/validatekey
        public static async Task<string> TestApi()
        {
            try
            {
                string endpointUrl = "deadapi/v2/keytest/validatekey";

                _sharedClient.DefaultRequestHeaders.Add("apiKey", _apiKey);
                _sharedClient.DefaultRequestHeaders.Add("apiUserId", _apiUserId);
                HttpResponseMessage response = await _sharedClient.GetAsync(endpointUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("API response: " + responseBody);
                    return responseBody;
                }
                else
                {
                    Console.WriteLine("Failed to call API. Status code: " + response.StatusCode);
                    return "Failure";
                }
            }
            catch (Exception ex) {
                Console.Write(ex.ToString());
                return "Exception";
            }
        }
        #endregion

        #region Get Sets
        public static async Task<string> GetSets()
        {
            try
            {
                string getShowListSql = @"select distinct showList.showId, showList.showDate from _ShowsAPILandingTable showList 
                left join _ShowDetailsAPILandingTable showDetails on showDetails.showId = showList.showId
                where showDetails.showId is null
                order by showList.showDate";
                SqlConnection connection = new SqlConnection();
                string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=ListeningCompanion;Integrated Security=True;";
                connection.ConnectionString = connectionString;
                connection.Open();
                SqlCommand getShowListCommand = new SqlCommand(getShowListSql, connection);
                SqlDataReader getShowListReader = await getShowListCommand.ExecuteReaderAsync();
                List<string> showIdList = new List<string>();
                while(getShowListReader.Read())
                {
                    //get the show id from sql reader and add to list
                    string showId = getShowListReader["showId"].ToString();
                    showIdList.Add(showId);
                }
                connection.Close();
                foreach(string showId in  showIdList) 
                {
                    var showDetails = await GetShowDataByShowId(showId);
                    ShowDetails[] payloadArr;
                    ShowDetails payload = new ShowDetails();
                    if(showDetails != "Failure")
                    {
                        payloadArr = JsonSerializer.Deserialize<ShowDetails[]>(showDetails);
                        for(int i =  0; i < payloadArr.Length;i++)
                        {
                            payload = payloadArr[i];

                            //create disposable sql connection
                            SqlConnection connectionInsert = new SqlConnection();
                            connectionInsert.ConnectionString = connectionString;
                            connectionInsert.Open();

                            //insert show details
                            string insertShowDetailsSql = 
                            $"IF NOT EXISTS(select top 1 * from _ShowDetailsAPILandingTable where showId = '{payload.showId}') BEGIN " +
                            @"insert into [_ShowDetailsAPILandingTable]
                            (showId
                            , showDate
                            , venueId
                            , venue
                            , city
                            , state
                            , country
                            , runningLength)
                            VALUES"
                            + $"('{payload.showId}'" +
                            $", '{payload.showDate}'" +
                            $", '{payload.venueId.Replace("'", "''")}'" +
                            $", '{payload.venue.Replace("'", "''")}'" +
                            $", '{payload.city.Replace("'", "''")}'" +
                            $", '{payload.state.Replace("'", "''")}'" +
                            $", '{payload.country.Replace("'", "''")}'" +
                            $", '{payload.runningLength.Replace("'", "''")}') END";
                            
                            SqlCommand insertShowDetailsCommand = new SqlCommand(insertShowDetailsSql, connectionInsert);
                            int rowsAffected = insertShowDetailsCommand.ExecuteNonQuery();
                            

                            

                            //iterate through sets, insert set
                            foreach (Set set in payload.sets)
                            {
                                string insertSetSql =
                                $"IF NOT EXISTS(select top 1 * from _SetsAPILandingTable where setId= '{set.setId}') BEGIN " +
                                @"insert into [_SetsAPILandingTable]
                                (setId
                                , showId
                                , [sequence]
                                , runningLength
                                , runningLengthSeconds)
                                VALUES"
                                + $"('{set.setId}'" +
                                $", '{payload.showId}'" +
                                $", '{set.sequence}'" +
                                $", '{set.runningLength}'" +
                                $", '{set.runningLengthSeconds}') END";
                                SqlCommand insertSetCommand = new SqlCommand(insertSetSql, connectionInsert);
                                rowsAffected = insertSetCommand.ExecuteNonQuery();
                                //iterate through songs in set, insert into performed song
                                foreach (PerformedSong performedSong in set.songs)
                                {
                                    string insertPerformedSongSql = @"insert into [_PerformedSongsAPILandingTable]
                                    (songId
                                    , setId
                                    , songSequence
                                    , title
                                    , segue
                                    , daysSincePlayed
                                    , lengthMMSS
                                    , lengthSeconds)
                                    VALUES"
                                    + $"('{performedSong.songId}'" +
                                    $", '{set.setId}'" +
                                    $", '{performedSong.songSequence}'" +
                                    $", '{performedSong.title.Replace("'", "''")}'" +
                                    $", '{performedSong.segue}'" +
                                    $", '{performedSong.daysSincePlayed}'" +
                                    $", '{performedSong.lengthMMSS}'" +
                                    $", '{performedSong.lengthSeconds}')";

                                    SqlCommand insertPerformedSongCommand = new SqlCommand(insertPerformedSongSql, connectionInsert);
                                    rowsAffected = insertPerformedSongCommand.ExecuteNonQuery();
                                }
                            }

                            connectionInsert.Close();
                        }
                        
                    }
                    else
                    {
                        //wait 5 minutes to reset api 
                        System.Threading.Thread.Sleep(600001);
                    }
                    
                }
                return "Complete";

            }
            catch(Exception ex)
            {
                return ex.ToString();
                
            }
        }
        #endregion
        #region Get Venues
        public static async Task<string> GetVenues()
        {
            try
            {
                var venueList = await GetVenueList();
                VenuesPayload payload = new VenuesPayload();
                if (venueList!= "Failure")
                {
                    payload = JsonSerializer.Deserialize<VenuesPayload>(venueList);
                }
                foreach (Venue venue in payload.venues)
                {
                    //add song to db 
                    SqlConnection connection = new SqlConnection();
                    string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=ListeningCompanion;Integrated Security=True;";
                    connection.ConnectionString = connectionString;
                    connection.Open();
                    string insertSql = @"INSERT INTO [_VenuesAPILandingTable] 
                    (venueId
                    ,venue
                    ,city
                    ,state
                    ,country)
                    VALUES"
                    + $"('{venue.venueId}'" +
                    $", '{venue.venue.Replace("'", "''")}'" +
                    $", '{venue.city.Replace("'", "''")}'" +
                    $", '{venue.state.Replace("'", "''")}'" +
                    $",'{venue.country.Replace("'", "''")}')" ;
                    SqlCommand insertCommand = new SqlCommand(insertSql, connection);
                    int rowsAffected = insertCommand.ExecuteNonQuery();
                    connection.Close();

                }
                return "Complete";
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }
        }
        #endregion
        #region Get Songs 
        public static async Task<string> GetSongs()
        {
            try
            {
                var songList = await GetAllTunes();
                SongsPayload payload = new SongsPayload();
                if (songList != "Failure") 
                { 
                    payload = JsonSerializer.Deserialize<SongsPayload>(songList);
                }
                foreach(Song song in payload.alltunes)
                {
                    //add song to db 
                    SqlConnection connection = new SqlConnection();
                    string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=ListeningCompanion;Integrated Security=True;";
                    connection.ConnectionString = connectionString;
                    connection.Open();
                    string insertSql = @"INSERT INTO [_SongsAPILandingTable] 
                    (songId
                    , title
                    , firstPlayed
                    , firstPlayedShowId
                    , lastPlayed
                    , lastPlayedShowId
                    , countPlayed
                    , shortestSeconds
                    , shortestSecondsMMSS
                    , shortestShowId
                    , longestSeconds
                    , longestSecondsMMSS
                    , longestShowId)
                    VALUES"
                    + $"('{song.songId}'" +
                    $", '{song.title.Replace("'", "''")}'" +
                    $", '{song.firstPlayed}'" +
                    $", '{song.firstPlayedShowId}'" +
                    $",'{song.lastPlayed}'" +
                    $",'{song.lastPlayedShowId}'" +
                    $",'{song.countPlayed}'" +
                    $",'{song.shortestSeconds}'" +
                    $",'{song.shortestSecondsMMSS}'" +
                    $",'{song.shortestShowId}'" +
                    $",'{song.longestSeconds}'" +
                    $",'{song.longestSecondsMMSS}'" +
                    $",'{song.longestShowId}')";
                    SqlCommand insertCommand = new SqlCommand(insertSql, connection);
                    int rowsAffected = insertCommand.ExecuteNonQuery();
                    connection.Close();

                }
                return "Complete";
            }
            catch(Exception ex) {
                return ex.ToString();
            }
        }
        #endregion

        #region Get Shows
        public static async Task<string> GetShows()
        {
            try
            {
                Dictionary<int, ShowsPayload> dictShows = new Dictionary<int, ShowsPayload>();
                //pull shows for each year 
                for (int year = 1965; year < 1996; year++)
                {
                    var showList = await GetShowsForYear(year);
                    if(showList != "Failure")
                    {
                        ShowsPayload payload = JsonSerializer.Deserialize<ShowsPayload>(showList);
                        dictShows.Add(year, payload);
                    }
                    
                }

                //go through each year, grab each show and add to db 
                foreach (var dictShow in dictShows.Values)
                {
                    foreach (var show in dictShow.ShowsOneYear)
                    {
                        //add show to db 
                        SqlConnection connection = new SqlConnection();
                        string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=ListeningCompanion;Integrated Security=True;";
                        if (show.lengthSeconds.IsNullOrEmpty()) { show.lengthSeconds = "0"; }
                        connection.ConnectionString = connectionString;
                        connection.Open();
                        string insertSql = @"INSERT INTO [_ShowsAPILandingTable] (showId, showDate, dayName, lengthSeconds, venueId, venue, city, state, country) VALUES"
                        + $"('{show.showId.Replace("'","''")}'" +
                        $", '{show.showDate}'" +
                        $", '{show.dayName.Replace("'", "''")}'" +
                        $", '{show.lengthSeconds.Replace("'", "''")}'" +
                        $", '{show.venueId.Replace("'", "''")}'" +
                        $", '{show.venue.Replace("'", "''")}'" +
                        $", '{show.city.Replace("'", "''")}'" +
                        $", '{show.state.Replace("'", "''")}'" +
                        $", '{show.country.Replace("'", "''")}')";
                        SqlCommand insertCommand = new SqlCommand(insertSql, connection);
                        int rowsAffected = insertCommand.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return "Complete";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        } 

        #endregion

        #region Helper Functions
        private static async Task<string> GetShowsForYear(int year)
        {
            string endpointUrl = "deadapi/v2/years/getyeardata";
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri("https://api.gratefulstats.com"),
            };
            client.DefaultRequestHeaders.Add("apiKey", _apiKey);
            client.DefaultRequestHeaders.Add("apiUserId", _apiUserId);

            HttpResponseMessage response = await client.GetAsync(String.Concat(endpointUrl, "/", year.ToString()));


            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                response.Dispose();
                return responseBody;
            }
            else
            {
                Console.WriteLine("Failed to call API. Status code: " + response.StatusCode);
                return "Failure";
            }
            
        }

        private static async Task<string> GetAllTunes()
        {
            string endpointUrl = "deadapi/v2/tunes/gettunes";
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri("https://api.gratefulstats.com"),
            };
            client.DefaultRequestHeaders.Add("apiKey", _apiKey);
            client.DefaultRequestHeaders.Add("apiUserId", _apiUserId);

            HttpResponseMessage response = await client.GetAsync(endpointUrl);


            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                response.Dispose();
                return responseBody;
            }
            else
            {
                Console.WriteLine("Failed to call API. Status code: " + response.StatusCode);
                return "Failure";
            }
        }

        private static async Task<string> GetShowDataByShowId(string showId)
        {
            string endpointUrl = "deadapi/v2/shows/getshowdatabyshowid";
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri("https://api.gratefulstats.com"),
            };
            client.DefaultRequestHeaders.Add("apiKey", _apiKey);
            client.DefaultRequestHeaders.Add("apiUserId", _apiUserId);

            HttpResponseMessage response = await client.GetAsync(String.Concat(endpointUrl, "/", showId));


            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                response.Dispose();
                return responseBody;
            }
            else
            {
                Console.WriteLine("Failed to call API. Status code: " + response.StatusCode);
                return "Failure";
            }
        }

        private static async Task<string> GetVenueList()
        {
            string endpointUrl = "deadapi/v2/venues/getvenuedata";
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri("https://api.gratefulstats.com"),
            };
            client.DefaultRequestHeaders.Add("apiKey", _apiKey);
            client.DefaultRequestHeaders.Add("apiUserId", _apiUserId);

            HttpResponseMessage response = await client.GetAsync(endpointUrl);


            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                response.Dispose();
                return responseBody;
            }
            else
            {
                Console.WriteLine("Failed to call API. Status code: " + response.StatusCode);
                return "Failure";
            }
        }
        #endregion
    }
}
