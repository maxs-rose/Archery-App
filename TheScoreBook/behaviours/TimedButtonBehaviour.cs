using System;
using System.Threading;

namespace TheScoreBook.behaviours
{
    public abstract class TimedButtonBehaviour<T> : CustomButtonBehaviour<T>
    {
        private readonly object syncObject = new();

        private Timer timer;

        protected TimedButtonBehaviour(int duration) => Duration = duration;
        private int Duration { get; }

        protected void DestroyTimer()
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

        protected override void ButtonPressed(object sender, EventArgs e)
        {
            CreateTimer();
        }

        protected void CreateTimer()
        {
            lock (syncObject)
            {
                timer = new Timer(TimerElapsed, null, Duration, Timeout.Infinite);
            }
        }

        private void TimerElapsed(object state)
        {
            DestroyTimer();
            TimerFinished();
        }

        protected abstract void TimerFinished();
    }
}