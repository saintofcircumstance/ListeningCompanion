using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ListeningCompanionAPIService.Models;
using ListeningCompanionDataService.Models.MasterData;
using ListeningCompanionDataService.Models.View;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ListeningCompanionDataService.Logic
{
    public class ShowQueries
    {
        private readonly string _connectionString;

        public ShowQueries(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region Get Today's Shows
        public async Task<Dictionary<int, string>> GetTodaysShows(int bandId)
        {
            Dictionary<int, string> todaysShowDictionary = new Dictionary<int, string>();
            try
            {
                string currentDayShowSql = @"select s.ID, CONCAT(TRY_CAST(s.ShowDate as Date), ' - ', v.VenueName, ' - ', b.BandName) [ShowInfo]
            from Show s
            join Venue v on v.ID = s.VenueID
            join Band b on b.ID = s.BandID
            where
            DATEPART(dayofyear, TRY_CAST(s.ShowDate as Date)) = DATEPART(dayofyear, TRY_CAST(GetDate() as Date))
            and b.ID = @BandID
            order by s.ShowDate";

                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqlCommand getCurrentDayShowsCommand = new SqlCommand(currentDayShowSql, connection);
                getCurrentDayShowsCommand.Parameters.AddWithValue("@BandID", bandId);
                SqlDataReader getCurrentDayShowsReader = await getCurrentDayShowsCommand.ExecuteReaderAsync();
                while (getCurrentDayShowsReader.Read())
                {
                    int showId = (int)getCurrentDayShowsReader["ID"];
                    string showInfo = getCurrentDayShowsReader["ShowInfo"].ToString();
                    todaysShowDictionary.Add(showId, showInfo);
                }
                return todaysShowDictionary;
            }
            catch(Exception ex) { 
                todaysShowDictionary.Add(-1, ex.Message);
                return todaysShowDictionary;
            }
        }
        #endregion

        #region Get User Show Details 
        public async Task<List<UserShowDetails>> GetUserShowDetails(int bandId, int userId, DateTime? date) 
        {
            List<UserShowDetails> userShowList = new List<UserShowDetails>();
            try
            {
                string userShowDetailsSql = @"select s.ID [ShowID], uShow.ID [UserShowID], b.BandName
                , v.VenueName
                , TRY_CAST(s.ShowDate as Date) [Date]
                , uShow.InteractionStatus 
                , uShow.BookMarked [ShowBookmarked]
                , uShow.Liked [ShowLiked]
                , uShow.Rating [ShowRating]
                , uShow.Notes [ShowNotes]
                --, CONCAT(sg.Title, CASE WHEN ps.Segue = 1 THEN '>' ELSE '' END) [Song]
                from Show s 
                join Venue v on v.ID = s.VenueID
                join Band b on b.ID = s.BandID and b.ID = @bandId
                left join ApplicationUser au on au.ID = @UserID 
                left join UserShow uShow on uShow.ShowID = s.ID and uShow.UserID = au.ID
                WHERE 
                --DATEPART(dayofyear, TRY_CAST(s.ShowDate as Date)) = DATEPART(dayofyear,TRY_CAST(GetDate() as Date))
                --(@Date is null or DATEPART(dayofyear, TRY_CAST(s.ShowDate as Date)) = DATEPART(dayofyear,TRY_CAST(@Date as Date)))
                (@Date is null or (MONTH(ShowDate) = MONTH(@Date) AND DAY(ShowDate) = Day(@Date)))
                order by s.ShowDate";
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqlCommand getShowDetailsCommand = new SqlCommand(userShowDetailsSql, connection);
                getShowDetailsCommand.Parameters.AddWithValue("@BandID", bandId);
                getShowDetailsCommand.Parameters.AddWithValue("@UserID", userId);
                getShowDetailsCommand.Parameters.AddWithValue("@Date", date.Value.Date);
                SqlDataReader getUserShowDetailsReader = await getShowDetailsCommand.ExecuteReaderAsync();
                while (getUserShowDetailsReader.Read())
                {
                    UserShowDetails details = new UserShowDetails();
                    details.UserShowID = getUserShowDetailsReader["UserShowID"] == System.DBNull.Value ? -1 : (int)getUserShowDetailsReader["UserShowID"];
                    details.ShowID = (int)getUserShowDetailsReader["ShowID"];
                    details.BandName = getUserShowDetailsReader["BandName"].ToString();
                    details.VenueName = getUserShowDetailsReader["VenueName"].ToString();
                    DateTime showDate = (DateTime)getUserShowDetailsReader["Date"];
                    details.Date= showDate.Month.ToString() + "/" + showDate.Day.ToString() + "/" + showDate.Year.ToString();
                    details.InteractionStatus= getUserShowDetailsReader["InteractionStatus"].ToString();
                    
                    details.ShowBookMarked= getUserShowDetailsReader["ShowBookmarked"] == System.DBNull.Value ? false : (bool)getUserShowDetailsReader["ShowBookmarked"];
                    details.ShowLiked= getUserShowDetailsReader["ShowLiked"] == System.DBNull.Value ? false : (bool)getUserShowDetailsReader["ShowLiked"];
                    details.ShowRating = getUserShowDetailsReader["ShowRating"] == System.DBNull.Value ? 0: (int)getUserShowDetailsReader["ShowRating"];
                    details.ShowNotes = getUserShowDetailsReader["ShowNotes"].ToString();
                    userShowList.Add(details);
                }

                return userShowList;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<UserShowDetails>();
            }
            
        }
        #endregion

        #region Get User Set Details
        
        #endregion
    }
}
