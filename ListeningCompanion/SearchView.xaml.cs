using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using ListeningCompanionDataService.Models.View;
using System.Xml;
using System.Windows.Input;
using Microsoft.Maui.Controls.Handlers.Items;
using ListeningCompanionDataService.Models.User;
using ListeningCompanion.SharedViews.CustomView;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Maui.Controls;


namespace ListeningCompanion;

public partial class SearchView : ContentPage
{
    #region Fields
    private const string connectionString = @"Server=tcp:listeningcompanion.database.windows.net,1433;Initial Catalog=ListeningCompanion;Persist Security Info=False;User ID=captaintrips;Password=TerrapinStation77!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    public ICommand ShowSelectedCommand { get; private set; }
    public List<UserShowDetails> showResults { get; private set; } = new List<UserShowDetails>();
    CollectionView showResultsView { get; set; }
    RefreshView refreshView { get; set; }
    Label showResultsLabel { get; set; }

    Entry venueSearchBar { get; set; }
    
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
        if (Session.Session.CurrentSong != null)
        {
            Title = Session.Session.CurrentSong.SongName;
        }
    }
    #endregion

    #region Load Views
    public async void LoadSearchPage(bool displayShowResults = false)
	{
        showResultsView = await LoadShowCollectionView();
        showResultsView.IsVisible = displayShowResults;
        showResultsLabel = new Label
        {
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5),
            IsVisible = displayShowResults
        };


        var searchGrid = new Grid
        {
            Margin = new Thickness(20), // Add margin for spacing around the grid
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Auto size for the label
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }, // Star size for the entry to expand
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
            }
        };
        int gridRowCount = 0;

        // Create filters UI elements
        //TODO: make this not hardcoded - fine for now 
        Label filterByDateLabel = new Label
        {
            Text = "Filter by Date",
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5)
        };
        startDatePicker = new DatePicker
        {
            MinimumDate = new DateTime(1965, 05, 05),
            MaximumDate = new DateTime(1995, 07, 09),
            BackgroundColor = Colors.LightGray, // Example background color
            HeightRequest = 40, // Adjust height as needed
            VerticalOptions = LayoutOptions.Center, // Center entry vertically
            TextColor = Colors.Black,
            Margin = new Thickness(0, 0, 0, 10) // Add bottom margin for spacing
        };
        endDatePicker = new DatePicker
        {
            MinimumDate = new DateTime(1965, 05, 05),
            MaximumDate = new DateTime(1995, 07, 09),
            BackgroundColor = Colors.LightGray,
            IsVisible= false,
            HorizontalOptions = LayoutOptions.Center,
            HeightRequest = 40, // Adjust height as needed
            VerticalOptions = LayoutOptions.Center, // Center entry vertically
            TextColor=Colors.Black,
            Margin = new Thickness(0, 1, 0, 10) // Add bottom margin for spacing
        };

        Grid.SetRow(filterByDateLabel, gridRowCount);
        Grid.SetColumnSpan(filterByDateLabel, 2);
        searchGrid.Children.Add(filterByDateLabel);
        Grid.SetRow(startDatePicker, gridRowCount);
        Grid.SetColumn(startDatePicker, 2);
        Grid.SetColumnSpan(startDatePicker, filterRange != null && filterRange.IsChecked ? 1 : 2);
        searchGrid.Children.Add(startDatePicker);
        Grid.SetRow(endDatePicker, gridRowCount++);
        Grid.SetColumn(endDatePicker, 3);
        Grid.SetColumnSpan(endDatePicker, 1);
        searchGrid.Children.Add(endDatePicker);


        Label filterDayLabel = new Label { 
            Text = "Day",
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5)
        };

        filterDay = new CheckBox
        {
            IsChecked = true,
            Margin = new Thickness(0, 0, 0, 5)
        };

        Label filterMonthLabel= new Label { 
            Text = "Month",
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5)
        };
        filterMonth= new CheckBox
        {
            IsChecked = true,
            Margin = new Thickness(0, 0, 0, 5)
        };

        Label filterYearLabel= new Label {
            Text = "Year",
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5)
        };
        filterYear= new CheckBox
        {
            IsChecked = true,
            Margin = new Thickness(0, 0, 0, 5)
        };

        Label filterRangeLabel = new Label {
            Text = "Date Range",
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5)
        };
        filterRange= new CheckBox
        {
            IsChecked = false,
            Margin = new Thickness(0, 0, 0, 5)
        };
        filterRange.CheckedChanged += OnFilterRangeChanged;

        var checkBoxLayout= new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Children = { filterYearLabel, filterYear, filterMonthLabel, filterMonth, filterDayLabel, filterDay, filterRangeLabel, filterRange}
        };

        Grid.SetColumnSpan(checkBoxLayout, 4);
        Grid.SetRow(checkBoxLayout, gridRowCount++);
        searchGrid.Children.Add(checkBoxLayout);


        //set up vendor filter
        Label filterByVenueLabel = new Label
        {
            Text = "Search by Venue",
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5)
        };
        venues = await new ListeningCompanionDataService.Logic.ShowQueries(connectionString).GetVenueListForBand(1);
        venueSearchBar = new Entry{ 
            Placeholder = "Search Venue",
            ClearButtonVisibility = ClearButtonVisibility.WhileEditing,
            ReturnType= ReturnType.Done,
            TextColor = Colors.Black,
            BackgroundColor = Colors.LightGray,
            VerticalOptions = LayoutOptions.Center, // Center entry vertically
            Margin = new Thickness(0, 0, 0, 10) // Add bottom margin for spacing
        };
        venueSearchBar.TextChanged += OnVenueSearchBarTextChanged;
        venueListView = new CollectionView
        {
            ItemsSource = venues,
            SelectionMode = SelectionMode.Single,
            IsVisible= false
            
        };
        venueListView.ItemTemplate = new DataTemplate(() =>
        {
            var nameLabel = new Label()
            {
                TextColor = Colors.Black
            };
            nameLabel.SetBinding(Label.TextProperty, "VenueDisplayName");

            return new StackLayout
            {
                BackgroundColor = Colors.LightGray,
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

        Grid.SetRow(filterByVenueLabel, gridRowCount);
        Grid.SetColumnSpan(filterByVenueLabel, 1);
        searchGrid.Children.Add(filterByVenueLabel);
        Grid.SetRow(venueSearchBar, gridRowCount++);
        Grid.SetColumn(venueSearchBar, 1);
        Grid.SetColumnSpan(venueSearchBar, 3);
        searchGrid.Children.Add(venueSearchBar);

        Grid.SetRow(venueListView, gridRowCount++);
        Grid.SetColumn(venueListView, 1);
        Grid.SetColumnSpan(venueListView, 3);
        searchGrid.Children.Add(venueListView);



        var searchButton = new Button {
            Text = "Search",
            Margin = new Thickness(0, 10)
        };
        searchButton.Clicked += OnSearchButtonClicked;

        Grid.SetRow(searchButton, gridRowCount++);
        Grid.SetColumnSpan(searchButton, 4);
        searchGrid.Children.Add(searchButton);

        // Create layout
        var layout = new StackLayout();
        layout.Children.Add(GetFrameForView(searchGrid));

        Frame resultsFrame = new Frame
        {
            Padding = new Thickness(10),
            BorderColor = Colors.Transparent,
            Content = new StackLayout
            {
                Children = { showResultsLabel, showResultsView }
            }
        };
        layout.Children.Add(resultsFrame);
        //layout.Children.Add(showResultsLabel);
        //layout.Children.Add(showResultsView);

        ScrollView scrollView = new ScrollView { Content = layout};
        refreshView = new RefreshView { Content = scrollView };
        refreshView.Refreshing += OnRefreshing;
        //refreshView.BackgroundColor = Colors.White;
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

    public async Task<CollectionView> LoadShowCollectionView()
    {
        // Define SelectionChanged event handler

        CollectionView showsCollectionView = new ShowCollectionView(connectionString, () => {
            LoadSearchPage(true);
            refreshView.IsRefreshing = false;
        }).GetCollectionViewFromUserShowDetailsList(showResults);
        showsCollectionView.SelectionChanged += async (sender, e) =>
        {
            if (e.CurrentSelection.FirstOrDefault() is UserShowDetails selectedItem)
            {
                await Navigation.PushAsync(new ShowDetailsView(selectedItem));
            }
        };
        showsCollectionView.Margin = new Thickness(20);
        return showsCollectionView;
    }
    #endregion

    #region Commands
    private void OnRefreshing(object sender, EventArgs e)
    {
        LoadSearchPage(true);
        refreshView.IsRefreshing = false;
    }
    public void OnFilterRangeChanged(object sender, CheckedChangedEventArgs e)
    {
        if (filterRange.IsChecked)
        {
            filterYear.IsChecked = true;
            Grid.SetColumnSpan(startDatePicker, 1);
            startDatePicker.Margin = new Thickness(0, 0, 1, 10);
            endDatePicker.IsVisible = true;
        }
        else
        {
            Grid.SetColumnSpan(startDatePicker, 2);
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
        showResults = await new ListeningCompanionDataService.Logic.ShowQueries(connectionString).GetUserShowDetails(1, Session.Session.UserID, selectedStartDate, selectedEndDate, filterYear.IsChecked, filterMonth.IsChecked, filterDay.IsChecked, selectedVenueId);
        showResultsView.ItemsSource = showResults;
        //showResultsView = await LoadShowCollectionView();

        showResultsView.IsVisible = true;
        showResultsLabel.Text = $"Showing {showResults.Count().ToString()} Results...";
        showResultsLabel.IsVisible = true;
    }
    #endregion

}