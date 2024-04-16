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

using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Net.NetworkInformation;

using ListeningCompanionAPIService.API.Models;

using ListeningCompanionDataService.Models.MasterData;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ListeningCompanionDataService.Services.MasterData;

namespace ListeningCompanionDataService.Logic
{

    public class RelistenIntegration
    {
        #region Fields 
        private const string connectionString = @"Server=tcp:listeningcompanion.database.windows.net,1433;Initial Catalog=ListeningCompanion;Persist Security Info=False;User ID=captaintrips;Password=TerrapinStation77!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        #endregion

        #region Test API
        ///keytest/validatekey
        public async Task<string> GetSource()
        {
            //var showDateList = await new RelistenImport(connectionString).GetShowDateList();
            ///await new RelistenImport(connectionString).UpdatePerformedSongSource();
            await new RelistenImport(connectionString).UpdatePerformedSong();
            /*try
            {
                foreach (KeyValuePair<int, string> keyValuePair in showDateList)
                {
                    await GetGratefulDeadShowRecordings(keyValuePair.Value, keyValuePair.Key);
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }*/
            return "Success";
            

            
        }
        #endregion

        #region API Calls
        public async Task GetGratefulDeadShowRecordings(string date, int showID)
        {
            // Construct the API request URL with query parameters
            string apiUrl = $"https://api.relisten.net/api/v2/artists/grateful-dead/shows/{date}";
            //var queryParams = $"?venue_id=all&year={date.Substring(0, 4)}&month={date.Substring(5, 2)}&day={date.Substring(8, 2)}";
            string requestUrl = apiUrl;// + queryParams;

            // Make the API request
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(requestUrl);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();

                        // Parse the JSON response
                        JObject data = JObject.Parse(responseData);
                        Root sourceList = JsonConvert.DeserializeObject<Root>(responseData);

                        // Check if any shows were found
                        if (sourceList != null)
                        {
                            // Extract recordings for the first show
                            foreach(var source in sourceList.sources)
                            {
                                PerformedShowSource showSource = new PerformedShowSource()
                                {
                                    ShowID = showID,
                                    SourceUniqueID = new Guid(source.uuid),
                                    AverageRating = (decimal)source.avg_rating,
                                    NumberOfRatings = source.num_reviews,
                                    IsSoundBoard = source.is_soundboard,
                                    IsRemaster = source.is_remaster,
                                    ShowUrl = source.links.FirstOrDefault().url

                                };
                                new PerformedShowSourceService(connectionString).AddPerformedShowSource(showSource);
                                foreach(var set in source.sets)
                                {
                                    foreach(var track in set.tracks)
                                    {
                                        PerformedSongSource songSource = new PerformedSongSource()
                                        {
                                            PerformedSongID = await GetPerformedSongID(showID, track.track_position, track.title),
                                            ShowID = showID,
                                            SourceUniqueID = new Guid(source.uuid),
                                            TrackPosition = track.track_position,
                                            Title = track.title,    
                                            Mp3Url = track.mp3_url,
                                            Mp3Mu5 = track.mp3_md5
                                        };
                                        new PerformedSongSourceService(connectionString).AddPerformedSongSource(songSource);

                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("No Grateful Dead shows found for this date.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error fetching data from Relisten API: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error fetching data from Relisten API: {ex.Message}");
                }
            }
        }

        #endregion

        #region Helper functions
        private async Task<int> GetPerformedSongID(int showID, int trackNumber, string trackTitle)
        {
            return await new RelistenImport(connectionString).GetPerformedSongID(showID, trackNumber, trackTitle);
          
        }
        #endregion


    }
}
