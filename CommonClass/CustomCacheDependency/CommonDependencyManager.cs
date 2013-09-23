using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClass
{
    public abstract class CommonDependencyManager
    {
        private System.Threading.Timer _timer;

        private int _pollTime = 1000 * 10;
        public int PollTime
        {
            get { return _pollTime; }
            set
            {
                _pollTime = value;
                if (_timer != null)
                {
                    _timer.Change(_pollTime, _pollTime);
                }
            }
        }

        protected readonly object SyncObject = new object();

        public void InitialPolling()
        {
            if (_timer == null)
            {
                lock (SyncObject)
                {
                    if (_timer == null)
                    {
                        _timer = new System.Threading.Timer(PollForChanges, null, _pollTime, _pollTime);
                    }
                }
            }
        }

        public abstract void EnsureDependencyItemIsPooled();

        protected abstract void PollForChanges(object state);

        public string GetMoniterKey(string depKey)
        {
            return depKey + ":CacheDependencyManager." + this.GetType().Name;
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}
