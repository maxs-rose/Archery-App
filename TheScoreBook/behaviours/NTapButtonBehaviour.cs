using System;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace TheScoreBook.behaviours
{
    public class NTapButtonBehaviour : Behavior<Button>
    {
        private readonly object syncObject = new object();
        private const int tapSpeed = 500;

        private Timer timer;
        private readonly int duration;
        private int Taps { get; }
        private volatile int currentTaps;
        
        public event EventHandler NTappedEvent;
        
        public NTapButtonBehaviour(int taps)
        {
            Taps = taps;
            duration = tapSpeed;
        }

        protected virtual void OnNTapped()
        {
            currentTaps = 0;
            NTappedEvent?.Invoke(this, EventArgs.Empty);

            if (Command != null && Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);
        }
        
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
        }

        protected override void OnDetachingFrom(Button button)
        {
            base.OnDetachingFrom(button);
            BindingContext = null;
            button.Pressed -= Button_Pressed;
        }

        private void Button_Pressed(object sender, EventArgs e)
        {
            CreateTimer();
            currentTaps = currentTaps + 1;
            
            if(currentTaps >= Taps)
                TimerElapsed(this);
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
                timer = new Timer(TimerElapsed, null, duration, Timeout.Infinite);
            }
        }
        
        private void TimerElapsed(object state)
        {
            DestroyTimer();
            if (currentTaps < Taps)
            {
                currentTaps = 0;
                return;
            }
            Device.BeginInvokeOnMainThread(OnNTapped);
        }
    }
}