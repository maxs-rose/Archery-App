using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TheScoreBook.views.home
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