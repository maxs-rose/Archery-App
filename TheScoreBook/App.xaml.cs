using FormsControls.Base;
using TheScoreBook.views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace TheScoreBook
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // ExperimentalFeatures.Enable("Shapes_Experimental");
            Device.SetFlags(new[] {"Shapes_Experimental"});

            // var r = new Round("test");
            // r.AddScore(0, 0, EScore.SIX);
            // r.AddScore(0, 0, EScore.TEN);
            // r.AddScore(0, 0, EScore.X);
            //
            // UserData.Instance.SaveRound(r);

            MainPage = new AnimationNavigationPage(new GeneralContainer());
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