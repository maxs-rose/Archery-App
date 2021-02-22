﻿using System;
using TheScoreBook.acessors;
using TheScoreBook.models;
using TheScoreBook.models.enums;
using TheScoreBook.models.round;
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

            MainPage = new MainPage();

            var r = new Round("portsmouth");
            r.AddScore(0, 0, EScore.SIX);
            r.AddScore(0, 0, EScore.TEN);
            r.AddScore(0, 0, EScore.ONE);
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