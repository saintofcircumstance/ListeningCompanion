using System.Windows.Input;
using ListeningCompanionDataService.Models.User;
using ListeningCompanionDataService.Models.View;
using Microsoft.IdentityModel.Tokens;

namespace ListeningCompanion;

public partial class ShowDetailsView : ContentPage
{
    #region Fields 
    private const string connectionString = @"Server=tcp:listeningcompanion.database.windows.net,1433;Initial Catalog=ListeningCompanion;Persist Security Info=False;User ID=captaintrips;Password=TerrapinStation77!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    public ICommand BookmarkCommand { get; private set; }
    public ICommand LikeCommand { get; private set; }
    public UserShowDetails currentUserShow { get; private set; }
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
        BookmarkCommand = new Command(ExecuteBookmarkCommand);
        LikeCommand = new Command(ExecuteLikeCommand);
		//LoadTodaysShows();

		
	}
    #endregion

    #region Load Views

    public async void LoadShowDetails(UserShowDetails userShow)
    {
        CollectionView songsCollectionView = new CollectionView();
        songsCollectionView.ItemsSource = await new ListeningCompanionDataService.Logic.ShowQueries(connectionString).GetSongsForShowAndUserId(userShow.ShowID, 1);
        //showsCollectionView.SetBinding(ItemsView.ItemsSourceProperty, "UserShowDetails");
        songsCollectionView.ItemTemplate = new DataTemplate(() =>
        {
            Grid grid = new Grid { Padding = 10 };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });


            Label songNameLabel = new Label { FontAttributes = FontAttributes.Bold };
            songNameLabel.SetBinding(Label.TextProperty, "SongName");
            
            grid.Add(songNameLabel, 0, 0);


            SwipeView swipeView = new SwipeView();
                SwipeItem bookmarkSwipeItem = new SwipeItem
                {
                IconImageSource = ImageSource.FromFile("bookmark_big.png"),
                BackgroundColor = Colors.Green,
                Command = BookmarkCommand
            };
            //bookmarkSwipeItem.SetBinding(MenuItem.CommandProperty, new Binding("BindingContext.BookmarkCommand", source: showsCollectionView));
            bookmarkSwipeItem.SetBinding(MenuItem.CommandParameterProperty, ".");



            SwipeItem likeSwipeItem = new SwipeItem
            {
                
                IconImageSource = ImageSource.FromFile("star_big.png"),
                BackgroundColor = Colors.GreenYellow,
                Command = LikeCommand
            };
            likeSwipeItem.SetBinding(MenuItem.CommandParameterProperty, ".");

            swipeView.RightItems = new SwipeItems { bookmarkSwipeItem, likeSwipeItem };

            
            swipeView.Content = grid;

            return swipeView;
        });

        songsCollectionView.SelectionMode = SelectionMode.Single;
        // Define SelectionChanged event handler
        songsCollectionView.SelectionChanged += async (sender, e) =>
        {
            if (e.CurrentSelection.FirstOrDefault() is UserSongDetails selectedItem)
            {
                // Execute your command or navigate to a new page here
                // For example, if you want to navigate to a new page:
                // await Navigation.PushAsync(new YourDetailPage(selectedItem));
                await Navigation.PushAsync(new PerformedSongDetailsView(selectedItem));
            }
        };

        StackLayout stackLayout= new StackLayout
        {
            Margin = new Thickness(20),
            Children =
                {
                    new Label
                    {
                        Text = "Show Details",
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    songsCollectionView
                }
        };
        ScrollView scrollView = new ScrollView { Content= stackLayout };

        


        // Initialize ContentView for 'Songs' view
        setlistContentView = new ContentView
        {
            Content = scrollView
        };


        // Initialize ContentView for 'Journal' view
        journalContentView = LoadJournalView(userShow);

        // Create buttons to switch between views
        var songsButton = new Button
        {
            Text = "Songs",
            BackgroundColor = Colors.LightGray
        };

        var journalButton = new Button
        {
            Text = "Journal",
            BackgroundColor = Colors.LightGray
        };

        // Handle button click events to toggle visibility of content views
        songsButton.Clicked += (sender, e) =>
        {
            setlistContentView.IsVisible = true;
            journalContentView.IsVisible = false;
        };

        journalButton.Clicked += (sender, e) =>
        {
            setlistContentView.IsVisible = false;
            journalContentView.IsVisible = true;
        };


        StackLayout fullLayout = new StackLayout
        {
            Children =
            {
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        songsButton,
                        journalButton
                    }
                },
                setlistContentView,
                journalContentView
            }
        };

        ScrollView scrollViewFull = new ScrollView
        {
            Content = fullLayout
        };
        Content = scrollViewFull;
    }

    public ContentView LoadJournalView(UserShowDetails userShow)
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
            Text = userShow.ShowNotes
        };

        // Dropdown / wheel selection for rating
        ratingPicker = new Picker();
        for (int i = 1; i <= 10; i++)
        {
            ratingPicker.Items.Add(i.ToString());
        }
        ratingPicker.SelectedItem = userShow.ShowRating.ToString();

        // Toggle switches for Liked and Bookmarked
        var likedLabel = new Label { Text = "Liked" };
        likedSwitch = new Switch();
        likedSwitch.IsToggled = userShow.ShowLiked;
        var bookmarkedLabel = new Label { Text = "Bookmarked" };
        bookmarkedSwitch = new Switch();
        bookmarkedSwitch.IsToggled = userShow.ShowBookMarked;

        // Dropdown for Interaction Status
        interactionStatusPicker = new Picker();
        interactionStatusPicker.Items.Add("None");
        interactionStatusPicker.Items.Add("Listening");
        interactionStatusPicker.Items.Add("Listened");
        interactionStatusPicker.Items.Add("Attended");
        interactionStatusPicker.SelectedItem = userShow.InteractionStatus.IsNullOrEmpty() ? "None" : userShow.InteractionStatus;

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
        journalContent.Children.Add(new Label { Text = "Interaction Status:" });
        journalContent.Children.Add(interactionStatusPicker);
        journalContent.Children.Add(saveButton);


        return new ContentView
        {
            Content = journalContent,
            IsVisible = false // Start with 'Journal' content hidden
        };
    }
    #endregion
    #region Commands
    private async void ExecuteBookmarkCommand(object song)
    {
        var selectedSong = (UserSongDetails)song;
        UserPerformedSong userPerformedSong= new UserPerformedSong();
        if(selectedSong.UserPerformedSongId> 0)
        {
            userPerformedSong.ID = selectedSong.UserPerformedSongId;
        }
        userPerformedSong.Bookmarked = !selectedSong.SongBookmarked;
        userPerformedSong.PerformedSongID = selectedSong.PerformedSongId;
        userPerformedSong.Liked = selectedSong.SongLiked;
        userPerformedSong.Rating = selectedSong.SongRating;
        userPerformedSong.Notes = selectedSong.SongNotes;
        userPerformedSong.Rating = selectedSong.SongRating;
        userPerformedSong.UserID = 1;
        UserPerformedSongService userPerformedSongService= new UserPerformedSongService(connectionString);
        userPerformedSongService.SaveUserPerformedSong(userPerformedSong);
        LoadShowDetails(currentUserShow);

    }
    private async void ExecuteLikeCommand(object song)
    {
        var selectedSong = (UserSongDetails)song;
        UserPerformedSong userPerformedSong = new UserPerformedSong();
        if (selectedSong.UserPerformedSongId > 0)
        {
            userPerformedSong.ID = selectedSong.UserPerformedSongId;
        }
        userPerformedSong.Bookmarked = selectedSong.SongBookmarked;
        userPerformedSong.PerformedSongID = selectedSong.PerformedSongId;
        userPerformedSong.Liked = !selectedSong.SongLiked;
        userPerformedSong.Rating = selectedSong.SongRating;
        userPerformedSong.Notes = selectedSong.SongNotes;
        userPerformedSong.Rating = selectedSong.SongRating;
        userPerformedSong.UserID = 1;
        UserPerformedSongService userPerformedSongService = new UserPerformedSongService(connectionString);
        userPerformedSongService.SaveUserPerformedSong(userPerformedSong);
        LoadShowDetails(currentUserShow);

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
        userShow.UserID = 1;
        UserShowService userShowService = new UserShowService(connectionString);
        userShowService.SaveUserShow(userShow);
        LoadShowDetails(currentUserShow);
    }

    #endregion
}