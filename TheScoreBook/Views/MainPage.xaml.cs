using System;
using TheScoreBook.Views;
using Xamarin.Forms;

namespace TheScoreBook
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void Lang_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ChangeLanguagePage());
        }
    }
}