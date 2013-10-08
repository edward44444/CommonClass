using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace CommonClass
{
    public class FtpDataStream : FtpSocketStream
    {
        public event EventHandler Closed;

        public override void Close()
        {
            if (Closed != null)
            {
                Closed.Invoke(this, EventArgs.Empty);
            }
            base.Close();
        }
    }
}
