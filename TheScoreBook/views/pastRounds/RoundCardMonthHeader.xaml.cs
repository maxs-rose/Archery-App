using System;
using System.Threading;
using Xamarin.Forms;

namespace TheScoreBook.views.pastRounds
{
    public partial class RoundCardMonthHeader : Frame
    {
        public string Month { get; }
        
        public RoundCardMonthHeader(DateTime date)
        {
            InitializeComponent();
            
            Month = date.ToString("MMMM yyyy", Thread.CurrentThread.CurrentUICulture);
            
            BindingContext = this;
        }
    }
}