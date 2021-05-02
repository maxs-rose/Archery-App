using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using TheScoreBook.game;
using TheScoreBook.localisation;
using TheScoreBook.models.enums;
using Xamarin.Forms;

namespace TheScoreBook.views.shoot
{
    public partial class ScoreInputKeyboard : PopupPage
    {
        private readonly int Distance;
        private readonly int End;
        private readonly int ArrowsPerEnd;

        private List<EScore> inputScores = new ();
        private List<ScoreInputButton> displayButtons = new();
        
        public ScoreInputKeyboard(int distance, int end, int arrowsPerEnd)
        {
            InitializeComponent();
            BindingContext = this;

            Distance = distance;
            End = end;
            ArrowsPerEnd = arrowsPerEnd;
            
            GenerateDisplayButtons();
            SetButtonBindings();
        }

        private void GenerateDisplayButtons()
        {
            if(GameManager.EndComplete(Distance, End))
                for(var i = 0; i < ArrowsPerEnd; i++)
                    inputScores.Add((EScore) GameManager.GetScore(Distance, End, i));

            var col = 0;
            var row = 0;
            for (var i = 0; i < ArrowsPerEnd; i++)
            {
                if (i % 3 == 0 && i != 0)
                {
                    ScoreDisplay.RowDefinitions.Add(new RowDefinition
                    {
                        Height = GridLength.Star
                    });
                    col = 0;
                    row++;
                }
                
                if(col < 1)
                    ScoreDisplay.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = GridLength.Star
                    });
                
                displayButtons.Add(new ScoreInputButton
                {
                    BackgroundColor = Color.Transparent,
                    Margin = 0,
                    Padding = 0,
                });
                
                displayButtons[^1].Score = inputScores.Any() ? inputScores[i] : null;
                
                ScoreDisplay.Children.Add(displayButtons[^1], col++, row);
            }
        }

        private void SetButtonBindings()
        {
            foreach (ScoreInputButton b in ScoreInput.Children)
            {
                ICommand com = b.WordString switch
                {
                    "" => new Command(() => InputScore((EScore) b.Score!)),
                    "🗑" => new Command(RemoveScore),
                    "✔" => new Command(AcceptScores),
                    _ => throw new NotImplementedException($"{b.WordString} is not a know button!")
                };

                b.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = com
                });
            }
        }

        private void InputScore(EScore score)
        {
            if (inputScores.Count >= ArrowsPerEnd)
                return;
            
            inputScores.Add(score);
            displayButtons[inputScores.Count - 1].Score = inputScores[^1];
        }

        private void RemoveScore()
        {
            if (inputScores.Count <= 0)
                return;
            
            inputScores.RemoveAt(inputScores.Count - 1);
            displayButtons[inputScores.Count].Score = null;
        }

        private void AcceptScores()
        {
            if (GameManager.EndComplete(Distance, End))
                GameManager.ClearEnd(Distance, End);
            
            foreach (var s in inputScores)
                GameManager.AddScore(Distance, End, s);

            GameManager.Finish(Distance, End);
            PopupNavigation.Instance.PopAsync(true);
            Scoring.UpdateScoringUiEvent?.Invoke(Distance, End);
        }
    }
}