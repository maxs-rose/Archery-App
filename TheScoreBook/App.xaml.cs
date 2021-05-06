using FormsControls.Base;
using TheScoreBook.game;
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

            MainPage = new AnimationNavigationPage(new GeneralContainer());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            GameManager.SavePartlyFinishedRound();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}