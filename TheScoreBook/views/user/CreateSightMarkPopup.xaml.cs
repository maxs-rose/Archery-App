using System;
using System.Collections.Generic;
using System.Linq;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using TheScoreBook.acessors;
using TheScoreBook.models;
using TheScoreBook.models.enums;

namespace TheScoreBook.views.user
{
    public partial class CreateSightMarkPopup : PopupPage
    {
        public List<string> Distances => Enum.GetNames(typeof(EDistanceUnit)).ToList();
        public int SelectedDistance { get; set; }
        
        public CreateSightMarkPopup()
        {
            InitializeComponent();
            BindingContext = this;
        }


        private void DoneButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Position.Text) ||
                string.IsNullOrEmpty(Notch.Text)    ||
                string.IsNullOrEmpty(Distance.Text))
                return;

            var pos = int.Parse(Position.Text);
            var not = int.Parse(Notch.Text);
            var dst = int.Parse(Distance.Text);
            var unt = Distances[SelectedDistance].ToEDistanceUnit();
            
            UserData.Instance.AddSightMark(new SightMark(dst, unt, pos, not));
            PopupNavigation.Instance.PopAsync();
        }
    }
}