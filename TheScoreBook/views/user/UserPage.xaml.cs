using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheScoreBook.acessors;
using TheScoreBook.localisation;
using TheScoreBook.models.enums;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TheScoreBook.views.user
{
    public partial class UserPage : Frame
    {
        public UserPage()
        {
            InitializeComponent();

            // group rounds name -> count number of rounds in each group -> order by count -> round
            var prefR = UserData.Rounds
                .GroupBy(r => r.RoundName)
                .Select(g => (roundName: g.Key, round: g.First(), count: g.Count()))
                .OrderByDescending(t => t.count)
                .Select(rt => rt.round);

            // ordered rounds -> take first round of correct location
            var prefIndoor = prefR.FirstOrDefault(r => r.Location == ELocation.INDOOR);
            var prefOutdoor = prefR.FirstOrDefault(r => r.Location == ELocation.OUTDOOR);
            
            IndoorRound.Text = $"{prefIndoor?.RoundName ?? "N/A"} - {LocalisationManager.Instance["Indoor"]}";
            OutdoorRound.Text = $"{prefOutdoor?.RoundName ?? "N/A"} - {LocalisationManager.Instance["Outdoor"]}";

            // PB -> Order by score -> round
            var bestRounds = UserData.Instance
                .GetPB()
                .OrderByDescending(r => r.Score());

            // Ordered PB's -> take first of correct location
            var bestIndoor = bestRounds.FirstOrDefault(r => r.Location == ELocation.INDOOR);
            var bestOutdoor = bestRounds.FirstOrDefault(r => r.Location == ELocation.OUTDOOR);
            
            BestIndoorRound.Text = $"{bestIndoor?.RoundName ?? "N/A"} - {LocalisationManager.Instance["Indoor"]}";
            BestOutdoorRound.Text = $"{bestOutdoor?.RoundName ?? "N/A"} - {LocalisationManager.Instance["Outdoor"]}";
            
        }
    }
}
