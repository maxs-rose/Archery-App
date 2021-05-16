using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace TheScoreBook.behaviours
{
    public abstract class CustomButtonBehaviour<T> : Behavior<Button>
    {
        public static readonly BindableProperty CommandProperty = BindableProperty
            .Create(
                nameof(Command),
                typeof(ICommand),
                typeof(T),
                default(ICommand)
            );

        public static readonly BindableProperty CommandParameterProperty = BindableProperty
            .Create(
                nameof(CommandParameter),
                typeof(object),
                typeof(T)
            );

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected abstract void ButtonPressed(object sender, EventArgs e);

        protected override void OnAttachedTo(Button button)
        {
            base.OnAttachedTo(button);
            BindingContext = button.BindingContext;
            button.Pressed += ButtonPressed;
        }

        protected override void OnDetachingFrom(Button button)
        {
            base.OnDetachingFrom(button);
            BindingContext = null;
            button.Pressed -= ButtonPressed;
        }
    }
}