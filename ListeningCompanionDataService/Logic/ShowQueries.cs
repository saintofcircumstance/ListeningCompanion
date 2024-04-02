﻿using System;
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
                , CONCAT(v.VenueName, ' - ', v.City, ', ', v.[State], ', ', v.Country) [VenueName]
                , TRY_CAST(s.ShowDate as Date) [Date]
                , CASE WHEN uShow.InteractionStatus is null THEN 'None' ELSE uShow.InteractionStatus END [InteractionStatus]
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
        #region Get Songs for Show
        public async Task<List<UserSongDetails>> GetSongsForShowAndUserId(int showId, int userId)
        {
            List<UserSongDetails> userSongList = new List<UserSongDetails>();
            try
            {
                string userSongDetailsSql= @"select 
                uPs.ID [UserPerformedSongId]
                , ps.ID [PerformedSongId]
                ,CONCAT(sg.Title, CASE WHEN ps.Segue = 1 THEN '>' ELSE '' END) [SongName]
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
    }
}