using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace CommonClass
{
    public class FtpSocketStream:Stream,IDisposable
    {
        protected NetworkStream _netStream;

        protected Socket _socket;

        protected int _timeOut = 1000;

        public FtpSocketStream()
        {
        }

        public void Connect(IPAddress ipAddress, int port)
        {
            _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //_socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.KeepAlive, true);
            _socket.Connect(ipAddress, port);
            if (_socket == null || !_socket.Connected)
            {
                this.Close();
            }
            else
            {
                _netStream = new NetworkStream(_socket);
            }
        }

        public void Connect(string host, int port)
        {
            IPAddress[] hostAddresses = Dns.GetHostAddresses(host);
            Connect(hostAddresses[0], port);
        }

        public string ReadLine(Encoding encoding)
        {
            byte[] buffer = new byte[1];
            List<byte> lstBytes = new List<byte>();
            string str = null;
            while (this.Read(buffer, 0, buffer.Length) > 0)
            {
                lstBytes.Add(buffer[0]);
                if (buffer[0] == 10)
                {
                    str = encoding.GetString(lstBytes.ToArray()).TrimEnd(new char[] { '\r', '\n' });
                    break;
                }
            }
            return str;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _netStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _netStream.Write(buffer, offset, count);
        }

        public void WriteLine(Encoding encoding, string str)
        {
            byte[] buffer = encoding.GetBytes(string.Format("{0}\r\n", str));
            this.Write(buffer, 0, buffer.Length);
        }

        public override void Close()
        {
            if (_netStream != null)
            {
                _netStream.Close();
            }
            if (_socket != null)
            {
                _socket.Close();
            }
            base.Close();
        }

        public void Dispose()
        {
            if (_netStream != null)
            {
                _netStream.Dispose();
            }
            if (_socket != null)
            {
                _socket.Dispose();
            }
            base.Dispose();
        }

        public override bool CanRead
        {
            get { return _netStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _netStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _netStream.CanRead; }
        }

        public override void Flush()
        {
            _netStream.Flush();
        }

        public override long Length
        {
            get { return _netStream.Length; }
        }

        public override long Position
        {
            get
            {
                return _netStream.Position;
            }
            set
            {
                _netStream.Position = value;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
           return _netStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _netStream.SetLength(value);
        }
    }
}
