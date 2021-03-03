using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormsControls.Base;
using TheScoreBook.acessors;
using TheScoreBook.localisation;
using TheScoreBook.models.enums;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TheScoreBook.views.shoot
{
    public partial class FinishedRound : Frame
    {
        public FinishedRound()
        {
            InitializeComponent();
            RoundName.Text = UserData.LatestRound.RoundName;
            Date.Text = LocalisationManager.LocalisedShortDate(UserData.LatestRound.Date);
            Bow.Text = UserData.LatestRound.Style.ToDisplayString();
            Score.Text = UserData.LatestRound.Score().ToString();
        }
    }
}