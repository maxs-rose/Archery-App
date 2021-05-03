using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using TheScoreBook.acessors;
using TheScoreBook.Annotations;
using TheScoreBook.behaviours;
using TheScoreBook.game;
using TheScoreBook.localisation;
using TheScoreBook.models.enums;
using TheScoreBook.models.round;
using TheScoreBook.views.user;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;
using Xamarin.Forms.Xaml;
using Label = Xamarin.Forms.Label;

namespace TheScoreBook.views.shoot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DistanceDisplay : Frame
    {
        private int DistanceIndex { get; }
        private Distance Distance { get; }

        private bool ButtonsWork { get; }
        
        private int arrowsPerEnd;
        private int endCount = 0;
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
            
            endLabels = new Label[arrowsPerEnd * Distance.MaxEnds];
            endTotals = new Label[3 * Distance.MaxEnds];
            inputButtons = new Button[Distance.MaxEnds];

            ButtonsWork = true;
            
            CreateEndDisplay();
            UpdateEndTotalsUI();
        }

        public DistanceDisplay(Distance distance)
        {
            InitializeComponent();

            ButtonsWork = false;
            
            Distance = distance;

            arrowsPerEnd = Distance.Ends[0].ArrowsPerEnd;
            
            endLabels = new Label[arrowsPerEnd * Distance.MaxEnds];
            endTotals = new Label[3 * Distance.MaxEnds];
            
            CreateEndDisplay();
            UpdateEndTotalsUI();

            DistanceIndex = 0;
            
            for(var i = 1; i < distance.MaxEnds; i++)
            {
                AddEnd();
            }
            UpdateUI(0, 0);
            UpdateDistanceTotalsUI();
        }
        
        ~DistanceDisplay()
        {
            if(ButtonsWork)
                Scoring.UpdateScoringUiEvent -= UpdateUI;
            
            if(distanceLabel != null)
                UserData.SightMarksUpdatedEvent -= UpdateDistanceText;
        }

        private string GetDistanceSightMark()
        {
            var mark = UserData.SightMarks.FirstOrDefault(m => m.Distance == Distance.DistanceLength && m.DistanceUnit == Distance.DistanceUnit);
            return mark != default ? $"{LocalisationManager.Instance["SightMark"]}: {mark.ToScoringString()}" : "";
        }

        private Label distanceLabel = null;
        
        private void AddDistanceSightMark(Label l)
        {
            if (!ButtonsWork)
                return;
            
            var mark = GetDistanceSightMark();

            if (mark.Length > 0)
            {
                l.Text += $" | {mark}";

                if (distanceLabel == null)
                    return;

                UserData.SightMarksUpdatedEvent -= UpdateDistanceText;
                distanceLabel = null;
                l.InputTransparent = false;
                l.InputTransparent = true;
                l.GestureRecognizers.Clear();
            }
            else
            {
                distanceLabel = l;
                
                // only subscribe to the event if we need to to help with performance
                UserData.SightMarksUpdatedEvent += UpdateDistanceText;
                l.InputTransparent = false;
                l.Text += " | +";
                l.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() =>
                    {
                        if(PopupNavigation.Instance.PopupStack.Count < 1)
                            PopupNavigation.Instance.PushAsync(new CreateSightMarkPopup(Distance.DistanceLength, Distance.DistanceUnit));
                    })
                });
            }
        }
        
        private void UpdateDistanceText()
        {
            if (distanceLabel == null)
                return;
            
            distanceLabel.Text = $"{Distance.DistanceLength}{Distance.DistanceUnit.ToString()}";
            AddDistanceSightMark(distanceLabel);
        }

        private void CreateEndDisplay()
        {
            EndDisplay.RowDefinitions.Add(new RowDefinition
            {
                Height = GridLength.Star
            });

            EndDisplay.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Star
            });

            AddLabel($"{Distance.DistanceLength}{Distance.DistanceUnit.ToString()}", 0, 0, horizonatal: TextAlignment.Start);
            AddDistanceSightMark((Label)EndDisplay.Children[^1]);
            Grid.SetColumnSpan(EndDisplay.Children[^1], arrowsPerEnd);
            
            AddLabel("ET", arrowsPerEnd, 0);
            AddLabel("G", arrowsPerEnd + 1, 0);
            AddLabel("RT", arrowsPerEnd + 2, 0);

            AddEnd();
        }

        private void AddEnd()
        {
            EndSelectionBox(endCount);
            for (var j = 0; j < arrowsPerEnd; j++)
                endLabels[arrowsPerEnd * endCount + j] = AddLabel("", j, endCount + 1);
            
            AddEndTotals(endCount);

            endCount++;
        }
        
        Label AddLabel(string text, int col, int row, TextAlignment vertical = TextAlignment.Center, TextAlignment horizonatal = TextAlignment.Center)
        {
            EndDisplay.Children.Add(new Label()
            {
                Text = text,
                InputTransparent = true,
                VerticalTextAlignment = vertical,
                HorizontalTextAlignment = horizonatal
            }, col, row);
            
            return EndDisplay.Children[^1] as Label; 
        }

        private void EndSelectionBox(int row)
        {
            if (!ButtonsWork)
                return;
            
            var selectionButton = new Button()
            {
                Margin = 0,
                Padding = 0,
                BackgroundColor = Color.Transparent,
                BorderWidth = 3,
                HeightRequest = 50
            };
            
            selectionButton.Clicked += delegate
            {
                if(GameManager.NextDistanceIndex() == DistanceIndex 
                   && GameManager.NextEndIndex(DistanceIndex) == row)
                    OpenScoreUI(row);
            };

            selectionButton.Behaviors.Add(new LongButtonPressBehaviour
            {
                Command = new Command(() =>
                {
                    if (Distance.AllEndsComplete() || // if the entire distance is complete we can long press this
                        GameManager.EndComplete(DistanceIndex, row)) // if the distance is not complete then this end should be completed
                        OpenScoreUI(row);
                })
            });

            inputButtons[row] = selectionButton;
            EndDisplay.Children.Add(selectionButton, 0, arrowsPerEnd+3, row+1, row+2);
        }

        private void AddEndTotals(int end)
        {
            for (var j = 0; j < 3; j++)
                endTotals[3 * end + j] = AddLabel("", arrowsPerEnd + j, end + 1);
        }

        private void UpdateUI(int distance, int end)
        {
            if (distance != DistanceIndex) // dont update the ui if we have been called from another distance
                return;
            
            UpdateEndArrowsUI();
            AddNewEnd(end);
            UpdateEndTotalsUI();
            UpdateDistanceTotalsUI();
        }
        
        private void UpdateEndArrowsUI()
        {
            for (var i = 0; i < endCount; i++)
                for (var j = 0; j < arrowsPerEnd; j++)
                   endLabels[arrowsPerEnd * i + j].Text = Distance.Ends[i].GetScore(j)?.ToUserString();
        }

        private void AddNewEnd(int end)
        {
            // add a new end if we need to
            if(!Distance.AllEndsComplete() && endCount < Distance.MaxEnds && endCount == end + 1)
                AddEnd();
        }

        private void UpdateEndTotalsUI()
        {
            for (var i = 0; i < endCount; i++)
            {
                if(ButtonsWork)
                {
                    if (i == GameManager.NextEndIndex(DistanceIndex)
                        && GameManager.NextDistanceIndex() == DistanceIndex)
                        inputButtons[i].BorderColor = Color.Black;
                    else
                        inputButtons[i].BorderColor = Color.Transparent;
                }

                var ends = Distance.Ends; // local var for shorter lines of code
                
                if (ends[i]?.GetScore(0) == null)
                    continue;

                endTotals[3 * i + 0].Text = ends[i].Score().ToString();
                endTotals[3 * i + 1].Text = ends[i].Golds().ToString();
                endTotals[3 * i + 2].Text = Distance.RunningTotal(i).ToString();
            }
        }

        private bool alreadyAdded = false;

        private void UpdateDistanceTotalsUI()
        {
            // only adds the distance totals at the very end
            if (!Distance.AllEndsComplete())
                return;

            if (!alreadyAdded)
                for (var i = 0; i < 3; i++)
                    AddLabel("-1", arrowsPerEnd + i, endCount + 1);

            alreadyAdded = true;

            ((Label) EndDisplay.Children[^1]).Text = Distance.Score().ToString();
            ((Label) EndDisplay.Children[^2]).Text = Distance.Golds().ToString();
            ((Label) EndDisplay.Children[^3]).Text = $"{LocalisationManager.Instance["Hits"]}: {Distance.Hits()}";
        }

        private void OpenScoreUI(int row)
        {
            if (PopupNavigation.Instance.PopupStack.Count >= 1)
                return;
            
            PopupNavigation.Instance.PushAsync(new ScoreInputKeyboard(DistanceIndex, row, arrowsPerEnd));
        }
    }
}