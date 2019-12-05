using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Logging;
using ZEQP.Print.Framework;
using Newtonsoft.Json;
using System.IO;

namespace ZEQP.Print.Service
{
    public class HttpPrintService : ServiceControl
    {
        public LogWriter Log { get; set; }
        public HttpListener Listener { get; set; }
        public HttpPrintService()
        {
            this.Log = HostLogger.Get<HttpPrintService>();
            this.Log.Info("HttpPrintService初始化");
            var host = ConfigurationManager.AppSettings["Host"];
            var port = ConfigurationManager.AppSettings["Port"];
            var prefix = $"http://{host}:{port}/api/print/";
            this.Log.Info($"Prefixes：{prefix}");
            this.Listener = new HttpListener();
            this.Listener.Prefixes.Add(prefix);

        }
        protected void BeginRequestCallback(IAsyncResult result)
        {
            var context = this.Listener.EndGetContext(result);
            this.Listener.BeginGetContext(new AsyncCallback(BeginRequestCallback), this.Listener);
            this.RequextAction(context);
        }
        private void RequextAction(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            try
            {
                this.Log.Info(request.Url);
                var dic = this.GetContent(request);
                this.Print(dic);
                var dicJson = dic.ToJson();
                this.Log.Info(dicJson);
                var resBytes = Encoding.UTF8.GetBytes(dicJson);
                response.StatusCode = 200;
                response.ContentLength64 = resBytes.Length;
                response.ContentEncoding = Encoding.UTF8;
                response.OutputStream.Write(resBytes, 0, resBytes.Length);
            }
            catch (Exception ex)
            {
                this.Log.Error(ex.Message, ex);
                var json = ex.ToJson();
                var bytes = Encoding.UTF8.GetBytes(json);
                response.StatusCode = 500;
                response.ContentLength64 = bytes.Length;
                response.ContentEncoding = Encoding.UTF8;
                response.OutputStream.Write(bytes, 0, bytes.Length);
            }
            response.ContentType = "application/json";
            response.OutputStream.Flush();
            response.OutputStream.Close();
        }
        private void Print(Dictionary<string, string> content)
        {
            var model = this.ToPrintModel(content);
            using (var printDoc = new PrintDocument(model))
            {
                printDoc.Print();
            }
        }

        public bool Start(HostControl hostControl)
        {
            try
            {
                this.Listener.Start();
                var result = this.Listener.BeginGetContext(new AsyncCallback(BeginRequestCallback), this.Listener);
                this.Log.Info($"服务已启动");
                return true;
            }
            catch (Exception ex)
            {
                this.Log.Info($"服务启动失败");
                this.Log.Error(ex.Message, ex);
                return false;
            }

        }

        public bool Stop(HostControl hostControl)
        {
            try
            {
                this.Listener.Stop();
                this.Log.Info($"服务已停止");
                return true;
            }
            catch (Exception ex)
            {
                this.Log.Info($"服务停止出错");
                this.Log.Error(ex.Message, ex);
                return false;
            }
        }
        #region Helper
        private Dictionary<string, string> GetContent(HttpListenerRequest request)
        {
            var result = new Dictionary<string, string>();
            var method = request.HttpMethod;
            var query = request.Url.Query;
            var dicQuery = this.ToNameValueDictionary(query);
            foreach (var item in dicQuery)
            {
                result.Add(item.Key, item.Value);
            }
            if (method.Equals("POST", StringComparison.CurrentCultureIgnoreCase))
            {
                var body = request.InputStream;
                //var encoding = context.Request.ContentEncoding;
                var encoding = Encoding.UTF8;
                var reader = new System.IO.StreamReader(body, encoding);
                var bodyContent = reader.ReadToEnd();
                if (request.ContentType.IndexOf("application/x-www-form-urlencoded") >= 0)
                {
                    var dicBody = this.ToNameValueDictionary(bodyContent);
                    foreach (var item in dicBody)
                    {
                        result.Add(item.Key, item.Value);
                    }
                }
                else if (request.ContentType.IndexOf("application/json") >= 0)
                {
                    var model = JsonConvert.DeserializeObject<Dictionary<string, object>>(bodyContent);
                    foreach (var item in model)
                    {
                        result.Add(item.Key, item.Value.ToString());
                    }
                }
            }
            return result;
        }
        public Dictionary<string, string> ToNameValueDictionary(string source)
        {
            var dic = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(source))
            {
                if (source.StartsWith("?")) source = source.Substring(1);
                foreach (var p in source.Split('&'))
                {
                    var s = p.Split(new char[] { '=' }, StringSplitOptions.None);
                    if (s.Length == 2)
                    {
                        var key = HttpUtility.UrlDecode(s[0]);
                        var value = HttpUtility.UrlDecode(s[1]);
                        value = HttpUtility.UrlDecode(value);
                        dic.Add(key, value);
                    }
                }
            }
            return dic;
        }
        public PrintModel ToPrintModel(Dictionary<string, string> dic)
        {
            var result = new PrintModel();
            result.Template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "Default.docx");
            foreach (var item in dic)
            {
                if (item.Key.Equals("PrintName", StringComparison.CurrentCultureIgnoreCase))
                {
                    result.PrintName = item.Value;
                    continue;
                }
                if (item.Key.Equals("Copies", StringComparison.CurrentCultureIgnoreCase))
                {
                    result.Copies = int.Parse(item.Value);
                    continue;
                }
                if (item.Key.Equals("Template", StringComparison.CurrentCultureIgnoreCase))
                {
                    var tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", $"{item.Value}.docx");
                    if (File.Exists(tempPath))
                        result.Template = tempPath;
                    continue;
                }
                result.DicCotent.Add(item.Key, item.Value);
            }
            return result;
        }
        #endregion
    }
}
