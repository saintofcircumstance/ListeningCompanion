
using ListeningCompanion.SharedViews.CustomView;
using ListeningCompanionDataService.Models.User;
using ListeningCompanionDataService.Models.View;
using Microsoft.Maui.Controls.Internals;

namespace ListeningCompanion;

public partial class UserHomePage : ContentPage
{

    #region Fields
    private const string connectionString = @"Server=tcp:listeningcompanion.database.windows.net,1433;Initial Catalog=ListeningCompanion;Persist Security Info=False;User ID=captaintrips;Password=TerrapinStation77!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    public string viewMode { get; set; } = "Standard";
    public CollectionView detailsCollectionView;
    public Label detailsLabel;
    public RefreshView refreshView { get; set; }
    public UserDetails currentUserDetails { get; set; }
    #endregion
    public UserHomePage()
	{
        //Session.Session.UserID = 1;
        var test = new GetDeviceInfo().GetDeviceID();
        InitializeComponent();
        LoadUserDetails();

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Remove previous page from navigation stack
        if (Navigation.NavigationStack.Count > 1)
        {
            var previousPage = Navigation.NavigationStack[Navigation.NavigationStack.Count - 2];
            if(previousPage != null && previousPage.GetType() == typeof(LoginPage))
            {
                Navigation.RemovePage(previousPage);
            }
            
        }
    }

    #region Load Views
    public async void LoadUserDetails()
	{
        if (Session.Session.UserID < 1)
        {
            ApplicationUser currentUser = new ApplicationUserService(connectionString).IsSavedDevice(new GetDeviceInfo().GetDeviceID());
            if(currentUser == null)
            {
                await Navigation.PushAsync(new LoginPage());
            }
            else
            {
                Session.Session.UserID = currentUser.ID;
                Session.Session.Username = currentUser.UserName;
            }
            
        }
        detailsCollectionView = new CollectionView()
        {
            IsVisible = viewMode.Contains("Standard")
        };

        //NavigationPage.SetTitleView(this, new LogoutButton(connectionString).GetLogoutTitleButton());

        

        detailsLabel = new Label()
        {
            Text = "",
            TextColor = Colors.LightGray,
            FontSize = 20,
            IsVisible = !viewMode.Contains("Standard"),
            Margin = new Thickness(0,0,0,10)
        };

        var headerLabel = new Label()
        {
            Text = $"Welcome {Session.Session.Username}!",
            TextColor = Colors.LightGray,
            FontSize = 30,
            Margin = new Thickness(0, 0, 0, 10),
            VerticalTextAlignment = TextAlignment.Start,
            HorizontalOptions = LayoutOptions.Start,
            HorizontalTextAlignment = TextAlignment.Start
        };

        var logoutButton = new LogoutButton(connectionString).GetLogoutButton();
        var headerLayout = new Grid
        {
            Margin = new Thickness(5), // Add margin for spacing around the grid
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }, // Star size for the entry to expand
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Star size for the entry to expand
            }
        };
        Grid.SetColumn(headerLabel, 0);
        headerLayout.Children.Add(headerLabel);
        Grid.SetColumn(logoutButton, 1);
        headerLayout.Children.Add(logoutButton);

        if (viewMode.Contains("ShowsListenedTo"))
        {
            var showList = currentUserDetails.UserShowDetails.Where(s => s.InteractionStatus.Contains("Listened")).ToList();
            detailsLabel.Text = $"{showList.Count().ToString()} Shows Listened to...";
            var showsCollectionView = new ShowCollectionView(connectionString, () => {
                LoadUserDetails();
                refreshView.IsRefreshing = false;
            }).GetCollectionViewFromUserShowDetailsList(showList);
            showsCollectionView.SelectionChanged += async (sender, e) =>
            {
                if (e.CurrentSelection.FirstOrDefault() is UserShowDetails selectedItem)
                {
                    await Navigation.PushAsync(new ShowDetailsView(selectedItem));
                }
            };
            detailsCollectionView = showsCollectionView;
        }
        else if (viewMode.Contains("ShowsInProgress"))
        {
            var showList = currentUserDetails.UserShowDetails.Where(s => s.InteractionStatus.Contains("Listening")).ToList();
            detailsLabel.Text = $"{showList.Count().ToString()} Shows In Progess...";
            var showsCollectionView = new ShowCollectionView(connectionString, () =>
            {
                LoadUserDetails();
                refreshView.IsRefreshing = false;
            }).GetCollectionViewFromUserShowDetailsList(showList);
            showsCollectionView.SelectionChanged += async (sender, e) =>
            {
                if (e.CurrentSelection.FirstOrDefault() is UserShowDetails selectedItem)
                {
                    await Navigation.PushAsync(new ShowDetailsView(selectedItem));
                }
            };
            detailsCollectionView = showsCollectionView;
        }
        else if (viewMode.Contains("ShowsLiked"))
        {
            var showList = currentUserDetails.UserShowDetails.Where(s => s.ShowLiked).ToList();
            detailsLabel.Text = $"{showList.Count().ToString()} Shows Liked...";
            var showsCollectionView = new ShowCollectionView(connectionString, () =>
            {
                LoadUserDetails();
                refreshView.IsRefreshing = false;
            }).GetCollectionViewFromUserShowDetailsList(showList);
            showsCollectionView.SelectionChanged += async (sender, e) =>
            {
                if (e.CurrentSelection.FirstOrDefault() is UserShowDetails selectedItem)
                {
                    await Navigation.PushAsync(new ShowDetailsView(selectedItem));
                }
            };
            detailsCollectionView = showsCollectionView;
        }
        else if (viewMode.Contains("ShowsBookmarked"))
        {
            var showList = currentUserDetails.UserShowDetails.Where(s => s.ShowBookMarked).ToList();
            detailsLabel.Text = $"{showList.Count().ToString()} Shows Bookmarked...";
            var showsCollectionView = new ShowCollectionView(connectionString, () =>
            {
                LoadUserDetails();
                refreshView.IsRefreshing = false;
            }).GetCollectionViewFromUserShowDetailsList(showList);
            showsCollectionView.SelectionChanged += async (sender, e) =>
            {
                if (e.CurrentSelection.FirstOrDefault() is UserShowDetails selectedItem)
                {
                    await Navigation.PushAsync(new ShowDetailsView(selectedItem));
                }
            };
            detailsCollectionView = showsCollectionView;
        }
        else if (viewMode.Contains("SongsLiked"))
        {
            var songList= currentUserDetails.UserSongDetails.Where(s => s.SongLiked).ToList();
            detailsLabel.Text = $"{songList.Count().ToString()} Songs Liked...";
            var songsCollectionView = new SongCollectionView(connectionString).GetCollectionViewFromUserShowDetailsList(songList);
            songsCollectionView.SelectionChanged += async (sender, e) =>
            {
                if (e.CurrentSelection.FirstOrDefault() is UserSongDetails selectedItem)
                {
                    var containingShow = currentUserDetails.UserShowDetails.Where(s => s.ShowID == selectedItem.ShowId).FirstOrDefault();
                    await Navigation.PushAsync(new ShowDetailsView(containingShow));
                }
            };
            detailsCollectionView = songsCollectionView;
        }
        else if (viewMode.Contains("SongsBookmarked"))
        {
            var songList = currentUserDetails.UserSongDetails.Where(s => s.SongBookmarked).ToList();
            detailsLabel.Text = $"{songList.Count().ToString()} Songs Bookmarked...";
            var songsCollectionView = new SongCollectionView(connectionString).GetCollectionViewFromUserShowDetailsList(songList);
            songsCollectionView.SelectionChanged += async (sender, e) =>
            {
                if (e.CurrentSelection.FirstOrDefault() is UserSongDetails selectedItem)
                {
                    var containingShow = currentUserDetails.UserShowDetails.Where(s => s.ShowID == selectedItem.ShowId).FirstOrDefault();
                    await Navigation.PushAsync(new ShowDetailsView(containingShow));
                    //await Navigation.PushAsync(new PerformedSongDetailsView(selectedItem));
                }
            };
            detailsCollectionView = songsCollectionView;
        }


        currentUserDetails = await new ListeningCompanionDataService.Logic.ShowQueries(connectionString).GetDetailsForUser(Session.Session.UserID);


		Grid showGrid = new Grid
        {
            Margin = new Thickness(5), // Add margin for spacing around the grid
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Auto size for the label
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }, // Star size for the entry to expand
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }, // Star size for the entry to expand
            }
        };
        showGrid.HorizontalOptions = LayoutOptions.Center;

        int showGridRowCount = 0;

        var showsLabel = new Label()
        {
            FontSize = 16,
            TextColor = Colors.LightGray,
            Text = "My Shows"
        };
        Grid.SetRow(showsLabel, showGridRowCount++);
        showGrid.Children.Add(showsLabel);


        //Shows listened to 
        var showListenedCountLabel = new Label
        {
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            TextColor = Colors.Black,
            Text = currentUserDetails.UserShowDetails.Where(s => s.InteractionStatus.Contains("Listened")).Count().ToString() ,
            HorizontalOptions = LayoutOptions.Center
        };
        var showListenedFrame = CreateTile("done_48.png", 10, "Listened", showListenedCountLabel);
        var showListenedTapGesture = new TapGestureRecognizer();
        showListenedTapGesture.Tapped += OnListenedShowsTapped;
        showListenedFrame.GestureRecognizers.Add(showListenedTapGesture);
        Grid.SetRow(showListenedFrame, showGridRowCount);
        Grid.SetColumn(showListenedFrame, 0);
        showGrid.Children.Add(showListenedFrame);


        //In Progress shows
        
        var showListeningCountLabel = new Label
        {
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            TextColor = Colors.Black,
            Text = currentUserDetails.UserShowDetails.Where(s => s.InteractionStatus.Contains("Listening")).Count().ToString(),
            HorizontalOptions = LayoutOptions.Center
        };
        var showListeningFrame = CreateTile("pending_48.png", 10, "Listening", showListeningCountLabel);
        var showInProgressTapGesture = new TapGestureRecognizer();
        showInProgressTapGesture.Tapped += OnInProgressShowsTapped;
        showListeningFrame.GestureRecognizers.Add(showInProgressTapGesture);
        Grid.SetRow(showListeningFrame, showGridRowCount++);
        Grid.SetColumn(showListeningFrame, 1);
        showGrid.Children.Add(showListeningFrame);

        //Liked shows
        var showLikedLabel = new Label
        {
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            TextColor = Colors.Black,
            Text = currentUserDetails.UserShowDetails.Where(s => s.ShowLiked).Count().ToString(),
            HorizontalOptions = LayoutOptions.Center
        };
        var showLikedFrame= CreateTile("star_big.png", 10, "Liked", showLikedLabel);
        var showLikedTapGesture= new TapGestureRecognizer();
        showLikedTapGesture.Tapped += OnLikedShowsTapped;
        showLikedFrame.GestureRecognizers.Add(showLikedTapGesture);
        Grid.SetRow(showLikedFrame, showGridRowCount);
        Grid.SetColumn(showLikedFrame, 0);
        showGrid.Children.Add(showLikedFrame);

        //Bookmarked shows
        var showBookmarkedLabel = new Label
        {
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            TextColor = Colors.Black,
            Text = currentUserDetails.UserShowDetails.Where(s => s.ShowBookMarked).Count().ToString(),
            HorizontalOptions = LayoutOptions.Center
        };
        var showBookmarkedFrame = CreateTile("bookmark_big.png", 10, "Bookmarked", showBookmarkedLabel);
        var showBookmarkedTapGesture = new TapGestureRecognizer();
        showBookmarkedTapGesture.Tapped += OnBookmarkedShowsTapped;
        showBookmarkedFrame.GestureRecognizers.Add(showBookmarkedTapGesture);
        Grid.SetRow(showBookmarkedFrame, showGridRowCount++);
        Grid.SetColumn(showBookmarkedFrame, 1);
        showGrid.Children.Add(showBookmarkedFrame);


        Grid songGrid = new Grid
        {
            Margin = new Thickness(5), // Add margin for spacing around the grid
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Auto size for the label
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto}
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
            }
        };
        songGrid.HorizontalOptions = LayoutOptions.Center;

        int songGridRowCount = 0;

        var songsLabel = new Label()
        {
            FontSize = 16,
            TextColor = Colors.LightGray,
            Text = "My Songs"
        };
        Grid.SetRow(songsLabel, songGridRowCount++);
        songGrid.Children.Add(songsLabel);

        //Liked Songs
        var songLikedLabel = new Label
        {
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            TextColor = Colors.Black,
            Text = currentUserDetails.UserSongDetails.Where(s => s.SongLiked).Count().ToString(),
            HorizontalOptions = LayoutOptions.Center
        };
        var songLikedFrame = CreateTile("star_big.png", 10, "Liked", songLikedLabel);
        var songLikedTapGesture = new TapGestureRecognizer();
        songLikedTapGesture.Tapped += OnLikedSongsTapped;
        songLikedFrame.GestureRecognizers.Add(songLikedTapGesture);
        Grid.SetRow(songLikedFrame, songGridRowCount++);
        Grid.SetColumn(songLikedFrame, 0);
        songGrid.Children.Add(songLikedFrame);

        //Bookmarked Songs
        var songBookmarkedLabel = new Label
        {
            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            TextColor = Colors.Black,
            Text = currentUserDetails.UserSongDetails.Where(s => s.SongBookmarked).Count().ToString(),
            HorizontalOptions = LayoutOptions.Center
        };
        var songBookmarkedFrame = CreateTile("bookmark_big.png", 10, "Bookmarked", songBookmarkedLabel);
        var songBookmarkedTapGesture = new TapGestureRecognizer();
        songBookmarkedTapGesture.Tapped += OnBookmarkedSongsTapped;
        songBookmarkedFrame.GestureRecognizers.Add(songBookmarkedTapGesture);
        Grid.SetRow(songBookmarkedFrame, songGridRowCount++);
        Grid.SetColumn(songBookmarkedFrame, 0);
        songGrid.Children.Add(songBookmarkedFrame);


        Image homeImage = new Image
		{
            WidthRequest = showGrid.Width, 
            HeightRequest = showGrid.Height,
			Source = ImageSource.FromFile("appicon_big.png"),
            IsVisible = viewMode.Contains("Standard")
        };

        Grid tilesGrid = new Grid
        {
            Margin = new Thickness(5), // Add margin for spacing around the grid
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Auto size for the label
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Star size for the entry to expand
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            }
        };

        var detailsLayout = new StackLayout()
        {
            Orientation = StackOrientation.Vertical,
            Children = { detailsLabel, detailsCollectionView}
        };
        var detailsFrame = GetFrameForView(detailsLayout);
        detailsFrame.IsVisible = !viewMode.Contains("Standard");

        var showFrame = GetFrameForView(showGrid);
        Grid.SetColumn(showFrame, 0);
        Grid.SetColumnSpan(showFrame, 2);
        tilesGrid.Children.Add(showFrame);
        var songFrame = GetFrameForView(songGrid);
        Grid.SetColumn(songFrame, 2);
        tilesGrid.Children.Add(songFrame);

        ScrollView scrollView = new ScrollView()
        {
            Content = new StackLayout()
            {
                Children = { GetFrameForView(headerLayout), tilesGrid , detailsFrame ,homeImage }
            }
        };
        refreshView = new RefreshView
        {
            Content = scrollView
        };
        refreshView.Refreshing += OnRefreshing;
        Content = refreshView;
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
        LoadUserDetails();
        refreshView.IsRefreshing = false;
    }
    private async void OnListenedShowsTapped(object sender, EventArgs e)
    {
        viewMode = "ShowsListenedTo";
        LoadUserDetails();
        
    }
    private async void OnInProgressShowsTapped(object sender, EventArgs e)
    {
        viewMode = "ShowsInProgress";
        LoadUserDetails();
    }
    private async void OnLikedShowsTapped(object sender, EventArgs e)
    {
        viewMode = "ShowsLiked";
        LoadUserDetails();
    }
    private async void OnBookmarkedShowsTapped(object sender, EventArgs e)
    {
        viewMode = "ShowsBookmarked";
        LoadUserDetails();
    }
    private async void OnLikedSongsTapped(object sender, EventArgs e)
    {
        viewMode = "SongsLiked";
        LoadUserDetails();
    }
    private async void OnBookmarkedSongsTapped(object sender, EventArgs e)
    {
        viewMode = "SongsBookmarked";
        LoadUserDetails();
    }

    #endregion
    #region Helper Functions
    // Method to create a tile frame
    private Frame CreateTile(string fileName, int cornerRadius, string colorType, Label tileCountLabel)
    {
        Color backgroundColor = Colors.White;
        switch (colorType)
        {
            case "Listened":
                backgroundColor = Colors.Green;
                break;
            case "Listening":
                backgroundColor = Colors.GreenYellow;
                break;
            case "Liked":
                backgroundColor = Colors.LightBlue;
                break;
            case "Bookmarked":
                backgroundColor = Colors.MediumPurple;
                break;
            default:
                break;
        }
        var frame =  new Frame
        {
            CornerRadius = cornerRadius,
            BackgroundColor = backgroundColor,
            Padding = new Thickness(10),
            Margin = new Thickness(10),
            WidthRequest = 100
        };
        frame.Content = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center,
            Children =  { tileCountLabel,  new Image { Source = ImageSource.FromFile(fileName), HeightRequest = 20, WidthRequest = 20 }  }
        };
        return frame;
    }
    #endregion
}