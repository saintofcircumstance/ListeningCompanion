using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ListeningCompanionDataService.Models.View;

namespace ListeningCompanion.SharedViews
{
    
    public class ShowCollectionView
    {
        //I think this is a bad idea , may revisit later - 2APR24 brian

        public ShowCollectionView()
        {
            
        }

        //public CollectionView GetCollectionViewFromUserShowDetailsList(List<UserShowDetails> userShowDetailsList)
        //{
        //    CollectionView showsCollectionView = new CollectionView();
        //    showsCollectionView.ItemsSource = userShowDetailsList;

        //    showsCollectionView.ItemTemplate = new DataTemplate(() =>
        //    {
        //        Grid grid = new Grid { Padding = 10 };
        //        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        //        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        //        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        //        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });


        //        Label showDateLabel = new Label { FontAttributes = FontAttributes.Bold };
        //        showDateLabel.SetBinding(Label.TextProperty, "Date");

        //        Label venueLabel = new Label { FontAttributes = FontAttributes.Italic, VerticalOptions = LayoutOptions.End };
        //        venueLabel.SetBinding(Label.TextProperty, "VenueName");

        //        Label statusLabel = new Label { FontAttributes = FontAttributes.Italic, VerticalOptions = LayoutOptions.End };
        //        statusLabel.SetBinding(Label.TextProperty, "InteractionStatus");

        //        grid.Add(showDateLabel, 0, 0);
        //        grid.Add(venueLabel, 0, 1);
        //        grid.Add(statusLabel, 1, 0);


        //        SwipeView swipeView = new SwipeView();
        //        SwipeItem bookmarkSwipeItem = new SwipeItem
        //        {
        //            Text = "Bookmark",
        //            IconImageSource = ImageSource.FromFile("bookmark_big.png"),
        //            BackgroundColor = Colors.Green,
        //            Command = BookmarkCommand
        //        };
        //        //bookmarkSwipeItem.SetBinding(MenuItem.CommandProperty, new Binding("BindingContext.BookmarkCommand", source: showsCollectionView));
        //        bookmarkSwipeItem.SetBinding(MenuItem.CommandParameterProperty, ".");

        //        SwipeItem likeSwipeItem = new SwipeItem
        //        {
        //            Text = "Like",
        //            IconImageSource = ImageSource.FromFile("star_big.png"),
        //            BackgroundColor = Colors.GreenYellow,
        //            Command = LikeCommand
        //        };
        //        likeSwipeItem.SetBinding(MenuItem.CommandParameterProperty, ".");


        //        swipeView.RightItems = new SwipeItems { bookmarkSwipeItem, likeSwipeItem };



        //        swipeView.Content = grid;

        //        return swipeView;
        //    });
        //    showsCollectionView.SelectionMode = SelectionMode.Single;
        //    return showsCollectionView
        //}
    }


}
