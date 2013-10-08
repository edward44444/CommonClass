using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;

namespace CommonClass
{
    public class FtpClient:IDisposable
    {
        private string _host;

        private int _port;

        private FtpSocketStream _stream;

        private Encoding _encoding=Encoding.ASCII;

        private NetworkCredential _credential;
        public NetworkCredential Credential
        {
            get { return _credential; }
            set { _credential = value; }
        }

        public FtpClient(string host, int port)
        {
            this._host = host;
            this._port = port;
        }

        public void SetWorkingDirectory(string path)
        {
            FtpReply reply = Execute("CWD {0}",path);
        }

        public string GetWorkingDirectory()
        {
            FtpReply reply = Execute("PWD");
            Match match = Regex.Match(reply.Message, "\"(?<pwd>.*)\"");
            if (match.Success)
            {
                return match.Groups["pwd"].Value;
            }
            match = Regex.Match(reply.Message, "PWD = (?<pwd>.*)");
            if (match.Success)
            {
                return match.Groups["pwd"].Value;
            }
            return "./";
        }

        public List<string> ListDirectory(string path)
        {
            SetWorkingDirectory(path);
            List<string> lstDirectory = new List<string>();
            FtpDataStream socketData = OpenDataStream();
            FtpReply reply= Execute("LIST");
            string str;
            while (!string.IsNullOrEmpty(str = socketData.ReadLine(_encoding)))
            {
                lstDirectory.Add(str);
            }
            socketData.Close();
            return lstDirectory;
        }

        public Stream OpenRead(string path)
        {
            SetWorkingDirectory(path.Remove(path.LastIndexOf('/')));
            FtpReply reply1 = Execute("TYPE I");
            FtpDataStream socketData = OpenDataStream();
            FtpReply reply2 = Execute("RETR {0}", path.Substring(path.LastIndexOf('/')+1));
            return socketData;
        }

        private FtpDataStream OpenDataStream()
        {
            FtpReply reply1 = Execute("PASV");
            Match match = Regex.Match(reply1.Message, "(?<quad1>\\d+),(?<quad2>\\d+),(?<quad3>\\d+),(?<quad4>\\d+),(?<port1>\\d+),(?<port2>\\d+)");
            IPAddress ipAddress = IPAddress.Parse(match.Groups["quad1"].Value + "." + match.Groups["quad2"].Value + "." + match.Groups["quad3"].Value + "." + match.Groups["quad4"].Value);
            int port = (int.Parse(match.Groups["port1"].Value) << 8) + int.Parse(match.Groups["port2"].Value);
            FtpDataStream dataStream = new FtpDataStream();
            dataStream.Connect(ipAddress, port);
            dataStream.Closed += delegate
            {
                FtpReply reply2=ReadReply();
            };
            return dataStream;
        }

        private FtpReply Execute(string command)
        {
            _stream.WriteLine(_encoding, command);
            FtpReply reply = ReadReply();
            return reply;
        }

        private FtpReply Execute(string command,params object[] args)
        {
            return Execute(string.Format(command, args));
        }

        public FtpReply ReadReply()
        {
           FtpReply reply = new FtpReply();
           string strReply = _stream.ReadLine(_encoding);
           Match match = Regex.Match(strReply, "^(?<code>[0-9]{3}) (?<message>.*)$");
           if (match.Success)
           {
               reply.Code = match.Groups["code"].Value;
               reply.Message = match.Groups["message"].Value;
           }
           else
           {
               reply.Message = strReply;
           }
           return reply;
        }

        private void Authenticate()
        {
            if (_credential != null)
            {
                FtpReply reply1 = this.Execute("USER {0}",_credential.UserName);
                FtpReply reply2 = this.Execute("PASS {0}", _credential.Password);
            }
        }

        public void Close()
        {
            if (_stream != null)
            {
                _stream.Close();
            }
        }

        public void Connect()
        {
            _stream = new FtpSocketStream();
            _stream.Connect(_host,_port);
            FtpReply reply = ReadReply();
            if (reply.Code != "220")
            {
                throw new Exception();
            }
            Authenticate();
        }

        public void Dispose()
        {
            this.Close();
        }
    }
}
