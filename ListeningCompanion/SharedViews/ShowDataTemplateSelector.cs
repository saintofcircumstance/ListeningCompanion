using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ListeningCompanionDataService.Models.View;

namespace ListeningCompanion.SharedViews
{
    
    public class ShowDataTemplateSelector : DataTemplateSelector
    {
        #region Fields
        public DataTemplate NormalShowTemplate { get; set; }
        public DataTemplate BookmarkedShowTemplate { get; set; }
        public DataTemplate LikedShowTemplate { get; set; }
        public DataTemplate BookmarkedAndLikedShowTemplate { get; set;}
        #endregion


        #region OnSelectTempalte 
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is UserShowDetails currentItem)
            {
                DataTemplate selectedTemplate;

                if (currentItem.ShowBookMarked && currentItem.ShowLiked)
                {
                    selectedTemplate = CustomizeTemplate(BookmarkedAndLikedShowTemplate, currentItem.InteractionStatus);
                }
                else if (currentItem.ShowBookMarked && !currentItem.ShowLiked)
                {
                    selectedTemplate = CustomizeTemplate(BookmarkedShowTemplate, currentItem.InteractionStatus);
                }
                else if (!currentItem.ShowBookMarked && currentItem.ShowLiked)
                {
                    selectedTemplate = CustomizeTemplate(LikedShowTemplate, currentItem.InteractionStatus);
                }
                else
                {
                    selectedTemplate = CustomizeTemplate(NormalShowTemplate, currentItem.InteractionStatus);
                }

                return selectedTemplate;
            }

            return NormalShowTemplate;
        }

        #endregion

        #region Helper Functions
        private DataTemplate CustomizeTemplate(DataTemplate originalTemplate, string interactionStatus)
        {
            // Create a copy of the original template
            var copy = new DataTemplate(() =>
            {
                // Create the frame with rounded corners
                Frame frame = new Frame
                {
                    CornerRadius = 10, // Adjust the corner radius as needed
                    Padding = new Thickness(5),
                    HasShadow = false // Optional: Set to true if you want a drop shadow
                };

                // Create the content of the template
                var content = (View)originalTemplate.CreateContent();

                // Customize the content based on InteractionStatus
                switch (interactionStatus)
                {
                    case "Listening":
                        frame.BackgroundColor = Colors.YellowGreen;
                        break;
                    case "Listened":
                        frame.BackgroundColor = Colors.Green;
                        break;
                    case "Attended":
                        frame.BackgroundColor = Colors.LightGreen;
                        break;
                    case "None":
                    default:
                        frame.BackgroundColor = Colors.LightGray;
                        // Handle None status if needed
                        break;
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
