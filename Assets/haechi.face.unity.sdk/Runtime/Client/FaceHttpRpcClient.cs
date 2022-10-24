using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Settings;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;

namespace haechi.face.unity.sdk.Runtime.Client
{
    public class FaceHttpRpcClient : ClientBase
    {
        
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        
        public FaceHttpRpcClient(Uri baseUrl, HttpClient httpClient, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (jsonSerializerSettings == null)
                jsonSerializerSettings = DefaultJsonSerializerSettingsFactory.BuildDefaultJsonSerializerSettings();
            this._jsonSerializerSettings = jsonSerializerSettings;
            this._httpClient = httpClient;
            this._httpClient.BaseAddress = baseUrl;
            this._httpClient.DefaultRequestHeaders.Add("X-Face-Dapp-Api-Hostname", "http://localhost:3000"); // TODO: FIXME
            this._httpClient.DefaultRequestHeaders.Add("X-Face-Dapp-Api-Key", FaceSettings.Instance.ApiKey);
        }
        
        protected override async Task<RpcResponseMessage> SendAsync(
            RpcRequestMessage request,
            string route = null)
        {
            RpcResponseMessage rpcResponseMessage;
            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject((object) request, this._jsonSerializerSettings), Encoding.UTF8, "application/json");
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(ClientBase.ConnectionTimeout);
                
                HttpResponseMessage httpResponseMessage = await this._httpClient.PostAsync(route, (HttpContent) content, cancellationTokenSource.Token).ConfigureAwait(false);
                httpResponseMessage.EnsureSuccessStatusCode();
                using (StreamReader reader1 = new StreamReader(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false)))
                {
                    using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
                        rpcResponseMessage = JsonSerializer.Create(this._jsonSerializerSettings).Deserialize<RpcResponseMessage>((JsonReader) reader2);
                }
            }
            catch (TaskCanceledException ex)
            {
                throw new RpcClientTimeoutException(string.Format("Rpc timeout after {0} milliseconds", (object) ClientBase.ConnectionTimeout.TotalMilliseconds), (System.Exception) ex);
            }
            catch (System.Exception ex)
            {
                throw new RpcClientUnknownException("Error occurred when trying to send rpc requests(s): " + request.Method, ex);
            }
            return rpcResponseMessage;
        }

        protected override Task<RpcResponseMessage[]> SendAsync(RpcRequestMessage[] requests)
        {
            // TODO: batch rpc sender will be implemented if needed
            throw new NotImplementedException();
        }


        private readonly HttpClient _httpClient;

        public async Task<RpcResponseMessage> SendRequest(
            RpcRequestMessage request,
            string route = null)
        {
            return await SendAsync(request, route);
        }
    }
}