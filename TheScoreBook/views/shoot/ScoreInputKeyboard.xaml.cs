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
using Xamarin.Forms.Markup;

namespace TheScoreBook.views.shoot
{
    public partial class ScoreInputKeyboard : PopupPage
    {
        private readonly int Distance;
        private readonly int End;
        private readonly int ArrowsPerEnd;

        private List<Score> inputScores = new ();
        private List<ScoreInputButton> displayButtons = new();
        
        public ScoreInputKeyboard(int distance, int end, int arrowsPerEnd)
        {
            InitializeComponent();
            BindingContext = this;

            Distance = distance;
            End = end;
            ArrowsPerEnd = arrowsPerEnd;
            
            Container.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(ClosePopup)
            });
            
            GenerateDisplayButtons();
            SetButtonBindings();
        }

        private void GenerateDisplayButtons()
        {
            if(GameManager.EndComplete(Distance, End))
                for(var i = 0; i < ArrowsPerEnd; i++)
                    inputScores.Add(GameManager.GetScore(Distance, End, i));

            var col = 0;
            var row = 0;
            for (var i = 0; i < ArrowsPerEnd; i++)
            {
                if (i % 3 == 0 && i != 0)
                    CreateBreak(ref row, ref col);
                
                if(col < 1 && row < 1)
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

        private void CreateBreak(ref int row, ref int col)
        {
            ScoreDisplay.RowDefinitions.Add(new RowDefinition
            {
                Height = GridLength.Star
            });
            col = 0;
            row++;
        }

        private void SetButtonBindings()
        {
            foreach (var b in ScoreInput.Children)
            {
                if (b is not ScoreInputButton)
                    continue;

                b.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = GenerateButtonCommand((ScoreInputButton) b)
                });
            }

            ReorderForScoring();
        }

        private void ReorderForScoring()
        {
            if (!GameManager.IsFiveZone())
                return;
            
            var removeList = new Stack<View>();
            
            foreach (var b in ScoreInput.Children)
                if (GameManager.IsFiveZone() && !((ScoreInputButton) b).IsFiveZoneScore())
                    removeList.Push(b);

            while (removeList.Any())
                ScoreInput.Children.Remove(removeList.Pop());

            // 9
            Grid.SetColumn(ScoreInput.Children[0], 0);
            // 7
            Grid.SetColumn(ScoreInput.Children[1], 2);
            Grid.SetRow(ScoreInput.Children[1], 0);
            // 5
            Grid.SetColumn(ScoreInput.Children[2], 1);
            Grid.SetRow(ScoreInput.Children[2], 1);
            // 3
            Grid.SetColumn(ScoreInput.Children[3], 0);
            Grid.SetRow(ScoreInput.Children[3], 2);
            // 1
            Grid.SetColumn(ScoreInput.Children[4], 2);
            Grid.SetRow(ScoreInput.Children[4], 2);
        }

        private ICommand GenerateButtonCommand(ScoreInputButton b)
        {
            return b.WordString switch
            {
                "" => new Command(() => InputScore(b.Score)),
                "🗑" => new Command(RemoveScore), // not sure if using emoji to match against is the best idea but no problem yet ☺
                "✔" => new Command(AcceptScores),
                _ => throw new NotImplementedException($"{b.WordString} is not a know button!")
            };
        }

        private void InputScore(Score score)
        {
            if (inputScores.Count >= ArrowsPerEnd || score == null)
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
            Scoring.UpdateScoringUiEvent?.Invoke(Distance, End);
            ClosePopup();
        }

        private void ClosePopup()
        {
            PopupNavigation.Instance.PopAsync(true);
        }
    }
}