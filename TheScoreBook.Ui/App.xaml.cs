using System;
using FormsControls.Base;
using TheScoreBook.acessors;
using TheScoreBook.game;
using TheScoreBook.models.round;
using TheScoreBook.models.round.Structs;
using TheScoreBook.Ui;
using TheScoreBook.views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Style = TheScoreBook.models.enums.Style;

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
            Application.Current.UserAppTheme = Settings.AppTheme;

            var t = Services.Create<RoundData>("test");

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