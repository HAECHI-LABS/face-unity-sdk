using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Client.WalletConnect;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;
using WalletConnectSharp.Network.Models;
using WalletConnectSharp.Sign.Models;
using WalletConnectSharp.Sign.Models.Engine;
using WalletConnectSharp.Sign.Models.Engine.Methods;
using WalletConnectSharpV1.Core.Models;
using WalletConnectSharpV1.Core.Models.Ethereum;

namespace haechi.face.unity.sdk.Runtime.Module
{
    public class WalletConnect
    {
        private readonly Wallet _wallet;
        private readonly WalletConnectClientSupplier _walletConnectClientSupplier;
        private readonly WalletConnectV1Client _walletConnectV1;
        private readonly WalletConnectV2Client _walletConnectV2;
        
        // Later, will add Aptos, Near, Solana in this array
        private readonly Blockchain[] unsupportedBlockchains = new Blockchain[] { };
        
        private static Regex WC_URI_V1_REGEX = new Regex(@"wc:([^@]+)@([^?]+)\?bridge=([^&])+&key=(\w+)");
        private static Regex WC_URI_V2_REGEX = new Regex(@"wc:([^@]+)@2\?relay-protocol=([^&])+&symKey=(\w+)");
        
        public WalletConnect(Wallet wallet)
        {
            this._wallet = wallet;
            this._walletConnectClientSupplier = new WalletConnectClientSupplier();
            this._walletConnectV1 = (WalletConnectV1Client) this._walletConnectClientSupplier.Supply(WalletConnectVersion.V1);
            this._walletConnectV2 = (WalletConnectV2Client) this._walletConnectClientSupplier.Supply(WalletConnectVersion.V2);
#if  !UNITY_WEBGL
            this._initWalletConnectV1();
            this._initWalletConnectV2();
#endif
        }
        
        private void _initWalletConnectV1()
        {
            this._registryWalletConnectV1Event();
        }
        
        private async void _initWalletConnectV2()
        {
            await this._walletConnectV2.Connect();
            this._registryWalletConnectV2Event();
        }

        /// <summary>
        /// Connect Face with Opensea via WalletConnect V1.
        /// Use WalletConnect V1 until 28th, June, 2023, or until OpenSea migrate to V2.
        /// </summary>
        /// <param name="address">Wallet address.</param>
        public async Task<DappMetadata> ConnectOpenSea(string address)
        { 
             string hostname = Profiles.IsMainNet(FaceSettings.Instance.Environment())
                ? "https://opensea.io/"
                : "https://testnets.opensea.io/";
             
             return await this.ConnectDappWithWalletConnectV1(address, "OpenSea", hostname);
        } 
        
        /// <summary>
        /// Connect Face with Dapp via WalletConnect V1.
        /// This version will be officially deprecated on 28th, June, 2023.
        /// See <a href="https://medium.com/walletconnect/weve-reset-the-clock-on-the-walletconnect-v1-0-shutdown-now-scheduled-for-june-28-2023-ead2d953b595">here</a>.
        /// </summary>
        /// <param name="address">wallet address to connect.</param>
        /// <param name="dappName">dapp name to connect.</param>
        /// <param name="dappUrl">dapp url to connect.</param>
        public async Task<DappMetadata> ConnectDappWithWalletConnectV1(string address, string dappName, string dappUrl)
        {
            try
            {
                return await _connectDappWithWalletConnect(address, dappName, dappUrl);
            } catch (PlatformNotSupportedException e)
            {
                return null;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }
        
        /// <summary>
        /// Connect Face with Dapp via WalletConnect V2.
        /// </summary>
        /// <param name="address">wallet address to connect.</param>
        /// <param name="dappName">dapp name to connect.</param>
        /// <param name="dappUrl">dapp url to connect.</param>
        public async Task<DappMetadata> ConnectDappWithWalletConnectV2(string address, string dappName, string dappUrl)
        {
            try
            {
                return await _connectDappWithWalletConnect(address, dappName, dappUrl);
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

        public async Task DisconnectWalletConnectV1()
        {
            await this._walletConnectV1.DisconnectIfSessionExist();
        }

        private async Task<DappMetadata> _connectDappWithWalletConnect(string address, string dappName,string dappUrl, bool invalid = false)
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
            WalletConnectVersion walletConnectVersion = getWalletConnectVersionByUri(wcUri);
            IWalletConnectClient walletConnectClient =
                this._walletConnectClientSupplier.Supply(walletConnectVersion);
            
            try
            {
                DappMetadata dappMetadata = await walletConnectClient.RequestPair(address, wcUri, 
                    async metadata => await this._confirmWalletConnectDapp(metadata), dappName);
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
            
            return await this._wallet.Provider.SendFaceRpcAsync(faceRpcRequest);
        }

        public async Task<FaceRpcResponse> _confirmWalletConnectDapp<T>(T dappMetadata)
        {
            FaceRpcRequest<T> faceRpcRequest = new FaceRpcRequest<T>(FaceSettings.Instance.Blockchain(),
                FaceRpcMethod.face_confirmWalletConnectDapp, dappMetadata);

            return await this._wallet.Provider.SendFaceRpcAsync(faceRpcRequest);
        }

        private void _registryWalletConnectV1Event()
        {
            this._walletConnectV1.OnTermSignRequest += async (topic, @event) =>
            {
                ClientMeta dappMetadata = this._walletConnectV1.Session.DappMetadata;
                FaceRpcResponse response = await this._signMessageWithMetadata(@event.Parameters[0], WcFaceMetadata.V1Converted(dappMetadata));
                NetworkMessage networkMessage = await this._walletConnectV1.Session.CreateNetworkMessage(
                    new WcConnectRequest<string>(@event.ID, response.Result.ToString()),
                    this._walletConnectV1.Session.DappPeerId,
                    "pub",
                    false);
                await this._walletConnectV1.Session.SendRequest(networkMessage);
#if UNITY_IOS
                await this._walletConnectV1.Session.Transport.Open(this._walletConnectV1.Session.Transport.URL, false);
                await this._walletConnectV1.Session.SendRequest(networkMessage);
                this._walletConnectV1.TermSignNetworkMessageQueue
                    .Enqueue(new Dictionary<DateTime, NetworkMessage> {{DateTime.Now, networkMessage}});
#endif
            };
            this._walletConnectV1.OnPersonalSignRequest += async (topic, @event) =>
            {
                ClientMeta dappMetadata = this._walletConnectV1.Session.DappMetadata;
                FaceRpcResponse response = await this._signMessageWithMetadata(@event.Parameters[0], WcFaceMetadata.V1Converted(dappMetadata));
                await this._walletConnectV1.Session.SendPersonalSignRequest(@event.ID, response.Result.ToString());
#if UNITY_IOS
                await this._walletConnectV1.Session.Transport.Open(this._walletConnectV1.Session.Transport.URL, false);
                await this._walletConnectV1.Session.SendPersonalSignRequest(@event.ID, response.Result.ToString());
#endif
            };
            this._walletConnectV1.OnSendTransactionEvent += async (topic, @event) =>
            {
                TransactionData transactionData = @event.Parameters[0];
                TransactionRequestId response = await this._wallet.SendTransaction(new RawTransaction(transactionData.from, transactionData.to, transactionData.value, transactionData.data));
                Debug.Log(response.transactionId);
                await this._walletConnectV1.Session.SendTransactionRequest(@event.ID, response.transactionId);
#if UNITY_IOS
                await this._walletConnectV1.Session.Transport.Open(this._walletConnectV1.Session.Transport.URL, false);
                await this._walletConnectV1.Session.SendTransactionRequest(@event.ID, response.transactionId);
#endif
            };
        }

        private void _registryWalletConnectV2Event()
        {
            this._walletConnectV2.OnPersonalSignRequest += async (topic, @event) =>
            {
                Metadata dappMetadata = _walletConnectV2.Client.Session.Get(topic).Peer.Metadata;
                FaceRpcResponse response = await this._signMessageWithMetadata(@event.Params.Request.Params[0], WcFaceMetadata.V2Converted(dappMetadata));
                await _walletConnectV2.Client.Respond<SessionRequest<string[]>, string>(new RespondParams<string>()
                {
                    Topic = topic,
                    Response = new JsonRpcResponse<string>()
                    {
                        Id = @event.Id,
                        Result = response.Result.ToString(),
                        Error = null
                    }
                });
            };
            this._walletConnectV2.OnSendTransactionEvent += async (topic, @event) =>
            {
                TransactionRequestId response = await this._wallet.SendTransaction(@event.Params.Request.Params[0]);
                await _walletConnectV2.Client.Respond<SessionRequest<string[]>, string>(new RespondParams<string>()
                {
                    Topic = topic,
                    Response = new JsonRpcResponse<string>()
                    {
                        Id = @event.Id,
                        Result = response.transactionId,
                        Error = null
                    }
                });
            };
        }
        
        private async Task<FaceRpcResponse> _signMessageWithMetadata(string message, WcFaceMetadata metadata)
        {
            WcFaceRpcRequest<string> rpcRequest = 
                new WcFaceRpcRequest<string>(FaceSettings.Instance.Blockchain(), 
                    FaceRpcMethod.personal_sign,
                    metadata,
                    message);
            return await this._wallet.Provider.SendFaceRpcAsync(rpcRequest);
        }

        private WalletConnectVersion getWalletConnectVersionByUri(string wcUri)
        {
            if (WC_URI_V1_REGEX.IsMatch(wcUri))
            {
                return WalletConnectVersion.V1;
            } 
            else if (WC_URI_V2_REGEX.IsMatch(wcUri))
            {
                return WalletConnectVersion.V2;
            }
            else
            {
                throw new NotSupportedException("Given uri does not match with any of WalletConnect version");
            }
        }
    }
}