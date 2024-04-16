using System.Windows.Input;
using ListeningCompanion.SharedViews.CustomView;
using ListeningCompanionDataService.Logic;
using ListeningCompanionDataService.Models.User;
using ListeningCompanionDataService.Models.View;
using Microsoft.IdentityModel.Tokens;

namespace ListeningCompanion;

public partial class ShowDetailsView : ContentPage
{
    #region Fields 
    private const string connectionString = @"Server=tcp:listeningcompanion.database.windows.net,1433;Initial Catalog=ListeningCompanion;Persist Security Info=False;User ID=captaintrips;Password=TerrapinStation77!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    public RefreshView refreshView { get; private set; }
    public string viewMode { get; private set; } = "setlist";
    public UserShowDetails currentUserShow { get; private set; }
    public List<UserSongDetails> userSongDetails { get; private set; }
    private ContentView setlistContentView;
    private ContentView journalContentView;

    // Define UI elements as class members for easy access
    public Entry notesEntry;
    public Picker ratingPicker;
    public Switch likedSwitch;
    public Switch bookmarkedSwitch;
    public Picker interactionStatusPicker;


    #endregion

    #region Constructors
    public ShowDetailsView(UserShowDetails userShow)
	{
        currentUserShow = userShow;
		InitializeComponent();
        LoadShowDetails(userShow);

		
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadShowDetails(currentUserShow);
        if (Session.Session.CurrentSong != null)
        {
            Title = Session.Session.CurrentSong.SongName;
        }
    }
    #endregion

    #region Load Views

    public async void LoadShowDetails(UserShowDetails userShow)
    {
        userSongDetails = await new ListeningCompanionDataService.Logic.ShowQueries(connectionString).GetSongsForShowAndUserId(userShow.ShowID, Session.Session.UserID);
        CollectionView songsCollectionView = new SongCollectionView(connectionString, () =>
        {
            LoadShowDetails(currentUserShow);
            refreshView.IsRefreshing = false;
        }).GetCollectionViewFromUserShowDetailsList(userSongDetails);
        // Define SelectionChanged event handler
        songsCollectionView.SelectionChanged += async (sender, e) =>
        {
            if (e.CurrentSelection.FirstOrDefault() is UserSongDetails selectedItem)
            {

                string action = await DisplayActionSheet(selectedItem.SongName, "Cancel", null, "Play", "Edit");
                if (action == "Play")
                {
                    if(selectedItem.Mp3Url.IsNullOrEmpty()){
                        await DisplayAlert("Error", "Source not found. Yell at Brian.", "Cancel");
                    }
                    else
                    {
                        Session.Session.AudioPlayer.Source = selectedItem.Mp3Url;

                        Session.Session.AudioPlayer.IsVisible = true;
                        Session.Session.AudioPlayer.ShouldShowPlaybackControls = false;
                        Session.Session.SongQueue = await new ShowQueries(connectionString).GetSongQueueForPerformedSong(Session.Session.UserID, selectedItem.ShowId, selectedItem.SetSequence, selectedItem.SongSequence);
                        Session.Session.AudioPlayer.MediaEnded += (sender, args) => { Session.Session.PlayNextSong(); };
                        Session.Session.CurrentSong = selectedItem;
                        Session.Session.AudioPlayer.Play();
                        new MediaHandler().OnElementChanged(selectedItem);
                    }
                    
                }
                else if (action == "Edit")
                {
                    await Navigation.PushAsync(new PerformedSongDetailsView(selectedItem, userShow));   
                }

            }
        };
        StackLayout stackLayout= new StackLayout
        {
            Margin = new Thickness(20),
            Children =
                {
                    songsCollectionView
                }
        };
        ScrollView scrollView = new ScrollView { Content= stackLayout };

        // Initialize ContentView for 'Songs' view
        setlistContentView = new ContentView
        {
            Content = scrollView,
            BackgroundColor = Colors.Transparent,
            IsVisible = viewMode.Contains("setlist")
        };


        // Initialize ContentView for 'Journal' view
        journalContentView = LoadJournalView(userShow);


        // Create buttons to switch between views
        var songsButton = new Button
        {
            Text = "Setlist",
            BackgroundColor = viewMode.Contains("setlist") ? Colors.Green : Colors.LightGray,
            ImageSource = ImageSource.FromFile("queue_music_24.png"),
            Padding = new Thickness(10),
            CornerRadius = 10
            
        };

        var journalButton = new Button
        {
            Text = " Journal",
            BackgroundColor = viewMode.Contains("journal") ? Colors.Green : Colors.LightGray,
            ImageSource = ImageSource.FromFile("book_24.png"),
            Padding = new Thickness(10),
            CornerRadius = 10,
            IsVisible = Session.Session.UserID > 0
        };

        // Handle button click events to toggle visibility of content views
        songsButton.Clicked += (sender, e) =>
        {
            viewMode = "setlist";
            songsButton.BackgroundColor = Colors.Green;
            journalButton.BackgroundColor = Colors.DarkGray;
            setlistContentView.IsVisible = true;
            journalContentView.IsVisible = false;
        };

        journalButton.Clicked += (sender, e) =>
        {
            viewMode = "journal";
            songsButton.BackgroundColor = Colors.DarkGray;
            journalButton.BackgroundColor = Colors.Green;
            setlistContentView.IsVisible = false;
            journalContentView.IsVisible = true;
        };

        Grid gridHeader = new Grid { Padding = 10 };
        gridHeader.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        gridHeader.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        gridHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        gridHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        gridHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        Label headerLabel = new Label
        {
            Text = $"{userShow.Date} at {userShow.VenueName}",
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10)
        };


        Grid.SetColumnSpan(headerLabel, 3);
        Grid.SetColumn(headerLabel, 0);
        gridHeader.Children.Add(headerLabel);
        gridHeader.Add(songsButton, 0, 1);
        gridHeader.Add(journalButton, 2,1);
        Frame headerFrame = new Frame
        {
            Content = gridHeader,
            BorderColor = Colors.Transparent,
            Background = Colors.Transparent,
            Padding = 10
        };


        StackLayout fullLayout = new StackLayout
        {
            Children =
            {
                headerFrame,
                setlistContentView,
                journalContentView
            }
        };

        ScrollView scrollViewFull = new ScrollView
        {
            Content = fullLayout
        };

        refreshView = new RefreshView { Content = scrollViewFull };
        refreshView.Refreshing += OnRefreshing;
        //refreshView.BackgroundColor = Colors.White;
        Content = refreshView;
    }



    public ContentView LoadJournalView(UserShowDetails userShow)
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
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // Star size for the entry to expand
            }
        };
        int gridRowCount = 0;


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
            Text = userShow.ShowNotes,
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
        ratingPicker.SelectedItem = userShow.ShowRating.ToString();
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
    
        likedSwitch.IsToggled = userShow.ShowLiked;
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
        bookmarkedSwitch.IsToggled = userShow.ShowBookMarked;
        bookmarkedSwitch.OnColor = Colors.MediumPurple;
        //add to grid
        Grid.SetRow(bookmarkedLabel, gridRowCount++);
        formGrid.Children.Add(bookmarkedLabel);
        Grid.SetRow(bookmarkedSwitch, gridRowCount++);
        formGrid.Children.Add(bookmarkedSwitch);

        // Dropdown for Interaction Status
        var statusLabel= new Label
        {
            Text = "Status",
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5) // Add bottom margin for spacing between label and entry
        };
        interactionStatusPicker = new Picker()
        {
            BackgroundColor = Colors.LightGray, // Example background color
            HeightRequest = 40, // Adjust height as needed
            VerticalOptions = LayoutOptions.Center, // Center entry vertically
            TextColor=Colors.Black,
            Margin = new Thickness(0, 0, 0, 10) // Add bottom margin for spacing
        };
        interactionStatusPicker.Items.Add("None");
        interactionStatusPicker.Items.Add("Listening");
        interactionStatusPicker.Items.Add("Listened");
        interactionStatusPicker.Items.Add("Attended");
        interactionStatusPicker.SelectedItem = userShow.InteractionStatus.IsNullOrEmpty() ? "None" : userShow.InteractionStatus;
        //add to grid
        Grid.SetRow(statusLabel, gridRowCount++);
        formGrid.Children.Add(statusLabel);
        Grid.SetRow(interactionStatusPicker, gridRowCount++);
        formGrid.Children.Add(interactionStatusPicker);

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
        LoadShowDetails(currentUserShow);
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
        string interactionStatus = interactionStatusPicker.SelectedItem?.ToString();

        currentUserShow.ShowNotes = notes;
        if(!rating.IsNullOrEmpty())
        {
            currentUserShow.ShowRating = int.Parse(rating);
        }
        
        currentUserShow.ShowLiked = isLiked;
        currentUserShow.ShowBookMarked = isBookmarked;
        currentUserShow.InteractionStatus = interactionStatus;

        UserShow userShow = new UserShow();
        if (currentUserShow.UserShowID > 0)
        {
            userShow.ID = currentUserShow.UserShowID;
        }
        userShow.BookMarked = currentUserShow.ShowBookMarked;
        userShow.ShowID = currentUserShow.ShowID;
        userShow.Liked = currentUserShow.ShowLiked;
        userShow.Rating = currentUserShow.ShowRating;
        userShow.InteractionStatus = currentUserShow.InteractionStatus;
        userShow.Notes = currentUserShow.ShowNotes;
        userShow.Rating = currentUserShow.ShowRating;
        userShow.UserID = Session.Session.UserID;
        UserShowService userShowService = new UserShowService(connectionString);
        userShowService.SaveUserShow(userShow);
        DisplayAlert("Saved", $"Journal Details saved for {currentUserShow.Date} at {currentUserShow.VenueName}.", "Done");
        LoadShowDetails(currentUserShow);
    }

    #endregion
}