using System.Windows.Input;
using ListeningCompanionDataService.Models.User;
using ListeningCompanionDataService.Models.View;
using Microsoft.IdentityModel.Tokens;

namespace ListeningCompanion;

public partial class PerformedSongDetailsView : ContentPage
{
    #region Fields 
    private const string connectionString = @"Server=tcp:listeningcompanion.database.windows.net,1433;Initial Catalog=ListeningCompanion;Persist Security Info=False;User ID=captaintrips;Password=TerrapinStation77!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    public RefreshView refreshView { get; private set; }
    public string viewMode { get; private set; } = "journal";
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

        refreshView = new RefreshView { Content = journalContentView };
        refreshView.Refreshing += OnRefreshing;
        //refreshView.BackgroundColor = Colors.White;
        Content = refreshView;
        Content = refreshView;
    }

    public ContentView LoadJournalView(UserSongDetails userSong)
    {
        // Create the content for the 'Journal' view
        var journalContent = new StackLayout
        {
            Orientation = StackOrientation.Vertical
        };


        var formGrid = new Grid
        {
            Margin = new Thickness(20), // Add margin for spacing around the grid
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Auto size for the label
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // Star size for the entry to expand
            }
        };
        int gridRowCount = 0;

        Label headerLabel = new Label
        {
            Text = $"{userSong.SongName}",
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10)
        };
        Grid.SetRow(headerLabel, gridRowCount++);
        formGrid.Children.Add(headerLabel);

        var notesLabel = new Label
        {
            Text = "Notes",
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5) // Add bottom margin for spacing between label and entry
        };

        // Text field for notes
        notesEntry = new Entry
        {
            Text = userSong.SongNotes,
            TextColor = Colors.Black,
            ReturnType = ReturnType.Done,
            FontSize = 16,
            BackgroundColor = Colors.LightGray, // Example background color
            HeightRequest = 40, // Adjust height as needed
            VerticalOptions = LayoutOptions.Center, // Center entry vertically
            Margin = new Thickness(0, 0, 0, 10) // Add bottom margin for spacing
        };
        //add to grid
        Grid.SetRow(notesLabel, gridRowCount++);
        formGrid.Children.Add(notesLabel);
        Grid.SetRow(notesEntry, gridRowCount++);
        formGrid.Children.Add(notesEntry);


        // Dropdown / wheel selection for rating
        var ratingLabel = new Label
        {
            Text = "Rating",
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5) // Add bottom margin for spacing between label and entry
        };
        ratingPicker = new Picker
        {
            BackgroundColor = Colors.LightGray, // Example background color
            TextColor = Colors.Black,
            HeightRequest = 40, // Adjust height as needed
            VerticalOptions = LayoutOptions.Center, // Center entry vertically
            Margin = new Thickness(0, 0, 0, 10) // Add bottom margin for spacing
        };
        for (int i = 1; i <= 10; i++)
        {
            ratingPicker.Items.Add(i.ToString());
        }
        ratingPicker.SelectedItem = userSong.SongRating.ToString();
        //add to grid
        Grid.SetRow(ratingLabel, gridRowCount++);
        formGrid.Children.Add(ratingLabel);
        Grid.SetRow(ratingPicker, gridRowCount++);
        formGrid.Children.Add(ratingPicker);


        // Toggle switches for Liked and Bookmarked
        var likedLabel = new Label
        {
            Text = "Liked",
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5) // Add bottom margin for spacing between label and entry
        };
        likedSwitch = new Switch
        {
            //BackgroundColor = Colors.LightGray, // Example background color
            HeightRequest = 40, // Adjust height as needed
            VerticalOptions = LayoutOptions.Center, // Center entry vertically
            Margin = new Thickness(0, 0, 0, 10) // Add bottom margin for spacing
        };

        likedSwitch.IsToggled = userSong.SongLiked;
        likedSwitch.OnColor = Colors.LightBlue;
        //add to grid
        Grid.SetRow(likedLabel, gridRowCount++);
        formGrid.Children.Add(likedLabel);
        Grid.SetRow(likedSwitch, gridRowCount++);
        formGrid.Children.Add(likedSwitch);



        var bookmarkedLabel = new Label
        {
            Text = "Bookmarked",
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5) // Add bottom margin for spacing between label and entry
        };
        bookmarkedSwitch = new Switch
        {
            //BackgroundColor = Colors.LightGray, // Example background color
            HeightRequest = 40, // Adjust height as needed
            VerticalOptions = LayoutOptions.Center, // Center entry vertically
            Margin = new Thickness(0, 0, 0, 10) // Add bottom margin for spacing
        };
        bookmarkedSwitch.IsToggled = userSong.SongBookmarked;
        bookmarkedSwitch.OnColor = Colors.MediumPurple;
        //add to grid
        Grid.SetRow(bookmarkedLabel, gridRowCount++);
        formGrid.Children.Add(bookmarkedLabel);
        Grid.SetRow(bookmarkedSwitch, gridRowCount++);
        formGrid.Children.Add(bookmarkedSwitch);

        // Dropdown for Interaction Status
        var statusLabel = new Label
        {
            Text = "Status",
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5) // Add bottom margin for spacing between label and entry
        };
        

        // Save button
        var saveButton = new Button
        {
            Text = "Save"
        };
        saveButton.Clicked += OnSaveButtonClicked;
        Grid.SetRow(saveButton, gridRowCount++);
        formGrid.Children.Add(saveButton);

        // Add elements to layout
        journalContent.Children.Add(GetFrameForView(formGrid));

        return new ContentView
        {
            Content = journalContent,
            IsVisible = viewMode.Contains("journal") // Start with 'Journal' content hidden
        };
    }

    private Frame GetFrameForView(View view)
    {
        return new Frame
        {
            Padding = new Thickness(10),
            BorderColor = Colors.White,
            Content = view
        };
    }
    #endregion
    #region Commands
    private void OnRefreshing(object sender, EventArgs e)
    {
        LoadSongDetails(currentUserSong);
        refreshView.IsRefreshing = false;
    }

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
        userPerformedSong.UserID = Session.Session.UserID;
        UserPerformedSongService userPerformedSongService= new UserPerformedSongService(connectionString);
        userPerformedSongService.SaveUserPerformedSong(userPerformedSong);
        DisplayAlert("Saved", $"Journal Details saved for {currentUserSong.SongName}.", "Done");
        //await Navigation.PushAsync(new ShowDetailsView(selectedItem));
        LoadSongDetails(currentUserSong);
    }

    #endregion
}