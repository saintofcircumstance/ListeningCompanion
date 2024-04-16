
using ListeningCompanionAPIService.API;
using ListeningCompanionDataService.Logic;
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
        public async void OnImportArchiveClick(object sender, EventArgs e)
        {
            var result = await new RelistenIntegration().GetSource();
            TestArchiveButton.Text = result.ToString();
            SemanticScreenReader.Announce(TestArchiveButton.Text);

        }

        public async void OnImportPerformedSongsClicked(object sender, EventArgs e)
        {
            var result = await new GratefulStatsImport().ImportPerformedSongs();
            TestPerformedSongsImportBtn.Text = result.ToString();
            SemanticScreenReader.Announce(TestPerformedSongsImportBtn.Text);
        }

        public async void OnImportSetsClicked(object sender, EventArgs e)
        {
            var result = await new GratefulStatsImport().ImportSets();
            TestSetsImportBtn.Text = result.ToString();
            SemanticScreenReader.Announce(TestSetsImportBtn.Text);
        }

        public async void OnImportSongsClicked(object sender, EventArgs e)
        {
            var result = await new GratefulStatsImport().ImportSongs();
            TestSongsImportBtn.Text = result;
            SemanticScreenReader.Announce(TestSongsImportBtn.Text);
        }

        public async void OnImportVenuesClicked(object sender, EventArgs e)
        {
            var result = await new GratefulStatsImport().ImportVenues();
            TestVenuesImportBtn.Text = result;
            SemanticScreenReader.Announce(TestVenuesImportBtn.Text);

        }
        public async void OnImportShowsClicked(object sender, EventArgs e)
        {
            var result = await new GratefulStatsImport().ImportShows();
            TestImportShowsBtn.Text = result;
            SemanticScreenReader.Announce(TestImportShowsBtn.Text);

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
