using TheScoreBook.acessors;
using TheScoreBook.models.enums;
using TheScoreBook.models.round;
using TheScoreBook.views.home;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PastRoundsPage = TheScoreBook.views.pastRounds.PastRoundsPage;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace TheScoreBook
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // var r = new Round("bray 1");
            // r.AddScore(0, 0, EScore.SIX);
            // r.AddScore(0, 0, EScore.TEN);
            // r.AddScore(0, 0, EScore.X);
            //
            // UserData.Instance.SaveRound(r);
            
            MainPage = new NavigationPage(new PastRoundsPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}