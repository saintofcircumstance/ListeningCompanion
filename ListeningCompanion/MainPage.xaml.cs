
using ListeningCompanionAPIService.API;
using SQLite;
namespace ListeningCompanion
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count+= 10;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private async void OnTestApiClicked(object sender, EventArgs e)
        {
            var test = await GratefulStatsIntegration.TestApi();
            TestApiBtn.Text = test;
            SemanticScreenReader.Announce(TestApiBtn.Text);

        }

        //commenting out execution since these uploads are complete 
        private async void OnGetShowsClicked(object sender, EventArgs e)
        {
            //var test = await GratefulStatsIntegration.GetShows();
            Console.Write("complete");
        }

        private async void OnGetSongsClicked(object sender, EventArgs e)
        {
            //var test = await GratefulStatsIntegration.GetSongs();
            Console.Write("complete");
        }

        private async void OnGetSetsClicked(object sender, EventArgs e)
        {
            var test = await GratefulStatsIntegration.GetSets();
            Console.Write(test);
        }
        private async void OnGetVenuesClicked(object sender, EventArgs e)
        {
            var test = await GratefulStatsIntegration.GetVenues();
            Console.Write(test);
        }
    }

}
