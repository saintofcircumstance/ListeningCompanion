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

    public class ShowCollectionView
    {
        #region Fields
        private string _connectionString { get; set; }
        public ICommand BookmarkCommand { get; private set; }
        public ICommand LikeCommand { get; private set; }
        #endregion

        public ShowCollectionView(string connectionString)
        {
            _connectionString = connectionString;
            BookmarkCommand = new Command(ExecuteBookmarkCommand);
            LikeCommand = new Command(ExecuteLikeCommand);
        }

        public CollectionView GetCollectionViewFromUserShowDetailsList(List<UserShowDetails> userShowDetailsList)
        {
            CollectionView showsCollectionView = new CollectionView();
            showsCollectionView.ItemsSource = userShowDetailsList;
            showsCollectionView.ItemTemplate = new ShowDataTemplateSelector();
            var selector = (ShowDataTemplateSelector)showsCollectionView.ItemTemplate;
            selector.NormalShowTemplate = GetBaseShowDataTemplate();
            selector.LikedShowTemplate = GetBaseShowDataTemplate(true, false);
            selector.BookmarkedShowTemplate = GetBaseShowDataTemplate(false, true);
            selector.BookmarkedAndLikedShowTemplate = GetBaseShowDataTemplate(true, true);

            showsCollectionView.SelectionMode = SelectionMode.Single;
            return showsCollectionView;

        }

        #region Get Data Template
        public DataTemplate GetBaseShowDataTemplate(bool liked = false, bool bookMarked = false)
        {
            return new DataTemplate(() =>
            {
                Grid grid = new Grid { Padding = 10 };
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star});
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });


                Label showDateLabel = new Label { FontAttributes = FontAttributes.Bold, TextColor = Colors.Black };
                showDateLabel.SetBinding(Label.TextProperty, "Date");

                Label venueLabel = new Label { FontAttributes = FontAttributes.Italic, TextColor = Colors.Black, VerticalOptions = LayoutOptions.End };
                venueLabel.SetBinding(Label.TextProperty, "VenueName");

                Label statusLabel = new Label { FontAttributes = FontAttributes.Italic, TextColor = Colors.Black, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.End };
                statusLabel.SetBinding(Label.TextProperty, "InteractionStatus");

                grid.Add(showDateLabel, 0, 0);
                grid.Add(venueLabel, 0, 1);
                grid.Add(statusLabel, 1, 0);

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
                        bookmarkImage.Source = ImageSource.FromFile("bookmark_big.png");
                        iconsLayout.Children.Add(bookmarkImage); // Add the bookmark image to the stack layout
                    }

                    // Add the icons layout to the grid
                    grid.Add(iconsLayout, 1, 1);
                }

                SwipeView swipeView = new SwipeView();
                SwipeItem bookmarkSwipeItem = new SwipeItem
                {
                    Text = bookMarked ? "Remove" : "Bookmark",
                    IconImageSource = bookMarked ? ImageSource.FromFile("close_big.png") : ImageSource.FromFile("bookmark_big.png"),
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
        private async void ExecuteBookmarkCommand(object show)
        {
            var selectedShow = (UserShowDetails)show;
            UserShow userShow = new UserShow();
            if (selectedShow.UserShowID > 0)
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
            UserShowService userShowService = new UserShowService(_connectionString);
            userShowService.SaveUserShow(userShow);


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
            UserShowService userShowService = new UserShowService(_connectionString);
            userShowService.SaveUserShow(userShow);


        }
        #endregion
    }


}
