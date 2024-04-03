using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ListeningCompanion.SharedViews.TemplateSelector;
using ListeningCompanionDataService.Models.User;
using ListeningCompanionDataService.Models.View;


namespace ListeningCompanion.SharedViews.CustomView
{

    public class SongCollectionView
    {
        #region Fields
        private string _connectionString { get; set; }
        public ICommand BookmarkCommand { get; private set; }
        public ICommand LikeCommand { get; private set; }
        #endregion

        public SongCollectionView(string connectionString)
        {
            _connectionString = connectionString;
            BookmarkCommand = new Command(ExecuteBookmarkCommand);
            LikeCommand = new Command(ExecuteLikeCommand);
        }

        public CollectionView GetCollectionViewFromUserShowDetailsList(List<UserSongDetails> userSongDetails)
        {
            CollectionView songsCollectionView = new CollectionView();
            songsCollectionView.ItemsSource = userSongDetails;
            songsCollectionView.ItemTemplate = new SongDataTemplateSelector();
            var selector = (SongDataTemplateSelector)songsCollectionView.ItemTemplate;
            selector.NormalSongTemplate = GetBaseShowDataTemplate();
            selector.LikedSongTemplate = GetBaseShowDataTemplate(true, false);
            selector.BookmarkedSongTemplate = GetBaseShowDataTemplate(false, true);
            selector.BookmarkedAndLikedSongTemplate = GetBaseShowDataTemplate(true, true);

            songsCollectionView.SelectionMode = SelectionMode.Single;
            return songsCollectionView;

        }

        #region Get Data Template
        public DataTemplate GetBaseShowDataTemplate(bool liked = false, bool bookMarked = false)
        {
            return new DataTemplate(() =>
            {
                Grid grid = new Grid { Padding = 10 };
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });


                Label songNameLabel = new Label { FontAttributes = FontAttributes.Bold, TextColor = Colors.Black };
                songNameLabel.SetBinding(Label.TextProperty, "SongName");

                Label songNotesLabel = new Label { FontAttributes = FontAttributes.Italic, TextColor = Colors.Black, HorizontalOptions = LayoutOptions.Start};
                songNotesLabel.SetBinding(Label.TextProperty, "SongNotes");

                grid.Add(songNameLabel, 0, 0);
                grid.Add(songNotesLabel, 0, 1);

                if (liked || bookMarked)
                {
                    // Create a stack layout to hold the icons
                    var iconsLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.End };

                    if (liked)
                    {
                        Image likeImage = new Image { Aspect = Aspect.AspectFill, HeightRequest = 20, WidthRequest = 20 };
                        likeImage.Source = ImageSource.FromFile("star_big.png");
                        iconsLayout.Children.Add(likeImage); // Add the like image to the stack layout
                    }
                    if (bookMarked)
                    {
                        Image bookmarkImage = new Image { Aspect = Aspect.AspectFill, HeightRequest = 20, WidthRequest = 20 };
                        bookmarkImage.Source = ImageSource.FromFile("bookmark_added_48.png");
                        iconsLayout.Children.Add(bookmarkImage); // Add the bookmark image to the stack layout
                    }

                    // Add the icons layout to the grid
                    grid.Add(iconsLayout, 1, 0);
                }

                SwipeView swipeView = new SwipeView();
                SwipeItem bookmarkSwipeItem = new SwipeItem
                {
                    Text = bookMarked ? "Remove" : "Bookmark",
                    IconImageSource = bookMarked ? ImageSource.FromFile("bookmark_remove_48.png") : ImageSource.FromFile("bookmark_add_48.png"),
                    BackgroundColor = bookMarked ? Colors.PaleVioletRed : Colors.Blue,
                    Command = BookmarkCommand
                };
                //bookmarkSwipeItem.SetBinding(MenuItem.CommandProperty, new Binding("BindingContext.BookmarkCommand", source: showsCollectionView));
                bookmarkSwipeItem.SetBinding(MenuItem.CommandParameterProperty, ".");

                SwipeItem likeSwipeItem = new SwipeItem
                {
                    Text = liked ? "Remove" : "Like",
                    IconImageSource = liked ? ImageSource.FromFile("close_big.png") : ImageSource.FromFile("star_big.png"),
                    BackgroundColor = liked ? Colors.PaleVioletRed : Colors.LightBlue,
                    Command = LikeCommand
                };
                likeSwipeItem.SetBinding(MenuItem.CommandParameterProperty, ".");

                swipeView.RightItems = new SwipeItems { bookmarkSwipeItem, likeSwipeItem };

                swipeView.Content = grid;
                return swipeView;
            });
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
            UserPerformedSongService userPerformedSongService = new UserPerformedSongService(_connectionString);
            userPerformedSongService.SaveUserPerformedSong(userPerformedSong);
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
            UserPerformedSongService userPerformedSongService = new UserPerformedSongService(_connectionString);
            userPerformedSongService.SaveUserPerformedSong(userPerformedSong);

        }
        #endregion
    }


}
