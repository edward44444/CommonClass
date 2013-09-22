using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace CommonClass
{
    public enum BrowserType
    {
        Trident = 0
    }

    public class WebPageCapturer
    {
        private IWebPageCapturer _capturer;

        public WebPageCapturer(int minBrowserWidth, int minBrowserHeight, BrowserType browserType = BrowserType.Trident)
            : this(browserType)
        {
            _capturer.MinBrowserWidth = minBrowserWidth;
            _capturer.MinBrowserHeight = minBrowserHeight;
        }

        public WebPageCapturer(BrowserType browserType = BrowserType.Trident)
        {
            switch (browserType)
            {
                case BrowserType.Trident:
                    _capturer = new TridentAgent();
                    break;
                default:
                    _capturer = new TridentAgent();
                    break;
            }
        }

        public Bitmap Capture(string url,int delay=0, byte[] postData=null, Dictionary<string,string> headers=null)
        {
            return _capturer.Capture(url, delay, postData, headers);
        }
    }
}
