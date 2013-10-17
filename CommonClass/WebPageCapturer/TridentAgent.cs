using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CommonClass
{
    public class TridentAgent : IWebPageCapturer
    {
        private Bitmap _image;

        private Uri _url;

        private byte[] _postData;

        private Dictionary<string, string> _headers;

        private int _delay;

        private bool _documentCompleted = false;

        private int _minBrowserWidth = 0;
        public int MinBrowserWidth
        {
            get { return _minBrowserWidth; }
            set { _minBrowserWidth = value; }
        }

        private int _minBrowserHeight = 0;
        public int MinBrowserHeight
        {
            get { return _minBrowserHeight; }
            set { _minBrowserHeight = value; }
        }

        public Bitmap Capture(string url, int delay, byte[] postData, Dictionary<string, string> headers)
        {
            this._url = new Uri(url);
            this._postData = postData;
            this._delay = delay;
            this._headers = headers;
            this._documentCompleted = false;
            Thread thread = new Thread(WebPageShoot);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return _image;
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string urlName, string cookieName, string cookieData);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetGetCookie(string urlName, string cookieName, StringBuilder cookieData, ref int dataSize);

        [DllImport("kernel32.dll")]
        public static extern Int32 GetLastError();  

        private void WebPageShoot()
        {
            try
            {
                WebBrowser webBrowser = new WebBrowser();
                webBrowser.ScrollBarsEnabled = false;
                webBrowser.ScriptErrorsSuppressed = true;
                webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);
                StringBuilder sb = new StringBuilder();
                if (_headers != null && _headers.Count > 0)
                {
                    foreach (var entry in _headers)
                    {
                        sb.Append(entry.Key).Append(": ").Append(entry.Value).Append("\r\n");
                    }
                }
                if (_headers != null && _headers.Keys.Contains("Cookie"))
                {
                    string urlRoot = _url.AbsoluteUri.Substring(0, _url.AbsoluteUri.Length - _url.PathAndQuery.Length);
                    if (!InternetSetCookie(urlRoot, null, _headers["Cookie"]))
                    {
                        throw new Exception("set cookie error,kernel32 error code " + GetLastError());
                    }
                    int dataSize = 1000;
                    StringBuilder sbCookie = new StringBuilder(1000);
                    if (!InternetGetCookie(urlRoot, null, sbCookie, ref dataSize))
                    {
                        throw new Exception("get cookie error,kernel32 error code " + GetLastError());
                    }
                }
                webBrowser.Navigate(_url, null, _postData, sb.ToString());
                while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
                webBrowser.Dispose();
            }
            catch
            {
            }
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                // DocumentCompleted will fire for each frame in the web page. 
                //if (!e.Url.Equals(_url))
                //{
                //    return;
                //}
                if (this._documentCompleted == true)
                {
                    return;
                }
                WebBrowser webBrowser = (WebBrowser)sender;
                webBrowser.ClientSize = new Size(Math.Max(_minBrowserWidth, webBrowser.Document.Body.ScrollRectangle.Width), Math.Max(_minBrowserHeight, webBrowser.Document.Body.ScrollRectangle.Bottom));
                webBrowser.BringToFront();
                _image = new Bitmap(webBrowser.Bounds.Width, webBrowser.Bounds.Height, PixelFormat.Format32bppArgb);
                if (_delay > 0)
                {
                    Thread.Sleep(_delay);
                }
                ((Control)webBrowser).DrawToBitmap(_image, webBrowser.Bounds);
                this._documentCompleted = true;
            }
            catch
            {
            }
        }
    }
}
