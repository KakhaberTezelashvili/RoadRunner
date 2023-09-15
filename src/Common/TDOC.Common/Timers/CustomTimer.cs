using System.Timers;

namespace TDOC.Common.Timers
{
    public class CustomTimer
    {
        private System.Timers.Timer _timer;

        private event Action _onElapsed;

        public void ExecActionAfterSomeDelay(Action action, double intervalMilliseconds = 1, bool repeatedly = false)
        {
            UnregisterAllHandlersForTimer();
            _onElapsed += action;
            SetTimer(intervalMilliseconds, repeatedly);
        }

        private void UnregisterAllHandlersForTimer()
        {
            if (_timer != null)
                _timer.Stop();
            if (_onElapsed == null)
                return;
            _onElapsed = null;
        }

        private void SetTimer(double intervalMilliseconds, bool repeatedly)
        {
            _timer = new System.Timers.Timer(intervalMilliseconds);
            _timer.Elapsed += NotifyTimerElapsed;
            _timer.AutoReset = repeatedly;
            _timer.Start();
        }

        private void NotifyTimerElapsed(object source, ElapsedEventArgs e)
        {
            _onElapsed?.Invoke();
            if (!_timer.AutoReset)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}