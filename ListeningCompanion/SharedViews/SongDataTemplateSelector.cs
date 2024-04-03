using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ListeningCompanionDataService.Models.View;

namespace ListeningCompanion.SharedViews
{
    
    public class SongDataTemplateSelector : DataTemplateSelector
    {
        #region Fields
        public DataTemplate NormalSongTemplate { get; set; }
        public DataTemplate BookmarkedSongTemplate { get; set; }
        public DataTemplate LikedSongTemplate { get; set; }
        public DataTemplate BookmarkedAndLikedSongTemplate { get; set;}
        #endregion


        #region OnSelectTempalte 
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is UserSongDetails currentItem)
            {
                DataTemplate selectedTemplate;

                if (currentItem.SongBookmarked && currentItem.SongLiked)
                {
                    selectedTemplate = CustomizeTemplate(BookmarkedAndLikedSongTemplate, true, true);
                }
                else if (currentItem.SongBookmarked && !currentItem.SongLiked)
                {
                    selectedTemplate = CustomizeTemplate(BookmarkedSongTemplate, true, false);
                }
                else if (!currentItem.SongBookmarked && currentItem.SongLiked)
                {
                    selectedTemplate = CustomizeTemplate(LikedSongTemplate, false, true);
                }
                else
                {
                    selectedTemplate = CustomizeTemplate(NormalSongTemplate);
                }

                return selectedTemplate;
            }

            return NormalSongTemplate;
        }

        #endregion

        #region Helper Functions
        private DataTemplate CustomizeTemplate(DataTemplate originalTemplate, bool bookmarked = false, bool liked = false)
        {
            // Create a copy of the original template
            var copy = new DataTemplate(() =>
            {
                // Create the frame with rounded corners
                Frame frame = new Frame
                {
                    CornerRadius = 10, // Adjust the corner radius as needed
                    Padding = new Thickness(1),
                    HasShadow = false, // Optional: Set to true if you want a drop shadow
                    BackgroundColor = Colors.LightGray
                };

                // Create the content of the template
                var content = (View)originalTemplate.CreateContent();

                if (bookmarked)
                {
                    frame.BackgroundColor = Colors.Blue;
                }
                else if (liked)
                {
                    frame.BackgroundColor = Colors.LightBlue;
                }
                
                // Add the original content to the frame
                frame.Content = content;

                return frame;
            });

            return copy;
        }

        #endregion

    }

}
