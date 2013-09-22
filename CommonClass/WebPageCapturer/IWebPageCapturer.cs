using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CommonClass
{
    public interface IWebPageCapturer
    {
        Bitmap Capture(string url, int delay, byte[] postData, Dictionary<string, string> headers);

        int MinBrowserWidth { get; set; }

        int MinBrowserHeight { get; set; }
    }
}
