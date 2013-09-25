using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace CommonClass
{
    public class HttpHelper
    {
        private string _method;
        public string Method 
        {
            get { return _method; }
            set { _method = value; }
        }

        private string _contentType;
        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        private string _accept;
        public string Accept
        {
            get { return _accept; }
            set { _accept = value; }
        }

        private Dictionary<string, string> _headers;
        public Dictionary<string, string> Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }

        private bool _allowAutoRedirect = true;
        public bool AllowAutoRedirect
        {
            get { return _allowAutoRedirect; }
            set { _allowAutoRedirect = value; }
        }

        private IWebProxy _porxy;
        public IWebProxy Porxy
        {
            get { return _porxy; }
            set { _porxy = value; }
        }


        public HttpHelper(string method, string contentType = null, string accept = null, Dictionary<string, string> headers = null)
        {
            this._method = method;
            this._contentType = contentType;
            this._accept = accept;
            this._headers = headers;
        }

        private HttpWebRequest GetRequest(string uri, byte[] postData, CookieContainer cookieContainer)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(uri));
            request.AllowAutoRedirect = _allowAutoRedirect;
            request.Method = _method;
            if (_porxy != null) request.Proxy = _porxy;
            if (!string.IsNullOrEmpty(_contentType)) request.ContentType = _contentType;
            if (!string.IsNullOrEmpty(_accept)) request.Accept = _accept;
            if (cookieContainer != null) request.CookieContainer = cookieContainer;
            if (_headers != null)
            {
                foreach (var entry in _headers)
                {
                    request.Headers.Add(entry.Key, entry.Value);
                }
            }
            if (postData != null)
            {
                request.ContentLength = postData.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                }
            }
            return request;
        }

        public string GetResponse(string uri, byte[] postData = null, CookieContainer cookieContainer = null)
        {
            HttpWebRequest request = GetRequest(uri, postData, cookieContainer);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                   return sr.ReadToEnd();
                }
            }
        }

        public HttpWebResponse GetReponseRaw(string uri, byte[] postData = null, CookieContainer cookieContainer = null)
        {
            HttpWebRequest request = GetRequest(uri, postData, cookieContainer);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }
    }
}
