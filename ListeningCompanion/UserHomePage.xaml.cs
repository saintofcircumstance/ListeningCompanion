namespace ListeningCompanion;

public partial class UserHomePage : ContentPage
{
	public UserHomePage()
	{
		InitializeComponent();


        // Sample data - replace this with your actual dataset
        var tabs = new List<string>
            {
                "Home",
                "Today's Shows",
                "Search",
                "My Stuff",
                "Settings"
            };
    }
}