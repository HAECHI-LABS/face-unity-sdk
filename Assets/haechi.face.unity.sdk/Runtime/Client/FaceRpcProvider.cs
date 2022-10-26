using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Contract;
using haechi.face.unity.sdk.Runtime.Exception;
using haechi.face.unity.sdk.Runtime.Module;
using haechi.face.unity.sdk.Runtime.Webview;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using Nethereum.Contracts.Services;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.Unity.Rpc;
using Newtonsoft.Json;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Client
{
    public class FaceRpcProvider : ClientBase, IUnityRpcRequestClient
    {
        private readonly SafeWebviewController _webview;

        private readonly FaceClient _client;

        private readonly MethodHandlers _methodHandlers;

        private readonly IRequestSender _defaultRequestSender;
        
        public FaceRpcProvider(SafeWebviewController safeWebviewController, Uri uri, IWallet wallet)
        {
            this._webview = safeWebviewController;
            this._client = new FaceClient(uri, new HttpClient());
            this._methodHandlers = new MethodHandlers(this, wallet, this._webview);
            this._defaultRequestSender = new WebviewRequestSender(this, this._webview);
            this.JsonSerializerSettings = DefaultJsonSerializerSettingsFactory.BuildDefaultJsonSerializerSettings();
        }
        
        public JsonSerializerSettings JsonSerializerSettings { get; }
        public System.Exception Exception { get; set; }
        public RpcResponseMessage Result { get; set; }
        
        public IEnumerator SendRequest(RpcRequest request)
        {
            yield return this.SendAsync(new RpcRequestMessage(request.Id, request.Method, request.RawParameters));
        }

        protected override async Task<RpcResponseMessage> SendAsync(RpcRequestMessage request, string route = null)
        {
            if (!this._methodHandlers.TryGetRequestSender(request.Method, out IRequestSender sender))
            {
                return await this._defaultRequestSender.SendRequest(request);
            }
            
            return await sender.SendRequest(request);
        }

        protected override Task<RpcResponseMessage[]> SendAsync(RpcRequestMessage[] request)
        {
            // TODO: batch rpc sender will be implemented if needed
            throw new NotImplementedException();
        }

        internal async Task<FaceRpcResponse> SendFaceRpcAsync<TParams>(FaceRpcRequest<TParams> request)
        {
            return (FaceRpcResponse)await this.SendAsync(request);
        }
        
        private interface IRequestSender
        {
            Task<RpcResponseMessage> SendRequest(RpcRequestMessage request);
        }
    
        private class MethodHandlers
        {
            private readonly Dictionary<FaceRpcMethod, IRequestSender> _senders;

            public MethodHandlers(FaceRpcProvider provider, IWallet wallet, SafeWebviewController webview)
            {
                this._senders = new Dictionary<FaceRpcMethod, IRequestSender>
                {
                    {FaceRpcMethod.face_logInSignUp, new WebviewRequestSender(provider, webview)},
                    {FaceRpcMethod.face_logOut, new WebviewRequestSender(provider, webview)},
                    {FaceRpcMethod.eth_sendTransaction, new WebviewRequestSender(provider, webview)},
                    {FaceRpcMethod.personal_sign, new WebviewRequestSender(provider, webview)},
                    
                    {FaceRpcMethod.eth_call, new ServerRequestSender(provider)},
                    {FaceRpcMethod.eth_getBalance, new ServerRequestSender(provider)},
                    
                    {FaceRpcMethod.eth_estimateGas, new EstimateGasServerRequestSender(provider, wallet)},
                    // ...
                };
            }

            public bool TryGetRequestSender(string methodValue, out IRequestSender sender)
            {
                if (!FaceRpcMethods.Contains(methodValue))
                {
                    sender = null;
                    return false;
                }

                return this._senders.TryGetValue(FaceRpcMethods.ValueOf(methodValue), out sender);
            }
        }
        
        private class WebviewRequestSender : IRequestSender
        {
            private readonly FaceRpcProvider _provider;
            private readonly SafeWebviewController _webview;
            public WebviewRequestSender(FaceRpcProvider provider, SafeWebviewController webview)
            {
                this._provider = provider;
                this._webview = webview;
            }
            
            public async Task<RpcResponseMessage> SendRequest(RpcRequestMessage request)
            {
                TaskCompletionSource<FaceRpcResponse> promise = new TaskCompletionSource<FaceRpcResponse>();

                void OnCloseWebview(SafeWebviewController _, CloseWebviewArgs args)
                {
                    promise.TrySetResult(args.Response);
                    this._webview.OnCloseWebview -= OnCloseWebview;
                }

                this._webview.OnCloseWebview += OnCloseWebview;
                this._provider._webview.SendMessage(request, response => promise.TrySetResult(response));
                
                return await promise.Task;
            }
        }

        private class ServerRequestSender : IRequestSender
        {
            private readonly FaceRpcProvider _provider;
            public ServerRequestSender(FaceRpcProvider provider)
            {
                this._provider = provider;
            }

            public virtual async Task<RpcResponseMessage> SendRequest(RpcRequestMessage request)
            {
                TaskCompletionSource<RpcResponseMessage> promise = new TaskCompletionSource<RpcResponseMessage>();
                FaceRpcResponse response = await this._provider._client.SendRpcRequest(request, "/api/v1/rpc");
                promise.TrySetResult(response);
                return await promise.Task;
            }
        }

        private class EstimateGasServerRequestSender : ServerRequestSender
        {
            private readonly IWallet _wallet;
            private readonly ABIEncode _abiEncode = new ABIEncode();
            private readonly EthApiContractService _ethApiContractService = new EthApiContractService(null);
            
            public EstimateGasServerRequestSender(FaceRpcProvider provider, IWallet wallet) : base(provider)
            {
                this._wallet = wallet;
            }

            public override async Task<RpcResponseMessage> SendRequest(RpcRequestMessage request)
            {
                if (!this._isValidRequest(request))
                {
                    throw new InvalidRpcRequestException("Invalid eth_estimateGas params");
                }
                RawTransaction modifiedTransaction = await this._modifyTransactionValue(this._rawTransactionFromParams(request));
                FaceRpcRequest<RawTransaction> newRequest =
                    new FaceRpcRequest<RawTransaction>(FaceSettings.Instance.Blockchain(), FaceRpcMethod.eth_estimateGas, modifiedTransaction);
                return await base.SendRequest(newRequest);
            }

            private bool _isValidRequest(RpcRequestMessage request)
            {
                if (request.RawParameters.GetType() != typeof(object[]))
                {
                    return false;
                }

                object[] parameters = (object[])request.RawParameters;
                if (parameters.Length != 1)
                {
                    return false;
                }

                return parameters[0].GetType() == typeof(RawTransaction);
            }

            private RawTransaction _rawTransactionFromParams(RpcRequestMessage request)
            {
                return (RawTransaction)((object[])request.RawParameters)[0];
            }

            /// <summary>
            /// _modifyTransactionValue update transaction's value depending on the account's balance.
            ///  This is done for showing to the end-user the notification through Face Wallet SDK modal page.
            /// While sending transaction via Face Wallet, even if account has less balance than the value, it does not fail
            /// </summary>
            private async Task<RawTransaction> _modifyTransactionValue(RawTransaction transaction)
            {
                if (string.IsNullOrEmpty(transaction.from) && string.IsNullOrEmpty(transaction.data))
                {
                    return transaction;
                }
            
                if (this._isNativeTokenTransferTransaction(transaction))
                {
                    HexBigInteger balance = new HexBigInteger((await this._wallet.GetBalance(transaction.from)).CastResult<string>());
                    BigInteger diff = BigInteger.Subtract(balance.Value, new HexBigInteger(transaction.value));
                    if (diff.CompareTo(BigInteger.Zero) < 0)
                    {
                        transaction.value = "0x0";
                    }

                    return transaction;
                }

                Function transferFunction = this._ethApiContractService
                    .GetContract(Abi.transferOnlyABI, "ArbitraryAddress")
                    .GetFunction("transfer");
                List<ParameterOutput> decode = transferFunction.DecodeInput(transaction.data);
                if (!this._isContractTransferCallTransaction(decode))
                {
                    return transaction;
                }

                HexBigInteger balance2 = new HexBigInteger((await this._wallet.GetBalance(decode[0].Result.ToString())).CastResult<string>());
                BigInteger diff2 = BigInteger.Subtract(balance2.Value, new HexBigInteger(transaction.value));
                if (diff2.CompareTo(BigInteger.Zero) < 0)
                {
                    transaction.value = "0x0";
                    transaction.data = System.Text.Encoding.UTF8.GetString(this._abiEncode.GetABIEncoded(new TransferFunction
                    {
                        To = transaction.to,
                        Value = new HexBigInteger(transaction.value)
                    }));
                }
                return transaction;
            }

            private bool _isNativeTokenTransferTransaction(RawTransaction transaction)
            {
                return string.IsNullOrEmpty(transaction.data);
            }

            private bool _isContractTransferCallTransaction(IReadOnlyList<ParameterOutput> decodeData)
            {
                return !(decodeData.Count != 2 && decodeData[0].Result.GetType() != typeof(string) && decodeData[1].Result.GetType() != typeof(BigInteger));
            }
        }
    }
    
    

}