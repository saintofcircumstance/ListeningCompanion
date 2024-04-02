using System.Windows.Input;
using ListeningCompanionDataService.Models.User;
using ListeningCompanionDataService.Models.View;
using Microsoft.Maui.Controls;

namespace ListeningCompanion;

public partial class CurrentDayShowView : ContentPage
{
    #region Fields 
    private const string connectionString = @"Server=tcp:listeningcompanion.database.windows.net,1433;Initial Catalog=ListeningCompanion;Persist Security Info=False;User ID=captaintrips;Password=TerrapinStation77!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    public ICommand BookmarkCommand { get; private set; }
    public ICommand LikeCommand { get; private set; }
    public ICommand ShowSelectedCommand { get; private set; }
    #endregion

    #region Constructors
    public CurrentDayShowView()
	{
		InitializeComponent();
        LoadShowsCollectionView();
        BookmarkCommand = new Command(ExecuteBookmarkCommand);
        LikeCommand = new Command(ExecuteLikeCommand);
        ShowSelectedCommand = new Command(ExecuteShowSelectedCommand);
		//LoadTodaysShows();

		
	}
    #endregion

    #region Load Views
    public async void LoadTodaysShows()
	{
        //user id hardcoded to 1 
        var currentDayShows = await new ListeningCompanionDataService.Logic.ShowQueries(connectionString).GetTodaysShows(1);

        var listView = new ListView();
        listView.ItemsSource = currentDayShows.Values;
        Content = new StackLayout
        {
            Margin = new Thickness(20),
            Children =
                {
                    new Label
                    {
                        Text = "List of Shows",
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    listView
                }
        };

    }

    public async void LoadShowsCollectionView()
    {
        CollectionView showsCollectionView = new CollectionView();
        showsCollectionView.ItemsSource = await new ListeningCompanionDataService.Logic.ShowQueries(connectionString).GetUserShowDetails(1, 1, DateTime.Now);
        //showsCollectionView.SetBinding(ItemsView.ItemsSourceProperty, "UserShowDetails");
        showsCollectionView.ItemTemplate = new DataTemplate(() =>
        {
            Grid grid = new Grid { Padding = 10 };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });


            Label showDateLabel= new Label { FontAttributes = FontAttributes.Bold };
            showDateLabel.SetBinding(Label.TextProperty, "Date");

            Label venueLabel = new Label { FontAttributes = FontAttributes.Italic, VerticalOptions = LayoutOptions.End };
            venueLabel.SetBinding(Label.TextProperty, "VenueName");

            Label statusLabel = new Label { FontAttributes = FontAttributes.Italic, VerticalOptions = LayoutOptions.End };
            statusLabel.SetBinding(Label.TextProperty, "InteractionStatus");
            
            grid.Add(showDateLabel, 0, 0);
            grid.Add(venueLabel, 0,1);
            grid.Add(statusLabel, 1, 0);


            SwipeView swipeView = new SwipeView();
            SwipeItem bookmarkSwipeItem = new SwipeItem
            {
                Text = "Bookmark",
                //IconImageSource = "Resources/bookmark.jpg",
                BackgroundColor = Colors.Green,
                Command = BookmarkCommand
            };
            //bookmarkSwipeItem.SetBinding(MenuItem.CommandProperty, new Binding("BindingContext.BookmarkCommand", source: showsCollectionView));
            bookmarkSwipeItem.SetBinding(MenuItem.CommandParameterProperty, ".");

            SwipeItem likeSwipeItem = new SwipeItem
            {
                Text = "Like",
                //IconImageSource = "Resources/thumsup.jpg",
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
                // Execute your command or navigate to a new page here
                // For example, if you want to navigate to a new page:
                // await Navigation.PushAsync(new YourDetailPage(selectedItem));
                await Navigation.PushAsync(new ShowDetailsView(selectedItem));
            }
        };

        StackLayout stackLayout = new StackLayout
        {
            Margin = new Thickness(20),
            Children =
                {
                    new Label
                    {
                        Text = "List of Shows",
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    showsCollectionView
                }
        };

        ScrollView scrollView = new ScrollView { Content = stackLayout };
        Content = scrollView;
    }
    #endregion
    #region Commands

    private async void ExecuteShowSelectedCommand(object show)
    {
        var selectedShow = (UserShowDetails)show;
        await Navigation.PushAsync(new ShowDetailsView(selectedShow));
    }
    private async void ExecuteBookmarkCommand(object show)
    {
        var selectedShow = (UserShowDetails)show;
        UserShow userShow = new UserShow();
        if(selectedShow.UserShowID > 0)
        {
            userShow.ID = selectedShow.UserShowID;
        }
        userShow.BookMarked = !selectedShow.ShowBookMarked;
        userShow.ShowID = selectedShow.ShowID;
        userShow.Liked = selectedShow.ShowLiked;
        userShow.Rating = selectedShow.ShowRating;
        userShow.InteractionStatus = selectedShow.InteractionStatus;
        userShow.Notes = selectedShow.ShowNotes;
        userShow.Rating = selectedShow.ShowRating;
        userShow.UserID = 1;
        UserShowService userShowService = new UserShowService(connectionString);
        userShowService.SaveUserShow(userShow);
        LoadShowsCollectionView();

    }
    private async void ExecuteLikeCommand(object show)
    {
        var selectedShow = (UserShowDetails)show;
        UserShow userShow = new UserShow();
        if (selectedShow.UserShowID > 0)
        {
            userShow.ID = selectedShow.UserShowID;
        }
        userShow.BookMarked = selectedShow.ShowBookMarked;
        userShow.ShowID = selectedShow.ShowID;
        userShow.Liked = !selectedShow.ShowLiked;
        userShow.Rating = selectedShow.ShowRating;
        userShow.InteractionStatus = selectedShow.InteractionStatus;
        userShow.Notes = selectedShow.ShowNotes;
        userShow.Rating = selectedShow.ShowRating;
        userShow.UserID = 1;
        UserShowService userShowService = new UserShowService(connectionString);
        userShowService.SaveUserShow(userShow);
        LoadShowsCollectionView();

    }

    #endregion
}