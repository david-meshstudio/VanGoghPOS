using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;

namespace COM.MeshStudio.Lib.BasicComponent
{
    public class HttpServer
    {
        private HttpListener server;
        private Thread thread;
        public delegate string GetResponse(string path, string query);
        public GetResponse getResponse;

        public HttpServer()
        {
            server = new HttpListener();
            server.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            server.Prefixes.Add("http://localhost:8090/");
            server.Prefixes.Add("http://127.0.0.1:8090/");
        }

        public void Start()
        {
            server.Start();
            thread = new Thread(new ThreadStart(delegate
            {
                try
                {
                    loop(server);
                }
                catch (Exception exp)
                {
                    server.Stop();
                }
            }));
            thread.Start();
        }

        public void Stop()
        {
            server.Stop();
            thread.Abort();
        }

        private string GetServletResponse(string path, string query)
        {
            return getResponse(path, query);
        }

        public void loop(HttpListener server)
        {
            while (true)
            {
                HttpListenerContext context = server.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                Servlet servlet = new Servlet();
                servlet.getResponse = GetServletResponse;
                servlet.onCreate();
                if (request.HttpMethod == "POST")
                {
                    servlet.onPost(request,response);
                }
                else if (request.HttpMethod == "GET")
                {
                    servlet.onGet(request, response);
                }
            }
        }

        public class Servlet
        {
            public delegate string GetResponse(string api, string query);
            public GetResponse getResponse;

            public void onCreate()
            {
                getResponse("predefined_create", "");
            }

            public void onGet(HttpListenerRequest request, HttpListenerResponse response)
            {
                string ret = getResponse(request.Url.LocalPath, request.Url.Query);
                byte[] res = Encoding.UTF8.GetBytes(ret);
                response.OutputStream.Write(res, 0, res.Length);
                response.Close();
            }

            public void onPost(HttpListenerRequest request, HttpListenerResponse response)
            {
                string ret = getResponse(request.Url.LocalPath, request.Url.Query);
                byte[] res = Encoding.UTF8.GetBytes(ret);
                response.OutputStream.Write(res, 0, res.Length);
                response.Close();
            }
        }
    }
}
