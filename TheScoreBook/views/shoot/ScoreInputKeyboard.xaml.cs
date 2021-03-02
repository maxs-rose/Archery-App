using System.Collections.Generic;
using System.Linq;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using TheScoreBook.game;
using TheScoreBook.models.enums;

namespace TheScoreBook.views.shoot
{
    public partial class ScoreInputKeyboard : PopupPage
    {
        private readonly int Distance;
        private readonly int End;
        private readonly int ArrowsPerEnd;

        private List<EScore> inputScores = new ();
        
        public ScoreInputKeyboard(int distance, int end, int arrowsPerEnd)
        {
            InitializeComponent();
            BindingContext = this;

            Distance = distance;
            End = end;
            ArrowsPerEnd = arrowsPerEnd;
            
            GenerateDisplayButtons();
        }

        private void GenerateDisplayButtons()
        {
            
        }

        private void InputScore(EScore score)
        {
            if (inputScores.Count >= ArrowsPerEnd)
                return;
            
            inputScores.Add(score);
        }

        private void RemoveScore()
        {
            if (inputScores.Count <= 0)
                return;
            
            inputScores.RemoveAt(inputScores.Count - 1);
        }

        private void AcceptScores()
        {
            foreach (var s in inputScores)
                GameManager.AddScore(Distance, End, s);

            GameManager.Finish(Distance, End);
            PopupNavigation.Instance.PopAsync(true);
        }
    }
}