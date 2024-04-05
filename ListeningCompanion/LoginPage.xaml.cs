using System.Text;
using ListeningCompanionDataService.Models.User;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;

namespace ListeningCompanion;

public partial class LoginPage : ContentPage
{

    #region Fields
    private const string connectionString = @"Server=tcp:listeningcompanion.database.windows.net,1433;Initial Catalog=ListeningCompanion;Persist Security Info=False;User ID=captaintrips;Password=TerrapinStation77!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    public Entry usernameEntry { get; set; }
    public Entry emailEntry { get; set; }
    public Entry passwordEntry { get; set; }
    public Entry confirmPasswordEntry { get; set; }
    public string viewMode { get; set; } = "Login";

    #endregion
    public LoginPage()
    {
        InitializeComponent();
        LoadLoginView();
    }
    #region Load Views
    public async void LoadLoginView()
    {
        var formGrid = new Grid
        {
            Margin = new Thickness(20), // Add margin for spacing around the grid
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Auto size for the label
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) } // Star size for the entry to expand
            }
        };
        int gridRowCount = 0;


        var usernameLabel = new Label
        {
            Text = "Username",
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5) // Add bottom margin for spacing between label and entry
        };

        // Text field for notes
        usernameEntry = new Entry
        {
            Placeholder = "Username",
            TextColor = Colors.Black,
            ReturnType = ReturnType.Done,
            FontSize = 16,
            BackgroundColor = Colors.LightGray, // Example background color
            HeightRequest = 40, // Adjust height as needed
            VerticalOptions = LayoutOptions.Center, // Center entry vertically
            Margin = new Thickness(0, 0, 0, 10) // Add bottom margin for spacing
        };
        //add to grid
        Grid.SetRow(usernameLabel, gridRowCount++);
        formGrid.Children.Add(usernameLabel);
        Grid.SetRow(usernameEntry, gridRowCount++);
        formGrid.Children.Add(usernameEntry);

        //add email field
        var emailLabel = new Label
        {
            Text = "Email",
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5), // Add bottom margin for spacing between label and entry
            IsVisible = viewMode.Contains("CreateNew")
        };

        // Text field for notes
        emailEntry = new Entry
        {
            Placeholder = "Email",
            TextColor = Colors.Black,
            ReturnType = ReturnType.Done,
            FontSize = 16,
            BackgroundColor = Colors.LightGray, // Example background color
            HeightRequest = 40, // Adjust height as needed
            VerticalOptions = LayoutOptions.Center, // Center entry vertically
            Margin = new Thickness(0, 0, 0, 10), // Add bottom margin for spacing
            IsVisible = viewMode.Contains("CreateNew")
        };
        //add to grid
        Grid.SetRow(emailLabel, gridRowCount++);
        formGrid.Children.Add(emailLabel);
        Grid.SetRow(emailEntry, gridRowCount++);
        formGrid.Children.Add(emailEntry);



        var passwordLabel = new Label
        {
            Text = "Password",
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5) // Add bottom margin for spacing between label and entry
        };

        // Text field for notes
        passwordEntry = new Entry
        {
            Placeholder = "Password",
            TextColor = Colors.Black,
            IsPassword = true,
            ReturnType = ReturnType.Done,
            FontSize = 16,
            BackgroundColor = Colors.LightGray, // Example background color
            HeightRequest = 40, // Adjust height as needed
            VerticalOptions = LayoutOptions.Center, // Center entry vertically
            Margin = new Thickness(0, 0, 0, 10) // Add bottom margin for spacing
        };


        var confirmPasswordLabel = new Label
        {
            Text = "Confirm Password",
            FontSize = 16,
            TextColor = Colors.LightGray,
            Margin = new Thickness(0, 0, 0, 5),
            IsVisible = viewMode.Contains("CreateNew")
            
        };

        // Text field for notes
        confirmPasswordEntry = new Entry
        {
            Placeholder = "Confirm Password",
            TextColor = Colors.Black,
            IsPassword = true,
            ReturnType = ReturnType.Done,
            FontSize = 16,
            BackgroundColor = Colors.LightGray, // Example background color
            HeightRequest = 40, // Adjust height as needed
            VerticalOptions = LayoutOptions.Center, // Center entry vertically
            Margin = new Thickness(0, 0, 0, 10),
            IsVisible = viewMode.Contains("CreateNew")
        };
        //add to grid
        Grid.SetRow(confirmPasswordLabel, gridRowCount++);
        formGrid.Children.Add(confirmPasswordLabel);
        Grid.SetRow(confirmPasswordEntry, gridRowCount++);
        formGrid.Children.Add(confirmPasswordEntry);


        //add to grid
        Grid.SetRow(passwordLabel, gridRowCount++);
        formGrid.Children.Add(passwordLabel);
        Grid.SetRow(passwordEntry, gridRowCount++);
        formGrid.Children.Add(passwordEntry);

        var loginButton = new Button {
            Text = "Login",
            IsVisible = viewMode.Contains("Login"),
            Margin = new Thickness(0,0,0,10)
        };
        loginButton.Clicked += async (sender, e) =>
        {
            string username = usernameEntry.Text;
            string password = passwordEntry.Text;

            var user = await Authenticate();

            if (user.ID > 0)
            {
                new ApplicationUserService(connectionString).CreateSavedDevice(user.ID, new GetDeviceInfo().GetDeviceID());
                // Navigate to main page
                Session.Session.UserID = user.ID;
                Session.Session.Username = user.UserName;
                await Navigation.PushAsync(new UserHomePage());
                //await Shell.Current.GoToAsync($"//{nameof(UserHomePage)}");
            }
            else
            {
                // Display error message
                await DisplayAlert("Error", "Invalid username or password", "OK");
            }
        };

        var createNewButton = new Button { 
            Text = "Create Account" ,
            IsVisible = viewMode.Contains("Login")
        };
        createNewButton.Clicked += (sender, e) =>
        {
            viewMode = "CreateNew";
            LoadLoginView();
        };
        var loginNavButton = new Button
        {
            Text = "Login",
            IsVisible = viewMode.Contains("CreateNew")
        };
        loginNavButton.Clicked += (sender, e) =>
        {
            viewMode = "Login";
            LoadLoginView();
        };



        Grid.SetRow(loginButton, gridRowCount++);
        formGrid.Children.Add(loginButton);
        Grid.SetRow(createNewButton, gridRowCount++);
        formGrid.Children.Add(createNewButton);

        Grid.SetRow(loginNavButton, gridRowCount++);
        formGrid.Children.Add(loginNavButton);

        var saveNewButton = new Button
        {
            Text = "Save New User",
            IsVisible = viewMode.Contains("CreateNew")
        };
        saveNewButton.Clicked += async (sender, e) =>
        {
            await SaveUser();
            LoadLoginView();
        };
        Grid.SetRow(saveNewButton, gridRowCount++);
        formGrid.Children.Add(saveNewButton);


        Content = new StackLayout
        {
            Children = { GetFrameForView(formGrid) }
        };
    }

    private Frame GetFrameForView(View view)
    {
        return new Frame
        {
            Padding = new Thickness(10),
            BorderColor = Colors.White,
            Content = view
        };
    }
    #endregion
    #region Commands
    private async Task SaveUser()
    {
        ApplicationUserService applicationUserService = new ApplicationUserService(connectionString);
        StringBuilder alertSb = new StringBuilder();

        bool noErrors = true;
        string emailAddress = emailEntry.Text;
        string username = usernameEntry.Text;
        string password = passwordEntry.Text;   
        string confirmPassword = confirmPasswordEntry.Text;

        if (!IsValidEmail(emailEntry.Text)) { alertSb.AppendLine("Invalid Email"); noErrors = false; }
        if(applicationUserService.EmailExists(emailAddress)) { alertSb.AppendLine("Email already in use"); noErrors = false; }
        if (applicationUserService.UsernameExists(username)) { alertSb.AppendLine("Username already in use"); noErrors = false; }
        if (password != null && confirmPassword != null && !password.Equals(confirmPassword)) { alertSb.AppendLine("Passwords do not match"); noErrors = false; }
        if (username.IsNullOrEmpty()) { alertSb.AppendLine("Username cannot be blank"); noErrors =false; }
        if (emailAddress.IsNullOrEmpty()) { alertSb.AppendLine("Email Address cannot be blank"); noErrors = false; }
        if (password.IsNullOrEmpty()) { alertSb.AppendLine("Password cannot be blank"); noErrors = false; }

        if (!noErrors)
        {
            await DisplayAlert("Error", alertSb.ToString(), "OK");
            return ;
        }
        else
        {
            var newUser = new ApplicationUser();
            newUser.UserName = username;
            newUser.EmailAddress = emailAddress;
            newUser.UserPassword = password;
            applicationUserService.CreateApplicationUser(newUser);
            viewMode = "Login";
            await DisplayAlert("User created. You can now login.", alertSb.ToString(), "OK");
        }
    }

    private async Task<ApplicationUser> Authenticate()
    {
        string username = usernameEntry.Text;
        string password = passwordEntry.Text;
        return new ApplicationUserService(connectionString).AuthenticateUser(username, password);
    }
    #endregion

    #region Helper Functions
    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Regular expression pattern for validating email addresses
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        // Use Regex.IsMatch to check if the email matches the pattern
        return Regex.IsMatch(email, pattern);
    }
    #endregion
}