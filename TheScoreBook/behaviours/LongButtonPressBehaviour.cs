using System;
using Xamarin.Forms;

// Magic Long press code from: https://stackoverflow.com/questions/43569515/how-to-make-long-press-gesture-in-xamarin-forms

namespace TheScoreBook.behaviours
{
    public class LongButtonPressBehaviour : TimedButtonBehaviour<LongButtonPressBehaviour>
    {
        private volatile bool isReleased;

        public LongButtonPressBehaviour() : base(1000)
        {
            isReleased = true;
        }

        public LongButtonPressBehaviour(int duration) : base(duration) => isReleased = true;

        public event EventHandler LongPressEvent;

        protected override void OnAttachedTo(Button button)
        {
            base.OnAttachedTo(button);
            button.Released += ButtonReleased;
        }

        protected override void OnDetachingFrom(Button button)
        {
            base.OnDetachingFrom(button);
            button.Released -= ButtonReleased;
        }

        protected override void ButtonPressed(object sender, EventArgs e)
        {
            isReleased = false;
            base.ButtonPressed(sender, e);
        }

        protected void ButtonReleased(object sender, EventArgs e)
        {
            isReleased = true;
            DestroyTimer();
        }

        protected override void TimerFinished()
        {
            if (isReleased)
                return;
            
            Device.BeginInvokeOnMainThread(OnLongPressed);
        }

        protected virtual void OnLongPressed()
        {
            LongPressEvent?.Invoke(this, EventArgs.Empty);

            if (Command != null && Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);
        }
    }
}