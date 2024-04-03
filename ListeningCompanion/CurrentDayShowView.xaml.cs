using System.Windows.Input;

using ListeningCompanion.SharedViews;
using ListeningCompanionDataService.Models.View;

namespace ListeningCompanion;

public partial class CurrentDayShowView : ContentPage
{
    #region Fields 
    private const string connectionString = @"Server=tcp:listeningcompanion.database.windows.net,1433;Initial Catalog=ListeningCompanion;Persist Security Info=False;User ID=captaintrips;Password=TerrapinStation77!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

    public ScrollView scrollView {  get; set; }
    public RefreshView refreshView { get; set; }    
    public ICommand ShowSelectedCommand { get; private set; }
    #endregion

    #region Constructors
    public CurrentDayShowView()
	{
		InitializeComponent();
        LoadShowsCollectionView();
        ShowSelectedCommand = new Command(ExecuteShowSelectedCommand);

		
	}
    #endregion

    #region Load Views
    public async void LoadShowsCollectionView()
    {
        //showsCollectionView.ItemsSource = await new ListeningCompanionDataService.Logic.ShowQueries(connectionString).GetUserShowDetails(1, 1, DateTime.Now, null, false, true, true, -1);
        var showList = await new ListeningCompanionDataService.Logic.ShowQueries(connectionString).GetUserShowDetails(1, 1, DateTime.Now, null, false, true, true, -1);
        var showsCollectionView = new ShowCollectionView(connectionString).GetCollectionViewFromUserShowDetailsList(showList);
        showsCollectionView.SelectionChanged += async (sender, e) =>
        {
            if (e.CurrentSelection.FirstOrDefault() is UserShowDetails selectedItem)
            {
                await Navigation.PushAsync(new ShowDetailsView(selectedItem));
            }
        };

        StackLayout stackLayout = new StackLayout
        {
            Margin = new Thickness(20),
            Children =
                {
                    new Label
                    {
                        Text = $"Shows Performed on {DateTime.Now.ToString("MMMM dd")}",
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    showsCollectionView
                }
        };

        scrollView = new ScrollView { Content = stackLayout };
        refreshView = new RefreshView { Content = scrollView };
        refreshView.Refreshing += OnRefreshing;
        refreshView.BackgroundColor = Colors.White;
        Content = refreshView;
    }
    #endregion
    #region Commands
    private void OnRefreshing(object sender, EventArgs e)
    {
        LoadShowsCollectionView();
        refreshView.IsRefreshing = false;
    }

    private async void ExecuteShowSelectedCommand(object show)
    {
        var selectedShow = (UserShowDetails)show;
        await Navigation.PushAsync(new ShowDetailsView(selectedShow));
    }

    #endregion
}