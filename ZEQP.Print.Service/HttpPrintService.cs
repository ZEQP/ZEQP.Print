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
using System.Data;

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
                var model = this.GetPrintModel(request);
                var dicJson = model.ToJson();
                this.Log.Info(dicJson);

                var printDoc = new MergeDocument(model);
                var xpsStream = printDoc.MergeToStream();
                if (model.Action == PrintActionType.Print)
                {
                    for (int i = 0; i < model.Copies; i++)
                    {
                        xpsStream.Seek(0, SeekOrigin.Begin);
                        XpsPrintHelper.Print(xpsStream, model.PrintName, $"XPS_{i}_{DateTime.Now.ToString("yyMMddHHmmssfff")}", false);
                    }
                    var resBytes = Encoding.UTF8.GetBytes(dicJson);
                    response.ContentType = "application/json";
                    response.StatusCode = 200;
                    response.ContentLength64 = resBytes.Length;
                    response.ContentEncoding = Encoding.UTF8;
                    response.OutputStream.Write(resBytes, 0, resBytes.Length);
                }
                if (model.Action == PrintActionType.File)
                {
                    response.ContentType = "application/vnd.ms-xpsdocument";
                    response.AddHeader("Content-Disposition", $"attachment; filename=XPS_{DateTime.Now.ToString("yyMMddHHmmssfff")}.xps");
                    response.StatusCode = 200;
                    response.ContentLength64 = xpsStream.Length;
                    //response.ContentEncoding = Encoding.UTF8;
                    xpsStream.Seek(0, SeekOrigin.Begin);
                    xpsStream.CopyTo(response.OutputStream);
                }
                if (model.Action == PrintActionType.PrintAndFile)
                {
                    for (int i = 0; i < model.Copies; i++)
                    {
                        xpsStream.Seek(0, SeekOrigin.Begin);
                        XpsPrintHelper.Print(xpsStream, model.PrintName, $"XPS_{i}_{DateTime.Now.ToString("yyMMddHHmmssfff")}", false);
                    }
                    response.ContentType = "application/vnd.ms-xpsdocument";
                    response.AddHeader("Content-Disposition", $"attachment; filename=XPS_{DateTime.Now.ToString("yyMMddHHmmssfff")}.xps");
                    response.StatusCode = 200;
                    response.ContentLength64 = xpsStream.Length;
                    //response.ContentEncoding = Encoding.UTF8;
                    xpsStream.Seek(0, SeekOrigin.Begin);
                    xpsStream.CopyTo(response.OutputStream);
                }
                xpsStream.Dispose();
                printDoc.Dispose();
            }
            catch (Exception ex)
            {
                this.Log.Error(ex.Message, ex);
                var json = ex.ToJson();
                var bytes = Encoding.UTF8.GetBytes(json);
                response.ContentType = "application/json";
                response.StatusCode = 500;
                response.ContentLength64 = bytes.Length;
                response.ContentEncoding = Encoding.UTF8;
                response.OutputStream.Write(bytes, 0, bytes.Length);
            }
            response.OutputStream.Flush();
            response.OutputStream.Close();
        }

        public bool Start(HostControl hostControl)
        {
            try
            {
                this.Listener.Start();
                this.Listener.BeginGetContext(new AsyncCallback(BeginRequestCallback), this.Listener);
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

        private PrintModel GetPrintModel(HttpListenerRequest request)
        {
            var result = new PrintModel();
            result.PrintName = ConfigurationManager.AppSettings["PrintName"];
            result.Template = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "Default.docx");
            result.Action = PrintActionType.Print;

            var query = request.Url.Query;
            var dicQuery = this.ToNameValueDictionary(query);
            if (dicQuery.ContainsKey("PrintName")) result.PrintName = dicQuery["PrintName"];
            if (dicQuery.ContainsKey("Copies")) result.Copies = int.Parse(dicQuery["Copies"]);
            if (dicQuery.ContainsKey("Template"))
            {
                var tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", dicQuery["Template"]);
                if (File.Exists(tempPath))
                    result.Template = tempPath;
            }
            if (dicQuery.ContainsKey("Action")) result.Action = (PrintActionType)Enum.Parse(typeof(PrintActionType), dicQuery["Action"]);

            foreach (var item in dicQuery)
            {
                if (item.Key.StartsWith("Image:"))
                {
                    var keyName = item.Key.Replace("Image:", "");
                    if (result.ImageContent.ContainsKey(keyName)) continue;
                    var imageModel = item.Value.ToObject<ImageContentModel>();
                    result.ImageContent.Add(keyName, imageModel);
                    continue;
                }
                if (item.Key.StartsWith("Table:"))
                {
                    var keyName = item.Key.Replace("Table:", "");
                    if (result.TableContent.ContainsKey(keyName)) continue;
                    var table = item.Value.ToObject<DataTable>();
                    table.TableName = keyName;
                    result.TableContent.Add(keyName, table);
                    continue;
                }
                if (result.FieldCotent.ContainsKey(item.Key)) continue;
                result.FieldCotent.Add(item.Key, item.Value);
            }

            if (request.HttpMethod.Equals("POST", StringComparison.CurrentCultureIgnoreCase))
            {
                var body = request.InputStream;
                var encoding = Encoding.UTF8;
                var reader = new StreamReader(body, encoding);
                var bodyContent = reader.ReadToEnd();
                var bodyModel = bodyContent.ToObject<Dictionary<string, object>>();
                foreach (var item in bodyModel)
                {
                    if (item.Key.StartsWith("Image:"))
                    {
                        var imageModel = item.Value.ToJson().ToObject<ImageContentModel>();
                        var keyName = item.Key.Replace("Image:", "");
                        if (result.ImageContent.ContainsKey(keyName))
                            result.ImageContent[keyName] = imageModel;
                        else
                            result.ImageContent.Add(keyName, imageModel);
                        continue;
                    }
                    if (item.Key.StartsWith("Table:"))
                    {
                        var table = item.Value.ToJson().ToObject<DataTable>();
                        var keyName = item.Key.Replace("Table:", "");
                        table.TableName = keyName;
                        if (result.TableContent.ContainsKey(keyName))
                            result.TableContent[keyName] = table;
                        else
                            result.TableContent.Add(keyName, table);
                        continue;
                    }
                    if (result.FieldCotent.ContainsKey(item.Key))
                        result.FieldCotent[item.Key] = HttpUtility.UrlDecode(item.Value.ToString());
                    else
                        result.FieldCotent.Add(item.Key, HttpUtility.UrlDecode(item.Value.ToString()));
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
        #endregion
    }
}
