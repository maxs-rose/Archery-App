using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TheScoreBook.localisation
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension<BindingBase>
    {
        public string Text { get; set; }
        public string StringFormat { get; set; }

        public BindingBase ProvideValue(IServiceProvider serviceProvider)
        {
            return new Binding
            {
                Mode = BindingMode.OneWay,
                Path = $"[{Text}]",
                Source = LocalisationManager.Instance,
                StringFormat = StringFormat
            };
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
            => ProvideValue(serviceProvider);
    }
}