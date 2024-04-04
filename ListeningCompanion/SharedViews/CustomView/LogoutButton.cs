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

    public class LogoutButton
    {
        #region Fields
        private string _connectionString { get; set; }
        public ICommand LogoutCommand { get; private set; }
        #endregion

        public LogoutButton(string connectionString)
        {
            _connectionString = connectionString;
            LogoutCommand = new Command(Logout);

        }

        public View GetLogoutButton()
        {
            return new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.End,
                Children =
                {
                    new Button
                    {
                        Text = "Logout",
                        Command = LogoutCommand,
                        BackgroundColor = Colors.LightGray,
                        ImageSource = ImageSource.FromFile("logout_24.png"),
                        HorizontalOptions = LayoutOptions.End
                    }
                }
            };
        }

    
        #region Commands
        private void Logout()
        {
            new ApplicationUserService(_connectionString).RemoveSavedDevice(Session.Session.UserID, new GetDeviceInfo().GetDeviceID());
            Session.Session.UserID = -1;
            Session.Session.Username = "";

        }
        #endregion
    }


}
