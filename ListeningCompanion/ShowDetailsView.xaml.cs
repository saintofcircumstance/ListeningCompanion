using System.Windows.Input;
using ListeningCompanionDataService.Models.User;
using ListeningCompanionDataService.Models.View;

namespace ListeningCompanion;

public partial class ShowDetailsView : ContentPage
{
    #region Fields 
    private const string connectionString = @"Server=tcp:listeningcompanion.database.windows.net,1433;Initial Catalog=ListeningCompanion;Persist Security Info=False;User ID=captaintrips;Password=TerrapinStation77!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    public ICommand BookmarkCommand { get; private set; }
    public ICommand LikeCommand { get; private set; }
    public UserShowDetails currentUserShow { get; private set; }
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
                Text = "Bookmark",
                IconImageSource = "Resources/bookmark.jpg",
                BackgroundColor = Colors.Green,
                Command = BookmarkCommand
            };
            //bookmarkSwipeItem.SetBinding(MenuItem.CommandProperty, new Binding("BindingContext.BookmarkCommand", source: showsCollectionView));
            bookmarkSwipeItem.SetBinding(MenuItem.CommandParameterProperty, ".");



            SwipeItem likeSwipeItem = new SwipeItem
            {
                Text = "Like",
                IconImageSource = "Resources/thumsup.jpg",
                BackgroundColor = Colors.GreenYellow,
                Command = LikeCommand
            };
            likeSwipeItem.SetBinding(MenuItem.CommandParameterProperty, ".");

            swipeView.RightItems = new SwipeItems { bookmarkSwipeItem, likeSwipeItem };

            
            swipeView.Content = grid;

            return swipeView;
        });

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
        Content = scrollView;
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

    #endregion
}