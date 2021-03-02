using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TheScoreBook.views.shoot
{
    public partial class ScoreInputKeyboard : PopupPage
    {
        public ScoreInputKeyboard()
        {
            InitializeComponent();
            BindingContext = this;
        }
    }
}