using System;
using System.Collections.Generic;
using System.Linq;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using TheScoreBook.acessors;
using TheScoreBook.models;
using TheScoreBook.models.enums;
using Xamarin.Forms.Xaml;

namespace TheScoreBook.views.user
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateSightMarkPopup : PopupPage
    {
        public List<string> Distances => MeasurementUnit.DistanceUnits().Select(d => d.ToString()).ToList();
        public int SelectedDistance { get; set; }

        public CreateSightMarkPopup()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public CreateSightMarkPopup(int distance, MeasurementUnit unit)
        {
            InitializeComponent();
            BindingContext = this;
            Distance.Text = $"{distance}";
            SelectedDistance = unit.Id;
            DistanceUnitPicker.SelectedIndex = SelectedDistance;
        }

        private void DoneButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Position.Text) ||
                string.IsNullOrEmpty(Notch.Text) ||
                string.IsNullOrEmpty(Distance.Text))
            {
                PopupNavigation.Instance.PopAsync();
                return;
            }

            var pos = float.Parse(Position.Text);
            var not = float.Parse(Notch.Text);
            var dst = (int) float.Parse(Distance.Text);
            var unt = (MeasurementUnit) Distances[SelectedDistance];

            UserData.Instance.AddSightMark(new SightMark(dst, unt, pos, not));
            PopupNavigation.Instance.PopAsync();
        }
    }
}