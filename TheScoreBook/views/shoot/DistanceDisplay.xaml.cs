using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using TheScoreBook.acessors;
using TheScoreBook.behaviours;
using TheScoreBook.game;
using TheScoreBook.localisation;
using TheScoreBook.models.enums;
using TheScoreBook.models.round;
using Xamarin.Forms;

namespace TheScoreBook.views.shoot
{
    public partial class DistanceDisplay : Frame
    {
        private int DistanceIndex { get; }
        private Distance Distance { get; }

        private int arrowsPerEnd;
        private int endCount;
        private Label[] endLabels;
        private Label[] endTotals;
        private Button[] inputButtons;
        
        public DistanceDisplay(int distanceIndex)
        {
            InitializeComponent();
            
            Scoring.UpdateScoringUiEvent += UpdateUI;
            
            DistanceIndex = distanceIndex;
            Distance = GameManager.GetDistance(distanceIndex);

            arrowsPerEnd = Distance.Ends[0].ArrowsPerEnd;
            endCount = Distance.MaxEnds; 
            
            endLabels = new Label[arrowsPerEnd * endCount];
            endTotals = new Label[3 * endCount];
            inputButtons = new Button[endCount];
            CreateEndDisplay();
            
            inputButtons[0].BorderColor = Color.Pink;
            inputButtons[0].HeightRequest = 50;
        }
        
        ~DistanceDisplay()
        {
            Scoring.UpdateScoringUiEvent -= UpdateUI;
        }

        private string GetDistanceSightMark()
        {
            var mark = UserData.SightMarks.FirstOrDefault(m => m.Distance == Distance.DistanceLength && m.DistanceUnit == Distance.DistanceUnit);
            return mark != default ? $"\t\t{LocalisationManager.Instance["SightMark"]}: {mark.ToScoringString()}" : "";
        }
        
        private void CreateEndDisplay()
        {
            for (var i = 0; i < endCount; i++) // adds and extra row for the header
                EndDisplay.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Star
                });
            
            for (var i = 0; i < arrowsPerEnd; i++) // add 3 extra for the totals
                EndDisplay.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = GridLength.Star
                });
            
            EndDisplay.Children.Add(new Label()
            {
                Text = $"{Distance.DistanceLength}{Distance.DistanceUnit.ToString()}" + GetDistanceSightMark()
            }, 0, 0);
            Grid.SetColumnSpan(EndDisplay.Children[^1], arrowsPerEnd);
            
            EndDisplay.Children.Add(new Label()
            {
                Text = "ET",
                InputTransparent = true,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            }, arrowsPerEnd, 0);
            
            EndDisplay.Children.Add(new Label()
            {
                Text = "G",
                InputTransparent = true,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            }, arrowsPerEnd+1, 0);
            
            EndDisplay.Children.Add(new Label()
            {
                Text = "RT",
                InputTransparent = true,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            }, arrowsPerEnd+2, 0);
            
            
            for(var i = 0; i < endCount; i++)
                AddEnd(i);
            
            AddEndTotals();
        }

        private void AddEnd(int row)
        {
            EndSelectionBox(row);
            for (var j = 0; j < arrowsPerEnd; j++)
            {
                var box = new Label
                {
                    Text = "",
                    InputTransparent = true,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                };

                endLabels[arrowsPerEnd * row + j] = box;
                EndDisplay.Children.Add(box, j, row+1);
            }
        }

        private void EndSelectionBox(int row)
        {
            var selectionButton = new Button()
            {
                Margin = 0,
                Padding = 0,
                BackgroundColor = Color.Transparent,
                BorderWidth = 3
            };
            
            selectionButton.Clicked += delegate
            {
                if(GameManager.NextDistanceIndex() == DistanceIndex 
                   && GameManager.NextEndIndex(DistanceIndex) == row)
                    OpenScoreUI(row);
            };

            selectionButton.Behaviors.Add(new LongButtonPressBehaviour()
            {
                Command = new Command(() =>
                {
                    if (GameManager.NextDistanceIndex() >= DistanceIndex
                        && GameManager.EndComplete(DistanceIndex, row))
                        OpenScoreUI(row);
                })
            });

            inputButtons[row] = selectionButton;
            EndDisplay.Children.Add(selectionButton, 0, arrowsPerEnd+3, row+1, row+2);
        }

        private void AddEndTotals()
        {
            for (var i = 0; i < endCount; i++)
            {
                for(var j = 0; j < 3; j++)
                    AddTotal(j, i);
            }

            void AddTotal(int col, int row)
            {
                var t = new Label()
                {
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                endTotals[3 * row + col] = t;
                EndDisplay.Children.Add(t, arrowsPerEnd+col, row+1);
            }
        }

        private void UpdateUI()
        {
            var ends = Distance.Ends;

            for (var i = 0; i < endCount; i++)
                for (var j = 0; j < arrowsPerEnd; j++)
                    endLabels[arrowsPerEnd * i + j].Text = ends[i].GetScore(j)?.ToUserString();

            for (var i = 0; i < endCount; i++)
            {
                if (i == GameManager.NextEndIndex(DistanceIndex)
                    && GameManager.NextDistanceIndex() == DistanceIndex)
                {
                    inputButtons[i].BorderColor = Color.Pink;
                    inputButtons[i].HeightRequest = 50;
                }
                else
                {
                    inputButtons[i].BorderColor = Color.Transparent;
                    inputButtons[i].HeightRequest = 0;
                }
                
                if (ends[i]?.GetScore(0) == null)
                    continue;
                
                endTotals[3 * i + 0].Text = ends[i].Score().ToString();
                endTotals[3 * i + 1].Text = ends[i].Golds().ToString();
                endTotals[3 * i + 2].Text = Distance.RunningTotal(i).ToString();
            }
        }
        
        private void OpenScoreUI(int row)
        {
            PopupNavigation.Instance.PushAsync(new ScoreInputKeyboard(DistanceIndex, row, arrowsPerEnd));
            Debug.WriteLine($"Clicked {row}!");
        }
    }
}