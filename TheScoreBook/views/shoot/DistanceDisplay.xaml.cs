using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using TheScoreBook.acessors;
using TheScoreBook.behaviours;
using TheScoreBook.game;
using TheScoreBook.localisation;
using TheScoreBook.models.enums;
using TheScoreBook.models.round;
using TheScoreBook.views.user;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
        private EndScoreDisplay[] endLabels;
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

            endLabels = new EndScoreDisplay[arrowsPerEnd * Distance.MaxEnds];
            endTotals = new Label[3 * Distance.MaxEnds];
            inputButtons = new Button[Distance.MaxEnds];

            ButtonsWork = true;

            Scroll = scroll;

            CreateEndDisplay();
            UpdateUI(DistanceIndex, -1);

            for (var i = 0; i < Distance.MaxEnds && Distance.Ends[i].EndComplete(); i++)
                UpdateUI(DistanceIndex, i);
        }

        public DistanceDisplay(Distance distance)
        {
            InitializeComponent();

            ButtonsWork = false;

            Distance = distance;

            arrowsPerEnd = Distance.Ends[0].ArrowsPerEnd;

            endLabels = new EndScoreDisplay[arrowsPerEnd * Distance.MaxEnds];
            endTotals = new Label[3 * Distance.MaxEnds];

            CreateEndDisplay();

            DistanceIndex = 0;

            UpdateUI(DistanceIndex, -1);

            for (var i = 0; i < distance.MaxEnds; i++)
                UpdateUI(DistanceIndex, i);

            if (Distance.AllEndsComplete())
                AddDistanceTotalsUI();
        }

        ~DistanceDisplay()
        {
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

        EndScoreDisplay AddScoreDisplay(int col, int row)
        {
            EndDisplay.Children.Add(new EndScoreDisplay
            {
                InputTransparent = true
            }, col, row);

            return EndDisplay.Children[^1] as EndScoreDisplay;
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

            if (ShouldAddNewEnd(end))
                AddEnd();

            await ScrollToBottom();
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
                BorderWidth = 3,
                HeightRequest = 50,
                BackgroundColor = Color.Transparent,
                Behaviors =
                {
                    RescoreType.GetCurrentSetting(new Command(() =>
                        {
                            if (
                                Distance.AllEndsComplete() || // if the entire distance is complete we can long press this
                                Distance.EndComplete(row) // if the distance is not complete then this end should be completed
                            )
                                OpenScoreUI(row);
                        }
                    ))
                }
            };

            // not sure why this cant be added whilst the object is being constructed
            selectionButton.Clicked += delegate
            {
                if (ShouldOpenScoreUI(row))
                    OpenScoreUI(row);
            };

            selectionButton.BindingContext = Distance.Ends[row];
            selectionButton.SetBinding(Button.BorderColorProperty, "IsNextEnd", BindingMode.OneWay,
                new BoolToColorConvertor());

            inputButtons[row] = selectionButton;
            EndDisplay.Children.Add(selectionButton, 0, arrowsPerEnd + 3, row + 1, row + 2);
        }

        private void AddEndScoreLabels()
        {
            for (var i = 0; i < arrowsPerEnd; i++)
            {
                endLabels[arrowsPerEnd * endCount + i] = AddScoreDisplay(i, endCount + 1);
                endLabels[arrowsPerEnd * endCount + i].BindingContext = Distance.Ends[endCount];
                // we attach this to score since we cant directly attach to GetScore since it is a function
                // instead we use the convertor to actually get our value
                endLabels[arrowsPerEnd * endCount + i].SetBinding(EndScoreDisplay.ScoreTextProperty, "Score",
                    converter: new EndScoreConvertor() {ScoreIndex = i, End = Distance.Ends[endCount]});
            }
        }

        private void AddEndTotalLabels(int end)
        {
            for (var j = 0; j < 3; j++)
            {
                endTotals[3 * end + j] = AddLabel("", arrowsPerEnd + j, end + 1);
                endTotals[3 * end + j].BindingContext = Distance.Ends[end];

                endTotals[3 * end + j].SetBinding(Label.TextProperty, j switch
                {
                    0 => "Score",
                    1 => "Golds",
                    2 => "RunningTotal",
                    _ => throw new ArgumentOutOfRangeException()
                });
            }

            Distance.Ends[end].PropertyHasChanged();
        }

        private bool alreadyAdded = false;

        public void AddDistanceTotalsUI()
        {
            if (!alreadyAdded)
                for (var i = 0; i < 3; i++)
                {
                    var l = AddLabel("", arrowsPerEnd + i, endCount + 1);
                    l.BindingContext = Distance;

                    l.SetBinding(Label.TextProperty, i switch
                    {
                        2 => "Score",
                        1 => "Golds",
                        0 => "Hits",
                        _ => throw new ArgumentOutOfRangeException()
                    }, stringFormat: i == 0 ? $"{LocalisationManager.Instance["Hits"]}: " + "{0:D}" : "{0:D}");
                }

            alreadyAdded = true;
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
            return endCount < Distance.MaxEnds && endCount == end + 1;
        }

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

        #region Convertors

        private class BoolToColorConvertor : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                => (bool) value switch
                {
                    true => Settings.GetStaticResource<Color>(Settings.IsDarkMode
                        ? "DarkButtonBorder"
                        : "LightButtonBorder"),
                    false => Color.Transparent
                };

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private class EndScoreConvertor : IValueConverter
        {
            public int ScoreIndex { get; set; }
            public End End { get; set; }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return $"{End.GetScore(ScoreIndex)}";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}