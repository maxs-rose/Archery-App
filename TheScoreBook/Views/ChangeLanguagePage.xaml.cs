using TheScoreBook.localisation;
using Xamarin.Forms;

namespace TheScoreBook.Views
{
    public partial class ChangeLanguagePage : ContentPage
    {
        public ChangeLanguagePage()
        {
            InitializeComponent();
            BindingContext = new ChangeLanguage();
        }
    }
}