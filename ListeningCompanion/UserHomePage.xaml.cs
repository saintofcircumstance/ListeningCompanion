namespace ListeningCompanion;

public partial class UserHomePage : ContentPage
{
	public UserHomePage()
	{
		InitializeComponent();
        LoadUserDetails();

    }

	public async void LoadUserDetails(int userID = 1)
	{
		Image homeImage = new Image
		{
			Source = ImageSource.FromFile("icon_big.png")
		};
		Content = homeImage;
	}
}