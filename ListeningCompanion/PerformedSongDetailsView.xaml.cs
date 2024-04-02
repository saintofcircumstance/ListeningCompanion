using System.Windows.Input;
using ListeningCompanionDataService.Models.User;
using ListeningCompanionDataService.Models.View;
using Microsoft.IdentityModel.Tokens;

namespace ListeningCompanion;

public partial class PerformedSongDetailsView : ContentPage
{
    #region Fields 
    private const string connectionString = @"Server=tcp:listeningcompanion.database.windows.net,1433;Initial Catalog=ListeningCompanion;Persist Security Info=False;User ID=captaintrips;Password=TerrapinStation77!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

    public UserSongDetails currentUserSong { get; private set; }
    private ContentView journalContentView;

    // Define UI elements as class members for easy access
    public Entry notesEntry;
    public Picker ratingPicker;
    public Switch likedSwitch;
    public Switch bookmarkedSwitch;


    #endregion

    #region Constructors
    public PerformedSongDetailsView(UserSongDetails userSong)
	{
        currentUserSong= userSong;
		InitializeComponent();
        LoadSongDetails(userSong);		
	}
    #endregion

    #region Load Views

    public async void LoadSongDetails(UserSongDetails userSong)
    {

        // Initialize ContentView for 'Journal' view
        journalContentView = LoadJournalView(userSong);

        //Content = scrollView;
        Content = journalContentView;
    }

    public ContentView LoadJournalView(UserSongDetails userSong)
    {
        // Create the content for the 'Journal' view
        var journalContent = new StackLayout
        {
            Children =
            {
                new Label { Text = "Journal Content" },
                // Add other components related to 'Journal'
            }
        };


        // Text field for notes
        notesEntry = new Entry
        {
            Text = userSong.SongNotes
        };

        // Dropdown / wheel selection for rating
        ratingPicker = new Picker();
        for (int i = 1; i <= 10; i++)
        {
            ratingPicker.Items.Add(i.ToString());
        }
        ratingPicker.SelectedItem = userSong.SongRating.ToString();

        // Toggle switches for Liked and Bookmarked
        var likedLabel = new Label { Text = "Liked" };
        likedSwitch = new Switch();
        likedSwitch.IsToggled = userSong.SongLiked;
        var bookmarkedLabel = new Label { Text = "Bookmarked" };
        bookmarkedSwitch = new Switch();
        bookmarkedSwitch.IsToggled = userSong.SongBookmarked;

        

        // Save button
        var saveButton = new Button { Text = "Save" };
        saveButton.Clicked += OnSaveButtonClicked;

        // Add elements to layout
        journalContent.Children.Add(notesEntry);
        journalContent.Children.Add(new Label { Text = "Rating:" });
        journalContent.Children.Add(ratingPicker);
        journalContent.Children.Add(likedLabel);
        journalContent.Children.Add(likedSwitch);
        journalContent.Children.Add(bookmarkedLabel);
        journalContent.Children.Add(bookmarkedSwitch);
        journalContent.Children.Add(saveButton);


        return new ContentView
        {
            Content = journalContent,
        };
    }
    #endregion
    #region Commands
    // Event handler for save button click
    void OnSaveButtonClicked(object sender, EventArgs e)
    {
        // Read values from UI elements
        string notes = notesEntry.Text;
        string rating = ratingPicker.SelectedItem?.ToString();
        bool isLiked = likedSwitch.IsToggled;
        bool isBookmarked = bookmarkedSwitch.IsToggled;

        currentUserSong.SongNotes = notes;
        if(!rating.IsNullOrEmpty())
        {
            currentUserSong.SongRating= int.Parse(rating);
        }

        currentUserSong.SongLiked= isLiked;
        currentUserSong.SongBookmarked= isBookmarked;

        UserPerformedSong userPerformedSong = new UserPerformedSong();
        if (currentUserSong.UserPerformedSongId> 0)
        {
            userPerformedSong.ID = currentUserSong.UserPerformedSongId;
        }
        userPerformedSong.Bookmarked = currentUserSong.SongBookmarked;
        userPerformedSong.PerformedSongID = currentUserSong.PerformedSongId;
        userPerformedSong.Liked = currentUserSong.SongLiked;
        userPerformedSong.Rating = currentUserSong.SongRating;
        userPerformedSong.Notes = currentUserSong.SongNotes;
        userPerformedSong.Rating = currentUserSong.SongRating;
        userPerformedSong.UserID = 1;
        UserPerformedSongService userPerformedSongService= new UserPerformedSongService(connectionString);
        userPerformedSongService.SaveUserPerformedSong(userPerformedSong);
        DisplayAlert("Saved", $"Journal Details saved for {currentUserSong.SongName}.", "Done");
        LoadSongDetails(currentUserSong);
    }

    #endregion
}