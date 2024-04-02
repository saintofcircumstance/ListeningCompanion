using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using ListeningCompanionDataService.Models.View;
using System.Xml;
using System.Windows.Input;
using Microsoft.Maui.Controls.Handlers.Items;
using ListeningCompanionDataService.Models.User;
using ListeningCompanion.SharedViews;
using static System.Net.Mime.MediaTypeNames;


namespace ListeningCompanion;

public partial class SearchView : ContentPage
{
    #region Fields
    private const string connectionString = @"Server=tcp:listeningcompanion.database.windows.net,1433;Initial Catalog=ListeningCompanion;Persist Security Info=False;User ID=captaintrips;Password=TerrapinStation77!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

    public ICommand BookmarkCommand { get; private set; }
    public ICommand LikeCommand { get; private set; }
    public ICommand ShowSelectedCommand { get; private set; }
    public List<UserShowDetails> showResults { get; private set; } = new List<UserShowDetails>();
    CollectionView showResultsView { get; set; }
    Label showResultsLabel { get; set; }

    SearchBar venueSearchBar { get; set; }
    
    //filters 
    DatePicker startDatePicker {  get; set; }   
    DatePicker endDatePicker { get; set; }

    CheckBox filterDay {  get; set; }
    CheckBox filterMonth { get; set; }
    CheckBox filterYear { get; set; }
    CheckBox filterRange { get; set; }

    CollectionView venueListView { get; set; }
    List<VenueDetails> venues { get; set; }
    string selectedVenueText { get; set; }
    int selectedVenueId { get; set; } = -1;
    #endregion

    #region Constructor
    public SearchView()
	{
		InitializeComponent();
        LoadSearchPage();
	}
    #endregion

    #region Load Views
    public async void LoadSearchPage(bool displayShowResults = false)
	{
        showResultsView = await LoadShowCollectionView();
        showResultsView.IsVisible = displayShowResults;
        showResultsLabel = new Label
        {
            IsVisible = false
        };
            
        

        // Create filters UI elements
        //TODO: make this not hardcoded - fine for now 
        startDatePicker = new DatePicker
        {
            MinimumDate = new DateTime(1965, 05, 05),
            MaximumDate = new DateTime(1995, 07, 09)
        };
        endDatePicker = new DatePicker
        {
            MinimumDate = new DateTime(1965, 05, 05),
            MaximumDate = new DateTime(1995, 07, 09),
            IsVisible= false
        };

        Label filterDayLabel = new Label { Text = "Day: " };
        filterDay = new CheckBox
        {
            IsChecked = true
        };

        Label filterMonthLabel= new Label { Text = "Month: " };
        filterMonth= new CheckBox
        {
            IsChecked = true
        };

        Label filterYearLabel= new Label { Text = "Year: " };
        filterYear= new CheckBox
        {
            IsChecked = true
        };

        Label filterRangeLabel = new Label { Text = "Date Range: " };
        filterRange= new CheckBox
        {
            IsChecked = false
        };
        filterRange.CheckedChanged += OnFilterRangeChanged;

        var checkBoxLayout= new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children = { filterYearLabel, filterYear, filterMonthLabel, filterMonth, filterDayLabel, filterDay, filterRangeLabel, filterRange}
        };


        //set up vendor filter
        venues = await new ListeningCompanionDataService.Logic.ShowQueries(connectionString).GetVenueListForBand(1);
        venueSearchBar = new SearchBar { Placeholder = "Search Venue" };
        venueSearchBar.TextChanged += OnVenueSearchBarTextChanged;
        venueListView = new CollectionView
        {
            ItemsSource = venues,
            SelectionMode = SelectionMode.Single,
            IsVisible= false
            
        };
        venueListView.ItemTemplate = new DataTemplate(() =>
        {
            var nameLabel = new Label();
            nameLabel.SetBinding(Label.TextProperty, "VenueDisplayName");

            return new StackLayout
            {
                Children = { nameLabel }
            };
        });

        // Define SelectionChanged event handler
        venueListView.SelectionChanged += async (sender, e) =>
        {
            if (e.CurrentSelection.FirstOrDefault() is VenueDetails selectedItem)
            {
                selectedVenueId = selectedItem.VenueID;
                selectedVenueText = selectedItem.VenueDisplayName;
                venueSearchBar.Text = selectedItem.VenueDisplayName;
            }
        };

        var searchButton = new Button { Text = "Search" };
        searchButton.Clicked += OnSearchButtonClicked;

        // Create layout
        var layout = new StackLayout();
        layout.Children.Add(new Label { Text = "Filter by Date:" });
        layout.Children.Add(startDatePicker);
        layout.Children.Add(endDatePicker);
        layout.Children.Add(checkBoxLayout);

        layout.Children.Add(new Label { Text = "Search by Venue:" });
        layout.Children.Add(venueSearchBar);
        layout.Children.Add(venueListView);
        layout.Children.Add(searchButton);
        layout.Children.Add(showResultsLabel);
        layout.Children.Add(showResultsView);

        Content = layout;
    }

    public async Task<CollectionView> LoadShowCollectionView()
    {
        CollectionView showsCollectionView = new CollectionView();
        showsCollectionView.ItemsSource = showResults;
        //showsCollectionView.SetBinding(ItemsView.ItemsSourceProperty, "UserShowDetails");
        showsCollectionView.ItemTemplate = new DataTemplate(() =>
        {
            Grid grid = new Grid { Padding = 10 };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });


            Label showDateLabel = new Label { FontAttributes = FontAttributes.Bold };
            showDateLabel.SetBinding(Label.TextProperty, "Date");

            Label venueLabel = new Label { FontAttributes = FontAttributes.Italic, VerticalOptions = LayoutOptions.End };
            venueLabel.SetBinding(Label.TextProperty, "VenueName");

            Label statusLabel = new Label { FontAttributes = FontAttributes.Italic, VerticalOptions = LayoutOptions.End };
            statusLabel.SetBinding(Label.TextProperty, "InteractionStatus");

            grid.Add(showDateLabel, 0, 0);
            grid.Add(venueLabel, 0, 1);
            grid.Add(statusLabel, 1, 0);


            SwipeView swipeView = new SwipeView();
            SwipeItem bookmarkSwipeItem = new SwipeItem
            {
                Text = "Bookmark",
                IconImageSource = ImageSource.FromFile("bookmark_big.png"),
                BackgroundColor = Colors.Green,
                Command = BookmarkCommand
            };
            //bookmarkSwipeItem.SetBinding(MenuItem.CommandProperty, new Binding("BindingContext.BookmarkCommand", source: showsCollectionView));
            bookmarkSwipeItem.SetBinding(MenuItem.CommandParameterProperty, ".");

            SwipeItem likeSwipeItem = new SwipeItem
            {
                Text = "Like",
                IconImageSource = ImageSource.FromFile("star_big.png"),
                BackgroundColor = Colors.GreenYellow,
                Command = LikeCommand
            };
            likeSwipeItem.SetBinding(MenuItem.CommandParameterProperty, ".");


            swipeView.RightItems = new SwipeItems { bookmarkSwipeItem, likeSwipeItem };



            swipeView.Content = grid;

            return swipeView;
        });
        showsCollectionView.SelectionMode = SelectionMode.Single;
        // Define SelectionChanged event handler
        showsCollectionView.SelectionChanged += async (sender, e) =>
        {
            if (e.CurrentSelection.FirstOrDefault() is UserShowDetails selectedItem)
            {
                await Navigation.PushAsync(new ShowDetailsView(selectedItem));
            }
        };
        return showsCollectionView;
    }
    #endregion

    #region Commands

    private async void ExecuteBookmarkCommand(object song)
    {
        var selectedSong = (UserSongDetails)song;
        UserPerformedSong userPerformedSong = new UserPerformedSong();
        if (selectedSong.UserPerformedSongId > 0)
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
        UserPerformedSongService userPerformedSongService = new UserPerformedSongService(connectionString);
        userPerformedSongService.SaveUserPerformedSong(userPerformedSong);
        LoadSearchPage(true);

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
        LoadSearchPage(true);

    }
    public void OnFilterRangeChanged(object sender, CheckedChangedEventArgs e)
    {
        if (filterRange.IsChecked)
        {
            filterYear.IsChecked = true;
            endDatePicker.IsVisible = true;
        }
        else
        {
            endDatePicker.IsVisible = false;
        }
    }
    public void OnVenueSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue;

        
        var filteredVenues = venues.Where(v => v.VenueDisplayName.ToLower().Contains(searchText.ToLower()));
        int i = filteredVenues.Count();
        // Update the items in the collection view
        venueListView.ItemsSource = filteredVenues;

        //hide result if not filtering
        venueListView.IsVisible = !searchText.IsNullOrEmpty() && searchText != selectedVenueText;
    }

    public async void OnSearchButtonClicked(object sender, EventArgs e)
    {
        // Get selected filters
        var selectedStartDate = startDatePicker.Date;

        DateTime? selectedEndDate = filterRange.IsChecked ? endDatePicker.Date : null;

        if (venueSearchBar.Text.IsNullOrEmpty()) { selectedVenueId = -1; }

        CollectionView showsCollectionView = new CollectionView();
        showResults = await new ListeningCompanionDataService.Logic.ShowQueries(connectionString).GetUserShowDetails(1, 1, selectedStartDate, selectedEndDate, filterYear.IsChecked, filterMonth.IsChecked, filterDay.IsChecked, selectedVenueId);
        showResultsView.ItemsSource = showResults;
        //showResultsView = await LoadShowCollectionView();

        showResultsView.IsVisible = true;
        showResultsLabel.Text = $"Showing {showResults.Count().ToString()} Results...";
        showResultsLabel.IsVisible = true;
    }
    #endregion

}