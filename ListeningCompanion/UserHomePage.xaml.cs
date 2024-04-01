namespace ListeningCompanion;

public partial class UserHomePage : ContentPage
{
	public UserHomePage()
	{
		InitializeComponent();


        // Sample data - replace this with your actual dataset
        var shows = new List<string>
            {
                "Game of Thrones",
                "Breaking Bad",
                "Stranger Things",
                "The Crown",
                "The Mandalorian",
                "Friends",
                "The Office"
            };

        var listView = new ListView();
        listView.ItemsSource = shows;

        Content = new StackLayout
        {
            Margin = new Thickness(20),
            Children =
                {
                    new Label
                    {
                        Text = "List of Shows",
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    listView
                }
        };
    }
}