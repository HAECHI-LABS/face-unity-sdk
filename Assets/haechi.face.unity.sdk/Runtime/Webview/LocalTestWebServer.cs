using System;
using System.Net;
using System.Net.Sockets;

namespace haechi.face.unity.sdk.Runtime.Webview
{
    internal interface IURLHandler
    {
        void HandleUrl(Uri url);
    }
    
    internal class LocalTestWebServer
    {
        private readonly IURLHandler _urlHandler;
        
        private LocalTestWebServer(IURLHandler urlHandler)
        {
            this._urlHandler = urlHandler;
        }
        
        internal static string Start(IURLHandler urlHandler)
        {
             return new LocalTestWebServer(urlHandler).Start();
        }

        private string Start()
        {
            HttpListener httpListener = new HttpListener();
            string redirectUrl = $"http://localhost:{this.GetRandomUnusedPort()}";
            httpListener.Prefixes.Add($"{redirectUrl}/complete/");
            httpListener.Start();
            httpListener.BeginGetContext(this.HandleHttpRequest, httpListener);
            return $"{redirectUrl}/complete/";
        }
        
        private int GetRandomUnusedPort()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
        
        private void HandleHttpRequest(IAsyncResult result)
        {
            // get back the reference to our http listener
            HttpListener httpListener = (HttpListener)result.AsyncState;

            // fetch the context object
            HttpListenerContext httpContext = httpListener.EndGetContext(result);

            HttpListenerRequest httpRequest = httpContext.Request;
            HttpListenerResponse httpResponse = httpContext.Response;
            if (httpRequest.Url.LocalPath == "/complete/")
            {
                httpListener.BeginGetContext(this.HandleHttpRequest, httpListener);

                var responseString = @"
                <!DOCTYPE html>
                <html>
                <head>
                  <meta charset=""utf-8"">
                  <meta name=""viewport"" content=""width=device-width"">
                  <title>Local Face Wallet Iframe</title>
                  <link href=""https://fonts.googleapis.com/css2?family=DM+Sans:wght@500&display=swap"" rel=""stylesheet"">
                </head>
                <body style=""padding:0;margin:0;font-size:10pt;font-family: 'DM Sans', sans-serif;"">
                  <div style=""display:flex;align-items:center;justify-content:center;height:100vh;display: flex;"" id=""success"">
                    <div style=""text-align:center"">
                       <h2 style=""margin-bottom:0""> Local iframe successfully sent data to Unity SDK &#x1F680</h2>
                       <p> You can close this tab/window now and return to Unity Editor</p>
                    </div>
                  </div>
                </body>
                </html>
            ";

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                httpResponse.ContentLength64 = buffer.Length;
                System.IO.Stream output = httpResponse.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();

                if (!string.IsNullOrEmpty(httpRequest.QueryString.Get("response")) ||
                    !string.IsNullOrEmpty(httpRequest.QueryString.Get("request")))
                {
                    this._urlHandler.HandleUrl(httpRequest.Url);
                }

                httpListener.Close();
            }
        }
    }
}