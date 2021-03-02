using System;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

// Magic Long press code from: https://stackoverflow.com/questions/43569515/how-to-make-long-press-gesture-in-xamarin-forms

namespace TheScoreBook.behaviours
{
    public class LongButtonPressBehaviour : Behavior<Button>
    {
        private readonly object syncObject = new object();
        private const int pressDuration = 1000;

        private Timer timer;
        private readonly int duration;
        private volatile bool isReleased;

        public event EventHandler LongPressEvent;

        public static readonly BindableProperty CommandProperty = BindableProperty
            .Create(
                nameof(Command),
                typeof(ICommand),
                typeof(LongButtonPressBehaviour),
                default(ICommand)
            );

        public static readonly BindableProperty CommandParameterProperty = BindableProperty
            .Create(
                nameof(CommandParameter),
                typeof(object),
                typeof(LongButtonPressBehaviour)
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

        protected override void OnAttachedTo(Button button)
        {
            base.OnAttachedTo(button);
            BindingContext = button.BindingContext;
            button.Pressed += Button_Pressed;
            button.Released += Button_Released;
        }

        protected override void OnDetachingFrom(Button button)
        {
            base.OnDetachingFrom(button);
            BindingContext = null;
            button.Pressed -= Button_Pressed;
            button.Released -= Button_Released;
        }

        private void DestroyTimer()
        {
            lock (syncObject)
            {
                if (timer == null)
                    return;

                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
                timer = null;
            }
        }

        private void CreateTimer()
        {
            lock (syncObject)
            {
                timer = new Timer(Timer_Elapsed, null, duration, Timeout.Infinite);
            }
        }
        
        private void Button_Pressed(object sender, EventArgs e)
        {
            isReleased = false;
            CreateTimer();
        }
        
        private void Button_Released(object sender, EventArgs e)
        {
            isReleased = true;
            DestroyTimer();
        }

        protected virtual void OnLongPressed()
        {
            LongPressEvent?.Invoke(this, EventArgs.Empty);
            
            if(Command != null && Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);
        }

        public LongButtonPressBehaviour()
        {
            isReleased = true;
            duration = pressDuration;
        }

        public LongButtonPressBehaviour(int duration) : this()
        {
            this.duration = duration;
        }

        private void Timer_Elapsed(object state)
        {
            DestroyTimer();
            if (isReleased)
                return;
            Device.BeginInvokeOnMainThread(OnLongPressed);
        }
    }
}