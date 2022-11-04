using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Client
{
    public class FaceClient : ClientBase
    {
        
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        
        public FaceClient(Uri baseUrl, HttpClient httpClient, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (jsonSerializerSettings == null)
                jsonSerializerSettings = DefaultJsonSerializerSettingsFactory.BuildDefaultJsonSerializerSettings();
            this._jsonSerializerSettings = jsonSerializerSettings;
            this._httpClient = httpClient;
            this._httpClient.BaseAddress = baseUrl;
            this._httpClient.DefaultRequestHeaders.Add("X-Face-Dapp-Api-Hostname", Application.identifier);
            this._httpClient.DefaultRequestHeaders.Add("X-Face-Dapp-Api-Key", FaceSettings.Instance.ApiKey());
        }
        
        protected override async Task<RpcResponseMessage> SendAsync(
            RpcRequestMessage request,
            string route = null)
        {
            throw new NotImplementedException();
        }

        protected override Task<RpcResponseMessage[]> SendAsync(RpcRequestMessage[] requests)
        {
            // TODO: batch rpc sender will be implemented if needed
            throw new NotImplementedException();
        }


        private readonly HttpClient _httpClient;

        public async Task<FaceRpcResponse> SendRpcRequest(
            RpcRequestMessage request,
            string route = null)
        {
            return await this._sendRequestTemplate<RpcRequestMessage, FaceRpcResponse>(
                new RequestParameters<RpcRequestMessage>
                {
                    Request = request,
                    Callback = this._sendPostRequestCallback,
                    Route = route
                });
        }

        public async Task<R> SendHttpGetRequest<R>(string route = null)
        {
            return await this._sendRequestTemplate<object, R>(new RequestParameters<object>{
                Callback = (_, cancellationTokenSource, route) => this._sendGetRequestCallback(cancellationTokenSource, route),
                Route = route
            });
        }

        private async Task<HttpResponseMessage> _sendGetRequestCallback(CancellationTokenSource cancellationTokenSource, string route = null)
        {
            return await this._httpClient.GetAsync(route, cancellationTokenSource.Token).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> _sendPostRequestCallback<T>(T request, CancellationTokenSource cancellationTokenSource, string route = null)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject((object) request, this._jsonSerializerSettings), Encoding.UTF8, "application/json");
            
            return await this._httpClient.PostAsync(route, (HttpContent) content, cancellationTokenSource.Token).ConfigureAwait(false);
        }

        struct RequestParameters<T>
        {
            public T Request;
            public  Func<T, CancellationTokenSource, string, Task<HttpResponseMessage>> Callback;
            public  string Route;
        }

        private async Task<R> _sendRequestTemplate<T, R>(RequestParameters<T> parameters)
        {
            R responseMessage;

            try
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(ClientBase.ConnectionTimeout);
                HttpResponseMessage httpResponseMessage = await parameters.Callback(parameters.Request, cancellationTokenSource, parameters.Route);
                httpResponseMessage.EnsureSuccessStatusCode();
                using (StreamReader reader1 = new StreamReader(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false)))
                {
                    using (JsonTextReader reader2 = new JsonTextReader((TextReader)reader1))
                    {
                        responseMessage = JsonSerializer.Create(this._jsonSerializerSettings).Deserialize<R>((JsonReader)reader2);
                    }

                }
            }
            catch (TaskCanceledException ex)
            {
                throw new RpcClientTimeoutException(string.Format("Rpc timeout after {0} milliseconds", (object)ClientBase.ConnectionTimeout.TotalMilliseconds), (System.Exception)ex);
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Failed to get response from server", ex);
            }
            catch (System.Exception ex)
            {
                throw new RpcClientUnknownException("Error occurred when trying to send rpc requests(s)", ex);
            }
            
            return responseMessage;
        }
    }
}