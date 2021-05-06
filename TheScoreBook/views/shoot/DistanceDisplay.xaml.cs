using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Bson;
using Rg.Plugins.Popup.Services;
using TheScoreBook.acessors;
using TheScoreBook.Annotations;
using TheScoreBook.behaviours;
using TheScoreBook.game;
using TheScoreBook.localisation;
using TheScoreBook.models.enums;
using TheScoreBook.models.round;
using TheScoreBook.views.user;
using Xamarin.Essentials;
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

        private ScrollView Scroll { get; }

        private int arrowsPerEnd;
        private int endCount = 0;
        private Label[] endLabels;
        private Label[] endTotals;
        private Button[] inputButtons;

        private Label distanceLabel = null;

        public DistanceDisplay(int distanceIndex, ScrollView scroll)
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

            Scroll = scroll;

            CreateEndDisplay();
            UpdateUI(DistanceIndex, -1);
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

            for (var i = 1; i < distance.MaxEnds; i++)
                AddEnd();

            UpdateUI(0, 0);
            UpdateDistanceTotalsUI();
        }

        ~DistanceDisplay()
        {
            if (!ButtonsWork)
                Scoring.UpdateScoringUiEvent -= UpdateUI;
            
            UserData.SightMarksUpdatedEvent -= UpdateDistanceText;
        }

        Label AddLabel(string text, int col, int row, TextAlignment vertical = TextAlignment.Center,
            TextAlignment horizonatal = TextAlignment.Center)
        {
            EndDisplay.Children.Add(new Label
            {
                Text = text,
                InputTransparent = true,
                VerticalTextAlignment = vertical,
                HorizontalTextAlignment = horizonatal
            }, col, row);

            return EndDisplay.Children[^1] as Label;
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

            AddDistanceLabel();

            AddLabel("ET", arrowsPerEnd, 0);
            AddLabel("G", arrowsPerEnd + 1, 0);
            AddLabel("RT", arrowsPerEnd + 2, 0);
        }

        #region Distance & Sight Marks
        
        private void AddDistanceLabel()
        {
            distanceLabel = AddLabel($"{Distance.DistanceLength}{Distance.DistanceUnit.ToString()}", 0, 0,
                horizonatal: TextAlignment.Start);
            
            UpdateDistanceText();
            Grid.SetColumnSpan(EndDisplay.Children[^1], arrowsPerEnd);
        }
        
        private void UpdateDistanceText()
        {
            distanceLabel.Text = $"{Distance.DistanceLength}{Distance.DistanceUnit.ToString()}";
            AddSightMarkToDistance(distanceLabel);
        }
        
        private void AddSightMarkToDistance(Label l)
        {
            if (!ButtonsWork)
                return;

            var mark = GetDistanceSightMark();

            if (mark != default)
            {
                AppendSightMarkToLabel(l, mark);
                ClearLabelGestures(l);
            }
            else
                AddCreateSightMarkToLabel(l);
        }
        
        private string GetDistanceSightMark()
        {
            var mark = UserData.SightMarks.FirstOrDefault(m =>
                m.Distance == Distance.DistanceLength && m.DistanceUnit == Distance.DistanceUnit);
            return mark != default ? $"{LocalisationManager.Instance["SightMark"]}: {mark.ToScoringString()}" : default;
        }
        
        private void AppendSightMarkToLabel(Label l, string mark)
        {
            l.Text += $" | {mark}";
            l.InputTransparent = true;
        }
        
        private void ClearLabelGestures(Label l)
        {
            UserData.SightMarksUpdatedEvent -= UpdateDistanceText;
            l.GestureRecognizers.Clear();
        }
        
        private void AddCreateSightMarkToLabel(Label l)
        {
            // only subscribe to the event if we need to to help with performance
            UserData.SightMarksUpdatedEvent += UpdateDistanceText;
            
            l.InputTransparent = false;
            l.Text += " | +";
            l.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    if (ShouldOpenPopup())
                        PopupNavigation.Instance.PushAsync(new CreateSightMarkPopup(Distance.DistanceLength, Distance.DistanceUnit));
                })
            });
        }

        #endregion

        #region End & Totals UI

        private async void UpdateUI(int distance, int end)
        {
            if (distance != DistanceIndex) // dont update the ui if we have been called from another distance
                return;

            UpdateEndArrowsUI(end);
            if (ShouldAddNewEnd(end))
                AddEnd();
            UpdateEndTotalsUI();
            UpdateDistanceTotalsUI();

            await ScrollToBottom();
        }
        
        private void UpdateEndArrowsUI(int endIndex)
        {
            if (endIndex < 0 && endIndex >= endCount)
                return;
            
            for (var i = 0; i < arrowsPerEnd; i++)
                endLabels[arrowsPerEnd * endIndex + i].Text = Distance.Ends[endIndex].GetScore(i)?.ToUserString();
        }
        
        private void AddEnd()
        {
            AddEndSelectionBox(endCount);
            AddEndScoreLabels();
            AddEndTotalLabels(endCount);

            endCount++;
        }

        private void AddEndSelectionBox(int row)
        {
            if (!ButtonsWork)
                return;

            var selectionButton = new Button
            {
                Margin = 0,
                Padding = 0,
                BackgroundColor = Color.Transparent,
                BorderWidth = 3,
                HeightRequest = 50,
                Behaviors =
                {
                    new LongButtonPressBehaviour
                    {
                        Command = new Command(() =>
                        {
                            if (
                                Distance.AllEndsComplete() || // if the entire distance is complete we can long press this
                                Distance.EndComplete(row) // if the distance is not complete then this end should be completed
                            )
                                OpenScoreUI(row);
                        })
                    }
                }
            };

            // not sure why this cant be added whilst the object is being constructed
            selectionButton.Clicked += delegate
            {
                if (ShouldOpenScoreUI(row))
                    OpenScoreUI(row);
            };

            inputButtons[row] = selectionButton;
            EndDisplay.Children.Add(selectionButton, 0, arrowsPerEnd + 3, row + 1, row + 2);
        }

        private void AddEndScoreLabels()
        {
            for (var i = 0; i < arrowsPerEnd; i++)
                endLabels[arrowsPerEnd * endCount + i] = AddLabel("", i, endCount + 1);
        }
        
        private void AddEndTotalLabels(int end)
        {
            for (var j = 0; j < 3; j++)
                endTotals[3 * end + j] = AddLabel("", arrowsPerEnd + j, end + 1);
        }

        private void UpdateEndTotalsUI()
        {
            for (var i = 0; i < endCount; i++)
            {
                HighlightNextEnd(i);

                if (!EndHasScores(i))
                    continue;

                endTotals[3 * i + 0].Text = Distance.Ends[i].Score().ToString();
                endTotals[3 * i + 1].Text = Distance.Ends[i].Golds().ToString();
                endTotals[3 * i + 2].Text = Distance.RunningTotal(i).ToString();
            }
        }

        private void HighlightNextEnd(int endIndex)
        {
            if (!ButtonsWork)
                return;

            inputButtons[endIndex].BorderColor = IsNextEnd(endIndex) ? Color.Black : Color.Transparent;
        }
        
        private bool alreadyAdded = false;
        private void UpdateDistanceTotalsUI()
        {
            // only adds the distance totals at the very end
            // since this could be called multiple times before the round is finished
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
        
        private async Task ScrollToBottom()
        {
            if (!ButtonsWork)
                return;
            
            await Scroll.ScrollToAsync(Scroll.Children.First(), ScrollToPosition.End, false);

            if (Device.RuntimePlatform == Device.iOS && Scroll.ContentSize.Height > Scroll.Height)
            {
                // iOS is a pain in the ass so it needs some more movement for some reason
                // Without this the scroll stops half way up the highlighted box
                var scrollAmount = Scroll.ScrollY + inputButtons[0].Height;
                await Scroll.ScrollToAsync(Scroll.ScrollX, scrollAmount, false);
            }
        }

        private bool ShouldAddNewEnd(int end)
        {
            return !Distance.AllEndsComplete() && endCount < Distance.MaxEnds && endCount == end + 1;
        }
        
        private bool EndHasScores(int endIndex)
            => Distance.Ends[endIndex]?.GetScore(0) != null;

        private bool IsNextEnd(int endIndex)
            => endIndex == Distance.NextEndIndex() && GameManager.NextDistanceIndex() == DistanceIndex;

        #endregion

        #region Popup

        private bool ShouldOpenScoreUI(int row)
        {
            return GameManager.NextDistanceIndex() == DistanceIndex
                   && Distance.NextEndIndex() == row
                   && ShouldOpenPopup();
        }

        private bool ShouldOpenPopup()
            => PopupNavigation.Instance.PopupStack.Count < 1;

        private void OpenScoreUI(int row)
        {
            PopupNavigation.Instance.PushAsync(new ScoreInputKeyboard(DistanceIndex, row, arrowsPerEnd));
        }

        #endregion
    }
}