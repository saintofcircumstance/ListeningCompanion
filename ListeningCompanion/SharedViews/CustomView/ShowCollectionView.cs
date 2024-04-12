﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public ICommand ListeningCommand { get; private set; }
        public ICommand ListenedCommand { get; private set; }

        private Action _refreshHandler;
        #endregion

        public ShowCollectionView(string connectionString, Action refreshHandler = null)
        {
            _connectionString = connectionString;
            _refreshHandler = refreshHandler;
            BookmarkCommand = new Command(ExecuteBookmarkCommand);
            LikeCommand = new Command(ExecuteLikeCommand);
            ListeningCommand = new Command(ExecuteListeningCommand);
            ListenedCommand= new Command(ExecuteListenedCommand);

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
            selector.NormalShowTemplateListened = GetBaseShowDataTemplate(false, false, "Listened");
            selector.LikedShowTemplateListened = GetBaseShowDataTemplate(true, false, "Listened");
            selector.BookmarkedShowTemplateListened = GetBaseShowDataTemplate(false, true, "Listened");
            selector.BookmarkedAndLikedShowTemplateListened = GetBaseShowDataTemplate(true, true, "Listened");
            selector.NormalShowTemplateListening = GetBaseShowDataTemplate(false, false, "Listening");
            selector.LikedShowTemplateListening = GetBaseShowDataTemplate(true, false, "Listening");
            selector.BookmarkedShowTemplateListening = GetBaseShowDataTemplate(false, true, "Listening");
            selector.BookmarkedAndLikedShowTemplateListening = GetBaseShowDataTemplate(true, true, "Listening");


            showsCollectionView.SelectionMode = SelectionMode.Single;
            return showsCollectionView;

        }

        #region Get Data Template
        public DataTemplate GetBaseShowDataTemplate(bool liked = false, bool bookMarked = false, string interactionStatus = "")
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
                        bookmarkImage.Source = ImageSource.FromFile("bookmark_added_48.png");
                        iconsLayout.Children.Add(bookmarkImage); // Add the bookmark image to the stack layout
                    }

                    // Add the icons layout to the grid
                    grid.Add(iconsLayout, 1, 1);
                }

                SwipeView swipeView = new SwipeView();
                SwipeItem bookmarkSwipeItem = new SwipeItem
                {
                    Text = bookMarked ? "Remove" : "Bookmark",
                    IconImageSource = bookMarked ? ImageSource.FromFile("bookmark_remove_48.png") : ImageSource.FromFile("bookmark_add_48.png"),
                    BackgroundColor = bookMarked ? Colors.PaleVioletRed : Colors.MediumPurple,
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


                SwipeItem listeningSwipeItem = new SwipeItem
                {
                    Text = interactionStatus.Contains("Listening") ? "Stop" : "Start",
                    IsVisible = !interactionStatus.Contains("Listening"),
                    IconImageSource = interactionStatus.Contains("Listening") ? ImageSource.FromFile("close_big.png") : ImageSource.FromFile("pending_48"),
                    BackgroundColor = interactionStatus.Contains("Listneing") ? Colors.PaleVioletRed : Colors.YellowGreen,
                    Command = ListeningCommand
                };
                listeningSwipeItem.SetBinding(MenuItem.CommandParameterProperty, ".");

                SwipeItem listenedSwipeItem = new SwipeItem
                {
                    Text = interactionStatus.Contains("Listened") ? "Active" : "Done",
                    IsVisible = !interactionStatus.Contains("Listened"),
                    IconImageSource = interactionStatus.Contains("Listened") ? ImageSource.FromFile("close_big.png") : ImageSource.FromFile("done_48"),
                    BackgroundColor = interactionStatus.Contains("Listneing") ? Colors.PaleVioletRed : Colors.Green,
                    Command = ListenedCommand
                };
                listenedSwipeItem.SetBinding(MenuItem.CommandParameterProperty, ".");


                swipeView.LeftItems = new SwipeItems { bookmarkSwipeItem, likeSwipeItem };
                swipeView.RightItems = new SwipeItems { listeningSwipeItem, listenedSwipeItem};


                swipeView.Content = grid;
                return swipeView;
            });
        }
        #endregion

        #region Commands
        private async void ExecuteListeningCommand(object show)
        {
            var selectedShow = (UserShowDetails)show;
            UserShow userShow = new UserShow();
            if (selectedShow.UserShowID > 0)
            {
                userShow.ID = selectedShow.UserShowID;
            }
            userShow.BookMarked = selectedShow.ShowBookMarked;
            userShow.ShowID = selectedShow.ShowID;
            userShow.Liked = selectedShow.ShowLiked;
            userShow.Rating = selectedShow.ShowRating;
            userShow.InteractionStatus = "Listening";
            userShow.Notes = selectedShow.ShowNotes;
            userShow.Rating = selectedShow.ShowRating;
            userShow.UserID = Session.Session.UserID;
            UserShowService userShowService = new UserShowService(_connectionString);
            userShowService.SaveUserShow(userShow);
            _refreshHandler?.Invoke();
        }
        private async void ExecuteListenedCommand(object show)
        {
            var selectedShow = (UserShowDetails)show;
            UserShow userShow = new UserShow();
            if (selectedShow.UserShowID > 0)
            {
                userShow.ID = selectedShow.UserShowID;
            }
            userShow.BookMarked = selectedShow.ShowBookMarked;
            userShow.ShowID = selectedShow.ShowID;
            userShow.Liked = selectedShow.ShowLiked;
            userShow.Rating = selectedShow.ShowRating;
            userShow.InteractionStatus = "Listened";
            userShow.Notes = selectedShow.ShowNotes;
            userShow.Rating = selectedShow.ShowRating;
            userShow.UserID = Session.Session.UserID;
            UserShowService userShowService = new UserShowService(_connectionString);
            userShowService.SaveUserShow(userShow);
            _refreshHandler?.Invoke();
        }

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
            userShow.UserID = Session.Session.UserID;
            UserShowService userShowService = new UserShowService(_connectionString);
            userShowService.SaveUserShow(userShow);
            _refreshHandler?.Invoke();

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
            userShow.UserID = Session.Session.UserID;
            UserShowService userShowService = new UserShowService(_connectionString);
            userShowService.SaveUserShow(userShow);
            _refreshHandler?.Invoke();

        }
        #endregion
    }


}
