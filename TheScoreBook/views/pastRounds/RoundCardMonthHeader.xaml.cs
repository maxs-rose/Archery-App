using System;
using System.Threading;
using Xamarin.Forms;

namespace TheScoreBook.views.pastRounds
{
    public partial class RoundCardMonthHeader : Frame
    {
        private string month;
        public string Month => month;
        
        public RoundCardMonthHeader(DateTime date)
        {
            InitializeComponent();
            
            month = date.ToString("MMMM yyyy", Thread.CurrentThread.CurrentUICulture);
            
            BindingContext = this;
        }
    }
}