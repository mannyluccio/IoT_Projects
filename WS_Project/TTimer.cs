using System.Threading;


namespace WS_Project
{
    public class TTimer
    {

        private Timer _timer;

        public event TickEventHandler Tick;
        public delegate void TickEventHandler(object sender);

        private bool _isRunning;
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
        }

        public void Start(int interval)
        {
            _timer = new Timer(ConnectionControl, null, 0, interval);
            _isRunning = true;
        }

        public void Stop()
        {
            _isRunning = false;
            _timer.Dispose();
        }

        private void ConnectionControl(object stateInfo)
        {
            if (Tick != null)
            {
                Tick(this);
            }
        }
    }
}
