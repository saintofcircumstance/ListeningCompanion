using System.Windows.Input;

using ListeningCompanion.SharedViews.CustomView;
using ListeningCompanionDataService.Models.View;
using Microsoft.IdentityModel.Tokens;

namespace ListeningCompanion;

public partial class CurrentDayShowView : ContentPage
{
    #region Fields 
    private const string connectionString = @"Server=tcp:listeningcompanion.database.windows.net,1433;Initial Catalog=ListeningCompanion;Persist Security Info=False;User ID=captaintrips;Password=TerrapinStation77!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    private DateTime viewDay { get ; set; } = DateTime.Now;
    public ScrollView scrollView {  get; set; }
    public RefreshView refreshView { get; set; }    
    public ICommand ShowSelectedCommand { get; private set; }
    #endregion

    #region Constructors
    public CurrentDayShowView()
	{
		InitializeComponent();
        LoadShowsCollectionView();
        ShowSelectedCommand = new Command(ExecuteShowSelectedCommand);
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadShowsCollectionView();
        if (Session.Session.CurrentSong != null)
        {
            Title = Session.Session.CurrentSong.SongName;
        }
    }

    #endregion

    #region Load Views
    public async void LoadShowsCollectionView()
    {
        //showsCollectionView.ItemsSource = await new ListeningCompanionDataService.Logic.ShowQueries(connectionString).GetUserShowDetails(1, 1, DateTime.Now, null, false, true, true, -1);
        var showList = await new ListeningCompanionDataService.Logic.ShowQueries(connectionString).GetUserShowDetails(1, Session.Session.UserID, viewDay, null, false, true, true, -1);
        var showsCollectionView = new ShowCollectionView(connectionString, () => {
            LoadShowsCollectionView();
            refreshView.IsRefreshing = false;
        }).GetCollectionViewFromUserShowDetailsList(showList);
        showsCollectionView.SelectionChanged += async (sender, e) =>
        {
            if (e.CurrentSelection.FirstOrDefault() is UserShowDetails selectedItem)
            {
                await Navigation.PushAsync(new ShowDetailsView(selectedItem));
            }
        };

        Grid gridHeader = new Grid { Padding = 10 };
        gridHeader.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        gridHeader.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        gridHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        gridHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        gridHeader.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        Label headerLabel = new Label
        {
            Text = $"Shows Performed on {viewDay.ToString("MMMM dd")}",
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 10)
        };
        Button btnNextDay = new Button
        {
            ImageSource = ImageSource.FromFile("arrow_fwd_24.png"),
            BackgroundColor = Colors.LightGray,
            HorizontalOptions = LayoutOptions.End,
            Margin = new Thickness(10, 0, 0, 0)
        };
        // Handle button click events to toggle visibility of content views
        btnNextDay.Clicked += (sender, e) =>
        {
            viewDay = viewDay.AddDays(1);
            LoadShowsCollectionView();
        };

        Button btnLastDay = new Button
        {
            ImageSource = ImageSource.FromFile("arrow_back_24.png"),
            BackgroundColor = Colors.LightGray,
            HorizontalOptions = LayoutOptions.Start,
            Margin = new Thickness(0,0,10,0)
        };
        // Handle button click events to toggle visibility of content views
        btnLastDay.Clicked += (sender, e) =>
        {
            viewDay = viewDay.AddDays(-1);
            LoadShowsCollectionView();
        };
        gridHeader.Add(btnLastDay, 0, 1);
        gridHeader.Add(headerLabel, 1, 0);
        gridHeader.Add(btnNextDay, 2, 1);

        Button buttonGoToDay = new Button
        {
            Text = "Search",
            ImageSource = ImageSource.FromFile("edit_calendar_24.png"),
            BackgroundColor = Colors.LightGray,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(5, 0, 5, 0)
        };
        // Handle button click events to toggle visibility of content views
        buttonGoToDay.Clicked += async (sender, e) =>
        {
            string day = await DisplayPromptAsync("Go to Day", "Enter as MM/DD or MMM/DD", "GO", "CANCEL", DateTime.Now.ToString("M"));
            if(DateTime.TryParse(day, out DateTime parsedDate))
            {
                viewDay = parsedDate;
            }
            else
            {
                if (!day.IsNullOrEmpty())
                {
                    await DisplayAlert("Error", "Invalid Date Input", "OK");
                }
                
            }
            
            LoadShowsCollectionView();
        };

        Button buttonGoToToday = new Button
        {
            Text = "Today",
            ImageSource = ImageSource.FromFile("today_24.png"),
            BackgroundColor = Colors.LightGray,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(5, 0, 5, 0)
        };
        buttonGoToToday.Clicked += async (sender, e) =>
        {
            viewDay = DateTime.Now;
            LoadShowsCollectionView();
        };


        //create grid to format buttons 
        Grid dateButtonGrid = new Grid ();
        dateButtonGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        dateButtonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        dateButtonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        dateButtonGrid.Add(buttonGoToDay, 0, 0);
        dateButtonGrid.Add(buttonGoToToday, 1,0);

        gridHeader.Add(dateButtonGrid, 1, 1);


        StackLayout stackLayout = new StackLayout
        {
            Margin = new Thickness(20),
            Children =
                {
                    gridHeader,
                    showsCollectionView
                }
        };

        scrollView = new ScrollView { Content = stackLayout };
        refreshView = new RefreshView { Content = scrollView };
        refreshView.Refreshing += OnRefreshing;
        //refreshView.BackgroundColor = Colors.White;
        Content = refreshView;
    }
    #endregion
    #region Commands
    private void OnRefreshing(object sender, EventArgs e)
    {
        LoadShowsCollectionView();
        refreshView.IsRefreshing = false;
    }

    private async void ExecuteShowSelectedCommand(object show)
    {
        var selectedShow = (UserShowDetails)show;
        await Navigation.PushAsync(new ShowDetailsView(selectedShow));
    }

    #endregion
}