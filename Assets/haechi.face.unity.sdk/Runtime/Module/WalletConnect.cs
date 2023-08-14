using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Client.WalletConnect;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;
using WalletConnectSharp.Core.Models.Pairing;
using WalletConnectSharp.Network.Models;
using WalletConnectSharp.Sign.Models.Engine.Methods;

namespace haechi.face.unity.sdk.Runtime.Module
{
    public class WalletConnect
    {
        private readonly FaceRpcProvider _provider;
        private readonly Wallet _wallet;
        private readonly WalletConnectClient _walletConnectClient;
        
        // Later, will add Aptos, Near, Solana in this array
        private readonly Blockchain[] unsupportedBlockchains = new Blockchain[] { };
        
        private static Regex WC_URI_V2_REGEX = new Regex(@"wc:([^@]+)@2\?relay-protocol=([^&])+&symKey=(\w+)");
        
        public WalletConnect(FaceRpcProvider provider, Wallet wallet)
        {
            this._provider = provider;
            this._wallet = wallet;
            this._walletConnectClient = WalletConnectClient.GetInstance();
#if  !UNITY_WEBGL
            this._initWalletConnect();
#endif
        }
        
        private void _initWalletConnect()
        {
            this._registryWalletConnectEvent();
        }

        /// <summary>
        /// Connect Face with Opensea via WalletConnect V2.
        /// </summary>
        /// <param name="address">Wallet address.</param>
        public async Task ConnectOpenSea(string address)
        { 
             string hostname = FaceSettings.Instance.Environment().IsMainNet()
                ? "https://opensea.io/"
                : "https://testnets.opensea.io/";
             
             await this.ConnectDappWithWalletConnect(address, "OpenSea", hostname);
        } 
        
        /// <summary>
        /// Connect Face with Dapp via WalletConnect V2.
        /// </summary>
        /// <param name="address">wallet address to connect.</param>
        /// <param name="dappName">dapp name to connect.</param>
        /// <param name="dappUrl">dapp url to connect.</param>
        public async Task<DappMetadata> ConnectDappWithWalletConnect(string address, string dappName, string dappUrl)
        {
            try
            {
                return await this._connectDappWithWalletConnect(address, dappName, dappUrl);
            }
            catch (PlatformNotSupportedException e)
            {
                return null;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        private async Task<DappMetadata> _connectDappWithWalletConnect(string address, string dappName, string dappUrl, bool invalid = false)
        {
            if (unsupportedBlockchains.Contains(FaceSettings.Instance.Blockchain()))
            {
                throw new NotSupportedException();
            }
            FaceRpcResponse response = await this._openWalletConnect(dappName, dappUrl, invalid);
#if !UNITY_WEBGL
            string encodedWcUri = response.Result.Value<string>("uri");
            byte[] wcUriBytes = Convert.FromBase64String(encodedWcUri);
            string wcUri = Encoding.UTF8.GetString(wcUriBytes);
            this._validateWcUri(wcUri);
            try
            {
                // await this._walletConnectClient.Connect();
                DappMetadata dappMetadata = await this._walletConnectClient.RequestPair(
                    address, 
                    wcUri, 
                    async metadata => await this._confirmWalletConnectDapp(metadata));
                await this._walletConnectClient.HandleMessage();
                return dappMetadata;
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
                
                /*
                 * TO-BE-FIXED:
                 * This usually happens when session expired.
                 * Logout first, and then log in again.
                 * Later, this will be fixed if Auth().IsLoggedIn() method actually check the session from server.
                 */
                return await _connectDappWithWalletConnect(dappName, dappName, address, true);
            }
#endif
            throw new PlatformNotSupportedException("WebGL does not support _connectDappWithWalletConnect() method");
        }

        private async Task<FaceRpcResponse> _openWalletConnect(string dappName, string dappUrl, bool invalid = false)
        {
            FaceRpcRequest<string> faceRpcRequest = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(),
                FaceRpcMethod.face_openWalletConnect, dappName, dappUrl, (invalid ? "invalid" : null));
            
            return await this._provider.SendFaceRpcAsync(faceRpcRequest);
        }

        private async Task<FaceRpcResponse> _confirmWalletConnectDapp<T>(T dappMetadata)
        {
            FaceRpcRequest<T> faceRpcRequest = new FaceRpcRequest<T>(FaceSettings.Instance.Blockchain(),
                FaceRpcMethod.face_confirmWalletConnectDapp, dappMetadata);

            return await this._provider.SendFaceRpcAsync(faceRpcRequest);
        }

        private void _registryWalletConnectEvent()
        {
            this._walletConnectClient.OnPersonalSignRequest += async (topic, @event) =>
            {
                Metadata dappMetadata = this._walletConnectClient.SignClient.Session.Get(topic).Peer.Metadata;
                FaceRpcResponse response = await this._signMessageWithMetadata(@event.Params[0], WcFaceMetadata.Converted(dappMetadata));
                await this._walletConnectClient.SignClient.Respond<SessionRequest<string[]>, string>(
                    topic,
                    new JsonRpcResponse<string>() 
                    {
                        Id = @event.Id,
                        Result = response.Result.ToString(),
                        Error = null 
                    }
                );
            };
            this._walletConnectClient.OnSendTransactionEvent += async (topic, @event) =>
            {
                TransactionRequestId response = await this._wallet.SendTransaction(@event.Params[0]);
                await this._walletConnectClient.SignClient.Respond<SessionRequest<SendTransaction[]>, string>(
                    topic,
                    new JsonRpcResponse<string>()
                    {
                        Id = @event.Id,
                        Result = response.transactionId,
                        Error = null
                    }
                );
            };
        }
        
        private async Task<FaceRpcResponse> _signMessageWithMetadata(string message, WcFaceMetadata metadata)
        {
            WcFaceRpcRequest<string> rpcRequest = 
                new WcFaceRpcRequest<string>(FaceSettings.Instance.Blockchain(), 
                    FaceRpcMethod.personal_sign,
                    metadata,
                    message);
            return await this._provider.SendFaceRpcAsync(rpcRequest);
        }

        private void _validateWcUri(string wcUri)
        {
            if (!WC_URI_V2_REGEX.IsMatch(wcUri))
            {
                throw new NotSupportedException("Given uri does not match with WalletConnect uri regex");
            }
        }
    }
}