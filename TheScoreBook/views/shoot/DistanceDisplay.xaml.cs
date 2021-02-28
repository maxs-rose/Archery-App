using System.Collections.Generic;
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
            CreateEndDisplay();
        }
        
        ~DistanceDisplay()
        {
            Scoring.UpdateScoringUiEvent -= UpdateUI;
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
                Text = $"{Distance.DistanceLength}{Distance.DistanceUnit.ToString()}"
            }, 0, 0);
            Grid.SetColumnSpan(EndDisplay.Children[^1], arrowsPerEnd);
            
            EndDisplay.Children.Add(new Label()
            {
                Text = $"ET"
            }, arrowsPerEnd, 0);
            
            EndDisplay.Children.Add(new Label()
            {
                Text = $"G"
            }, arrowsPerEnd+1, 0);
            
            EndDisplay.Children.Add(new Label()
            {
                Text = $"RT"
            }, arrowsPerEnd+2, 0);
            
            
            for(var i = 0; i < endCount; i++)
                AddEnd(i);
            
            AddEndTotals();
        }

        private void AddEnd(int row)
        {
            for (var j = 0; j < arrowsPerEnd; j++)
            {
                var box = new Label
                {
                    Text = "0"
                };

                endLabels[arrowsPerEnd * row + j] = box;
                EndDisplay.Children.Add(box, j, row+1);
            }
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
                var t = new Label();
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
                if (ends[i]?.GetScore(0) == null)
                    continue;
                
                endTotals[3 * i + 0].Text = ends[i].Score().ToString();
                endTotals[3 * i + 1].Text = ends[i].Golds().ToString();
                endTotals[3 * i + 2].Text = Distance.RunningTotal(i).ToString();
            }
        }
    }
}