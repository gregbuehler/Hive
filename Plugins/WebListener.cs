using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Hive.Core;

namespace Hive.Plugins
{
    public class WebListener : Plugin
    {
        public override string Descripton => "Listen for events via http";

        public string Host { get; set; }
        public int Port { get; set; }
        public string Prefix { get; set; }

        public HttpListener listener { get; set; }

        public WebListener(string Name, Configuration Config) : base(Name, Config)
        {
            this.Host = Config.Options.GetValueOrDefault("host", "localhost");
            this.Port = Config.Options.GetValueOrDefault("port", 9999);
            this.Prefix = Config.Options.GetValueOrDefault("prefix", "/");

            this.listener = new HttpListener();
            this.listener.Prefixes.Add($"http://{Host}:{Port}/");
        }

        public override void Process(Event e)
        {
            Emit(e);
        }

        public override void Run()
        {
            var t = new Thread(Listen);
            t.IsBackground = true;
            t.Start();
        }

        private void Listen()
        {
            this.listener.Start();

            while (listener.IsListening)
            {
                var result = listener.BeginGetContext(RequestHandler, listener);
                result.AsyncWaitHandle.WaitOne();
            }
        }

        private void RequestHandler(IAsyncResult result)
        {
            var listener = (HttpListener)result.AsyncState;
            var context = listener.EndGetContext(result);
            var request = context.Request;
            var response = context.Response;
            var body = "";

            if (request.HttpMethod == "POST")
            {
                using (var sr = new StreamReader(request.InputStream))
                {
                    body = sr.ReadToEnd();
                }

                var e = new Event();
                e.Data.Add("_raw", body);
                Process(e);

                var buff = System.Text.Encoding.UTF8.GetBytes(body);

                response.StatusCode = 200;
            }
            else
            {
                response.StatusCode = 400;
            }

            response.Close();
        }
    }
}