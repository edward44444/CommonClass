using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;

namespace CommonClass
{
    public class UploadFileInfo
    {
        public string FilePath { get; set; }

        public string ControlName { get; set; }

        public string ContentType { get; set; }
    }

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

        private bool _keepAlive;
        public bool KeepAlive
        {
            get { return _keepAlive; }
            set { _keepAlive = value; }
        }

        private string _referer;
        public string Referer
        {
            get { return _referer; }
            set { _referer = value; }
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
            if (_keepAlive == true) request.KeepAlive = _keepAlive;
            if (_porxy != null) request.Proxy = _porxy;
            if (!string.IsNullOrEmpty(_contentType)) request.ContentType = _contentType;
            if (!string.IsNullOrEmpty(_accept)) request.Accept = _accept;
            if (!string.IsNullOrEmpty(_referer)) request.Referer = _referer;
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

        private HttpWebRequest GetRequest(string uri, NameValueCollection form, NameValueCollection controlForm, UploadFileInfo[] files, CookieContainer cookieContainer)
        {
            string boundary = _contentType.Substring(_contentType.IndexOf("boundary=") + "boundary=".Length);
            byte[] boundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endLineBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            string formTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            string fileHeaderTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            HttpWebRequest request = GetRequest(uri, null, cookieContainer);
            request.SendChunked = false;
            request.ServicePoint.Expect100Continue = false;
            using (MemoryStream stream = new MemoryStream())
            {
                if (form != null)
                {
                    for (int i = 0; i < form.Keys.Count; i++)
                    {
                        string key = form.Keys[i];
                        string formItem = string.Format(formTemplate, key, form[key]);
                        byte[] formItemBytes = Encoding.UTF8.GetBytes(formItem);
                        stream.Write(formItemBytes, 0, formItemBytes.Length);
                    }
                }
                foreach (var file in files)
                {
                    string fileHeader = string.Format(fileHeaderTemplate, file.ControlName, Path.GetFileName(file.FilePath), file.ContentType);
                    byte[] fileHeaderBytes = Encoding.UTF8.GetBytes(fileHeader);
                    stream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
                    using (FileStream fs = new FileStream(file.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[1024];
                        int readBytes = 0;
                        while ((readBytes = fs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            stream.Write(buffer, 0, readBytes);
                        }
                    }
                }
                if (controlForm != null)
                {
                    for (int i = 0; i < controlForm.Keys.Count; i++)
                    {
                        string key = controlForm.Keys[i];
                        string formItem = string.Format(formTemplate, key, controlForm[key]);
                        byte[] formItemBytes = Encoding.UTF8.GetBytes(formItem);
                        stream.Write(formItemBytes, 0, formItemBytes.Length);
                    }
                }
                stream.Write(boundaryBytes, 0, boundaryBytes.Length);
                request.ContentLength = stream.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    stream.Position = 0;
                    stream.CopyTo(requestStream);
                }
            }
            return request;
        }

        public string GetResponse(string uri, byte[] postData = null, CookieContainer cookieContainer = null)
        {
            HttpWebRequest request = GetRequest(uri, postData, cookieContainer);
            return ExecResponseString(request);
        }

        private string ExecResponseString(HttpWebRequest request)
        {
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        public HttpWebResponse GetReponseRaw(string uri, byte[] postData = null, CookieContainer cookieContainer = null)
        {
            HttpWebRequest request = GetRequest(uri, postData, cookieContainer);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }

        public string UploadFile(string uri, NameValueCollection form, NameValueCollection controlForm, UploadFileInfo[] files, CookieContainer cookieContainer = null)
        {
            HttpWebRequest request = GetRequest(uri, form, controlForm, files, cookieContainer);
            return ExecResponseString(request);
        }
    }
}
