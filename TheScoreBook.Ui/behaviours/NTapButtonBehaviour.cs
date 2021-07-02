using System;
using Xamarin.Forms;

namespace TheScoreBook.behaviours
{
    public class NTapButtonBehaviour : TimedButtonBehaviour<NTapButtonBehaviour>
    {
        private volatile int currentTaps;

        public NTapButtonBehaviour(int taps) : base(500) => Taps = taps;

        private int Taps { get; }

        public event EventHandler NTappedEvent;

        protected virtual void OnNTapped()
        {
            currentTaps = 0;
            NTappedEvent?.Invoke(this, EventArgs.Empty);

            if (Command != null && Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);
        }

        protected override void ButtonPressed(object sender, EventArgs e)
        {
            CreateTimer();
            currentTaps = currentTaps + 1;

            if (currentTaps < Taps)
                return;
            
            DestroyTimer();
            TimerFinished();
        }

        protected override void TimerFinished()
        {
            if (currentTaps < Taps)
            {
                currentTaps = 0;
                return;
            }
            Device.BeginInvokeOnMainThread(OnNTapped);
        }
    }
}