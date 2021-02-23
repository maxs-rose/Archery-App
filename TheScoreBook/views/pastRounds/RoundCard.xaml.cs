﻿using TheScoreBook.localisation;
using TheScoreBook.models.enums;
using TheScoreBook.models.round;
using Xamarin.Forms;

namespace TheScoreBook.views.pastRounds
{
    public partial class RoundCard : Frame
    {
        private readonly Round round;
        
        public RoundCard(Round round)
        {
            InitializeComponent();

            this.round = round;

            BindingContext = this;
        }

        public string RoundName => LocalisationManager.ToTitleCase(round.RoundName);
        public string Date => LocalisationManager.LocalisedShortDate(round.Date);

        public int Hits => round.Hits();
        public int Tens => round.CountScore(EScore.TEN) + round.CountScore(EScore.X);
        public int Score => round.Score();
    }
}