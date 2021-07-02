using System.Linq;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using TheScoreBook.acessors;
using TheScoreBook.localisation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TheScoreBook.views.shoot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoundSelectionPopup : PopupPage
    {
        private RoundSelectionPage selectionPage;
        public RoundSelectionPopup(RoundSelectionPage selectionPage)
        {
            InitializeComponent();
            this.selectionPage = selectionPage;
            
            foreach (var g in Rounds.Instance.GetGroupedRounds())
            {
                RoundScroll.Children.Add(new Label
                {
                    BackgroundColor = Color.Black,
                    TextColor = Color.White,
                    Padding = new Thickness(10),
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = Font.SystemFontOfSize(NamedSize.Title).FontSize,
                    Text = g.group.DisplayName
                } );

                var groupRounds = new StackLayout()
                {
                    Padding = 0,
                    Margin = 0,
                    IsVisible = true
                };
                
                RoundScroll.Children.Last().GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command(() =>
                    {
                        groupRounds.IsVisible = !groupRounds.IsVisible;
                    })
                });
                
                foreach (var roundName in g.roundNames)
                {
                    var l = new Label
                    {
                        Text = LocalisationManager.ToRoundTitleCase(roundName),
                        Margin = new Thickness(10, 10)
                    };
                    l.GestureRecognizers.Add(new TapGestureRecognizer()
                    {
                        Command = new Command(() => RoundSelected(l.Text))
                    });
                    groupRounds.Children.Add(l);
                }
                
                RoundScroll.Children.Add(groupRounds);

            }

            BindingContext = this;
        }

        private void RoundSelected(string roundName)
        {
            selectionPage.SelectedRound = roundName;
            PopupNavigation.Instance.PopAsync();
        }
    }
}