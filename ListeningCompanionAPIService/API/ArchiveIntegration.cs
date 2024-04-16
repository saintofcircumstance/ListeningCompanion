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
using Newtonsoft.Json.Linq;

namespace ListeningCompanionAPIService.API
{

    public class ArchiveIntegration
    {
        #region Fields 
        #endregion

        #region Test API
        ///keytest/validatekey
        public async Task<string> TestApi(string showIdentifier)
        {
            string date = "04/13/1983";
            GetGratefulDeadShowsOnDate(date);
            return "hi";
        }
        #endregion

        #region API Calls
        public void GetGratefulDeadShowsOnDate(string date)
        {
            // Construct the API request URL
            string apiUrl = $"https://archive.org/advancedsearch.php?q=collection:GratefulDead%20AND%20date:{date}&output=json";

            // Make the API request
            using (WebClient client = new WebClient())
            {
                try
                {
                    string response = client.DownloadString(apiUrl);

                    // Parse the JSON response
                    JObject data = JObject.Parse(response);

                    // Extract identifiers of Grateful Dead shows
                    JArray docs = (JArray)data["response"]["docs"];
                    if (docs != null && docs.Count > 0)
                    {
                        foreach (var doc in docs)
                        {
                            string identifier = doc["identifier"].ToString();
                            DownloadAudioFiles(identifier);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No Grateful Dead shows found on this date.");
                    }
                }
                catch (WebException ex)
                {
                    Console.WriteLine($"Error fetching data from Archive.org: {ex.Message}");
                }
            }
        }

        public void DownloadAudioFiles(string showIdentifier)
        {
            // Construct the API request URL
            string apiUrl = $"https://archive.org/advancedsearch.php?q=identifier:{showIdentifier}&output=json";

            // Make the API request
            using (WebClient client = new WebClient())
            {
                try
                {
                    string response = client.DownloadString(apiUrl);

                    // Parse the JSON response
                    JObject data = JObject.Parse(response);

                    // Extract URLs of audio files
                    JArray files = (JArray)data["response"]["docs"][0]["files"];
                    if (files != null && files.Count > 0)
                    {
                        foreach (var file in files)
                        {
                            string format = file["format"].ToString();
                            if (format == "VBR MP3")
                            {
                                string fileUrl = file["url"].ToString();

                                // Download the audio file
                                Console.WriteLine($"Downloading {fileUrl}");
                                client.DownloadFile(fileUrl, $"{showIdentifier}.mp3");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No audio files found for the show.");
                    }
                }
                catch (WebException ex)
                {
                    Console.WriteLine($"Error fetching data from Archive.org: {ex.Message}");
                }
            }
        }

        #endregion


    }
}
