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
                        string insertSql = @"INSERT INTO [_ShowsAPILandingTable]
                        (showId, showDate, dayName, lengthSeconds, venueId, venue, city, state, country)
                        VALUES" + $"('{show.showId.Replace("'","''")}', '{show.showDate}', '{show.dayName.Replace("'", "''")}', '{show.lengthSeconds.Replace("'", "''")}', '{show.venueId.Replace("'", "''")}', '{show.venue.Replace("'", "''")}', '{show.city.Replace("'", "''")}', '{show.state.Replace("'", "''")}', '{show.country.Replace("'", "''")}')";
                        SqlCommand insertCommand = new SqlCommand(insertSql, connection);
                        int rowsAffected = insertCommand.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return "complete";
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
        #endregion
    }
}
