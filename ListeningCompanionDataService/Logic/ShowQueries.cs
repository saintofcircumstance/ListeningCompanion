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

        #region Get Venue List
        public async Task<List<VenueDetails>> GetVenueListForBand(int bandId)
        {
            List<VenueDetails> venueList = new List<VenueDetails>();
            try
            {
                string venueListSql = @"select distinct v.ID
                , CONCAT(v.VenueName, ' - ', v.City, ', ', v.[State], ', ', v.Country) [VenueDisplayName]
                , v.VenueName
                from Venue v 
                join Show s on s.VenueID = v.ID
				join Band b on b.ID = s.BandID and b.ID = @BandID
                order by v.VenueName";

                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqlCommand getVenueListCommand = new SqlCommand(venueListSql, connection);
                getVenueListCommand.Parameters.AddWithValue("@BandID", bandId);
                SqlDataReader getVenueListReader = await getVenueListCommand.ExecuteReaderAsync();
                while (getVenueListReader.Read())
                {
                    VenueDetails venueDetails = new VenueDetails();
                    venueDetails.VenueID = (int)getVenueListReader["ID"];
                    venueDetails.VenueName = getVenueListReader["VenueName"].ToString();
                    venueDetails.VenueDisplayName = getVenueListReader["VenueDisplayName"].ToString();

                    venueList.Add(venueDetails);
                }
                return venueList;
            }
            catch (Exception ex)
            {
                return venueList;
            }
        }
        #endregion

        #region Get User Details
        public async Task<UserDetails> GetDetailsForUser(int userId)
        {
            UserDetails userDetails = new UserDetails();
            List<UserShowDetails> userShowDetailsList = new List<UserShowDetails>();
            List<UserSongDetails> userSongDetailsList = new List<UserSongDetails>();
            try
            {
                //get user shows 
                string userShowDetailsSql = @"select s.ID [ShowID], uShow.ID [UserShowID], b.BandName
                , CONCAT(v.VenueName, ' - ', v.City, ', ', v.[State], ', ', v.Country) [VenueName]
                , TRY_CAST(s.ShowDate as Date) [Date]
                , CASE WHEN uShow.InteractionStatus is null THEN 'None' ELSE uShow.InteractionStatus END [InteractionStatus]
                , uShow.BookMarked [ShowBookmarked]
                , uShow.Liked [ShowLiked]
                , uShow.Rating [ShowRating]
                , uShow.Notes [ShowNotes]
                from Show s 
                join Venue v on v.ID = s.VenueID
                join Band b on b.ID = s.BandID
                left join ApplicationUser au on au.ID = @UserID 
                join UserShow uShow on uShow.ShowID = s.ID and uShow.UserID = au.ID
                order by s.ShowDate";

                SqlConnection showConnection = new SqlConnection();
                showConnection.ConnectionString = _connectionString;
                showConnection.Open();
                SqlCommand getShowDetailsCommand = new SqlCommand(userShowDetailsSql, showConnection);
                getShowDetailsCommand.Parameters.AddWithValue("@UserID", userId);
                SqlDataReader getUserShowDetailsReader = await getShowDetailsCommand.ExecuteReaderAsync();
                while (getUserShowDetailsReader.Read())
                {
                    UserShowDetails details = new UserShowDetails();
                    details.UserShowID = getUserShowDetailsReader["UserShowID"] == System.DBNull.Value ? -1 : (int)getUserShowDetailsReader["UserShowID"];
                    details.ShowID = (int)getUserShowDetailsReader["ShowID"];
                    details.BandName = getUserShowDetailsReader["BandName"].ToString();
                    details.VenueName = getUserShowDetailsReader["VenueName"].ToString();
                    DateTime showDate = (DateTime)getUserShowDetailsReader["Date"];
                    details.Date = showDate.Month.ToString() + "/" + showDate.Day.ToString() + "/" + showDate.Year.ToString();
                    details.InteractionStatus = getUserShowDetailsReader["InteractionStatus"].ToString();

                    details.ShowBookMarked = getUserShowDetailsReader["ShowBookmarked"] == System.DBNull.Value ? false : (bool)getUserShowDetailsReader["ShowBookmarked"];
                    details.ShowLiked = getUserShowDetailsReader["ShowLiked"] == System.DBNull.Value ? false : (bool)getUserShowDetailsReader["ShowLiked"];
                    details.ShowRating = getUserShowDetailsReader["ShowRating"] == System.DBNull.Value ? 0 : (int)getUserShowDetailsReader["ShowRating"];
                    details.ShowNotes = getUserShowDetailsReader["ShowNotes"].ToString();
                    userShowDetailsList.Add(details);
                }

                //get user songs
                //, TRY_CAST(sl.[SetSequence] as nvarchar(MAX)), '.', TRY_CAST(ps.SongSequence as nvarchar(MAX)), ' '
                string userSongDetailsSql = @"select 
                uPs.ID [UserPerformedSongId]
                , ps.ID [PerformedSongId]
                , s.ID [ShowID]
                ,CONCAT('(',TRY_CAST(TRY_CAST(s.ShowDate as Date) as nvarchar(max)), ') ',sg.Title, CASE WHEN ps.Segue = 1 THEN '>' ELSE '' END) [SongName]
                , sl.SetSequence
                , ps.SongSequence
                , uPs.BookMarked [SongBookmarked]
                , uPs.Liked [SongLiked]
                , ups.Rating [SongRating]
                , ups.Notes [SongNotes]
                from Show s 
                join Venue v on v.ID = s.VenueID
                join Band b on b.ID = s.BandID
                join SetList sl on sl.ShowID = s.ID
                join PerformedSong ps on ps.SetListID = sl.ID
                join Song sg on sg.ID = ps.SongID
                join ApplicationUser au on au.ID = @UserID 
                left join UserShow uShow on uShow.ShowID = s.ID and uShow.UserID = au.ID
                join UserPerformedSong uPs on uPs.PerformedSongID = ps.ID and uPs.UserID = au.ID
                order by s.ShowDate, -sl.SetSequence, ps.SongSequence";
                SqlConnection songConnection = new SqlConnection();
                songConnection.ConnectionString = _connectionString;
                songConnection.Open();
                SqlCommand getSongDetailsCommand = new SqlCommand(userSongDetailsSql, songConnection);
                getSongDetailsCommand.Parameters.AddWithValue("@UserID", userId);
                SqlDataReader getSongDetailsReader = await getSongDetailsCommand.ExecuteReaderAsync();
                while (getSongDetailsReader.Read())
                {
                    UserSongDetails userSongDetails = new UserSongDetails();
                    userSongDetails.UserPerformedSongId = getSongDetailsReader["UserPerformedSongId"] == System.DBNull.Value ? -1 : (int)getSongDetailsReader["UserPerformedSongId"];
                    userSongDetails.ShowId = (int)getSongDetailsReader["ShowId"];
                    userSongDetails.PerformedSongId = (int)getSongDetailsReader["PerformedSongId"];
                    userSongDetails.SetSequence = (int)getSongDetailsReader["SetSequence"];
                    userSongDetails.SongName = getSongDetailsReader["SongName"].ToString();
                    userSongDetails.SongSequence = (int)getSongDetailsReader["SongSequence"];
                    userSongDetails.SongBookmarked = getSongDetailsReader["SongBookmarked"] == System.DBNull.Value ? false : (bool)getSongDetailsReader["SongBookmarked"];
                    userSongDetails.SongLiked = getSongDetailsReader["SongLiked"] == System.DBNull.Value ? false : (bool)getSongDetailsReader["SongLiked"];
                    userSongDetails.SongRating = getSongDetailsReader["SongRating"] == System.DBNull.Value ? 0 : (int)getSongDetailsReader["SongRating"];
                    userSongDetails.SongNotes = getSongDetailsReader["SongNotes"].ToString();


                    userSongDetailsList.Add(userSongDetails);
                }

                userDetails.UserID = userId;
                userDetails.UserShowDetails = userShowDetailsList;
                userDetails.UserSongDetails = userSongDetailsList;
                return userDetails;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new UserDetails();
            }
        }
        #endregion

        #region Get User Show Details 
        public async Task<List<UserShowDetails>> GetUserShowDetails(int bandId, int userId, DateTime? date, DateTime? endDateTime = null, bool filterYear = false, bool filterMonth = false, bool filterDay = false, int filterVenueId= -1) 
        {
            List<UserShowDetails> userShowList = new List<UserShowDetails>();
            string whereClause= BuildWhereClauseForShowDetails(filterYear, filterMonth, filterDay, date, endDateTime, filterVenueId);
            
            try
            {
                string userShowDetailsSql = @"select s.ID [ShowID], uShow.ID [UserShowID], b.BandName
                , CONCAT(v.VenueName, ' - ', v.City, ', ', v.[State], ', ', v.Country) [VenueName]
                , TRY_CAST(s.ShowDate as Date) [Date]
                , CASE WHEN uShow.InteractionStatus is null THEN 'None' ELSE uShow.InteractionStatus END [InteractionStatus]
                , uShow.BookMarked [ShowBookmarked]
                , uShow.Liked [ShowLiked]
                , uShow.Rating [ShowRating]
                , uShow.Notes [ShowNotes]
                from Show s 
                join Venue v on v.ID = s.VenueID
                join Band b on b.ID = s.BandID and b.ID = @BandId
                left join ApplicationUser au on au.ID = @UserID 
                left join UserShow uShow on uShow.ShowID = s.ID and uShow.UserID = au.ID" 
                + $" {whereClause}"
                + " order by s.ShowDate";


                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqlCommand getShowDetailsCommand = new SqlCommand(userShowDetailsSql, connection);
                getShowDetailsCommand.Parameters.AddWithValue("@BandID", bandId);
                getShowDetailsCommand.Parameters.AddWithValue("@UserID", userId);
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
        #region Get Songs for Show
        public async Task<List<UserSongDetails>> GetSongsForShowAndUserId(int showId, int userId)
        {
            List<UserSongDetails> userSongList = new List<UserSongDetails>();
            try
            {
                string userSongDetailsSql= @"select 
                uPs.ID [UserPerformedSongId]
                , ps.ID [PerformedSongId]
                ,CONCAT(TRY_CAST(sl.[SetSequence] as nvarchar(MAX)), '.', TRY_CAST(ps.SongSequence as nvarchar(MAX)), ' ',sg.Title, CASE WHEN ps.Segue = 1 THEN '>' ELSE '' END) [SongName]
                , sl.SetSequence
                , ps.SongSequence
                , uPs.BookMarked [SongBookmarked]
                , uPs.Liked [SongLiked]
                , ups.Rating [SongRating]
                , ups.Notes [SongNotes]
                from Show s 
                join Venue v on v.ID = s.VenueID
                join Band b on b.ID = s.BandID
                join SetList sl on sl.ShowID = s.ID
                join PerformedSong ps on ps.SetListID = sl.ID
                join Song sg on sg.ID = ps.SongID
                left join ApplicationUser au on au.ID = @UserID 
                left join UserShow uShow on uShow.ShowID = s.ID and uShow.UserID = au.ID
                left join UserPerformedSong uPs on uPs.PerformedSongID = ps.ID and uPs.UserID = au.ID
                WHERE 
                s.ID = @ShowID
                order by sl.SetSequence, ps.SongSequence";
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = _connectionString;
                connection.Open();
                SqlCommand getSongDetailsCommand = new SqlCommand(userSongDetailsSql, connection);
                getSongDetailsCommand.Parameters.AddWithValue("@ShowID", showId);
                getSongDetailsCommand.Parameters.AddWithValue("@UserID", userId);
                SqlDataReader getSongDetailsReader= await getSongDetailsCommand.ExecuteReaderAsync();
                while (getSongDetailsReader.Read())
                {
                    UserSongDetails userSongDetails = new UserSongDetails();
                    userSongDetails.UserPerformedSongId= getSongDetailsReader["UserPerformedSongId"] == System.DBNull.Value ? -1 : (int)getSongDetailsReader["UserPerformedSongId"];
                    userSongDetails.ShowId = showId;
                    userSongDetails.PerformedSongId = (int)getSongDetailsReader["PerformedSongId"];
                    userSongDetails.SetSequence = (int)getSongDetailsReader["SetSequence"];
                    userSongDetails.SongName = getSongDetailsReader["SongName"].ToString();
                    userSongDetails.SongSequence = (int)getSongDetailsReader["SongSequence"];
                    userSongDetails.SongBookmarked = getSongDetailsReader["SongBookmarked"] == System.DBNull.Value ? false : (bool)getSongDetailsReader["SongBookmarked"];
                    userSongDetails.SongLiked = getSongDetailsReader["SongLiked"] == System.DBNull.Value ? false : (bool)getSongDetailsReader["SongLiked"];
                    userSongDetails.SongRating= getSongDetailsReader["SongRating"] == System.DBNull.Value ? 0 : (int)getSongDetailsReader["SongRating"];
                    userSongDetails.SongNotes= getSongDetailsReader["SongNotes"].ToString();


                    userSongList.Add(userSongDetails);
                }

                return userSongList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<UserSongDetails>();
            }

        }
        #endregion

        #region Get User Set Details

        #endregion

        #region Helper Functions
        public string BuildWhereClauseForShowDetails(bool filterYear, bool filterMonth, bool filterDay, DateTime? startDateTime, DateTime? endDateTime, int venueID)
        {
            StringBuilder whereClause = new StringBuilder("WHERE ");

            // Check if any filters are enabled
            if (filterYear || filterMonth || filterDay || venueID != -1)
            {
                // Initialize flag to determine whether to add 'AND' keyword
                bool andFlag = false;

                if (filterYear && startDateTime.HasValue)
                {
                    whereClause.Append($"YEAR(ShowDate) = {startDateTime.Value.Year}");
                    andFlag = true;
                }

                if (filterMonth && startDateTime.HasValue)
                {
                    if (andFlag)
                        whereClause.Append(" AND ");
                    whereClause.Append($"MONTH(ShowDate) = {startDateTime.Value.Month}");
                    andFlag = true;
                }

                if (filterDay && startDateTime.HasValue)
                {
                    if (andFlag)
                        whereClause.Append(" AND ");
                    whereClause.Append($"DAY(ShowDate) = {startDateTime.Value.Day}");
                    andFlag = true;
                }

                if (venueID != -1)
                {
                    if (andFlag)
                        whereClause.Append(" AND ");
                    whereClause.Append($"v.ID = {venueID}");
                    andFlag = true;
                }

                // If endDateTime is provided, add condition to limit within the range
                if (endDateTime.HasValue)
                {
                    if (andFlag)
                        whereClause.Append(" AND ");
                    whereClause.Append($"ShowDate <= '{endDateTime.Value.ToString("yyyy-MM-dd")}'");
                }
            }
            else if (startDateTime.HasValue)
            {
                // If no filters are enabled but startDateTime is provided, filter by startDateTime
                whereClause.Append($"ShowDate >= '{startDateTime.Value.ToString("yyyy-MM-dd")}'");
            }

            return whereClause.ToString();
        }
        #endregion
    }
}
