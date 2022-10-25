using System;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace haechi.face.unity.sdk.Samples.Script
{
    public class FaceUnity : MonoBehaviour
    {
        [SerializeField] internal DataDesignator dataDesignator;
        [SerializeField] internal InputDesignator inputDesignator;
        [SerializeField] internal Face face;
        [SerializeField] internal ActionQueue actionQueue;
        
        private void Start()
        {
            Application.targetFrameRate = 60;
        }
        
        public void InitializeFace()
        {
            this.face.Initialize(new FaceSettings.Parameters
            {
                ApiKey = this.inputDesignator.apiKey.text,
                Environment = this.inputDesignator.profileDrd.captionText.text,
                Blockchain = this.inputDesignator.blockchainDrd.captionText.text
            });
            
            this.inputDesignator.EnableDropdown(false);
        }

        // public void SwitchNetwork()
        // {
        //     await this.InitializeFace();
        //
        //     string blockchain = this.inputDesignator.blockchainDrd.captionText.text;
        //     string profile = this.inputDesignator.profileDrd.captionText.text;
        //     await this._face.wallet.SwitchNetwork(NetworkResolver.GetNetwork(blockchain, profile));
        //
        //     // this.GetBalance();
        // }
        
        public void LoginAndGetBalance()
        {
            // TODO: this is temporal code, so remove after initialize button ui created
            this.InitializeFace();
            
            Task<FaceRpcResponse> responseTask = this.face.wallet.LoginWithCredential();
            this.actionQueue.Enqueue(response =>
            {
                FaceLoginResponse faceLoginResponse = response.CastResult<FaceLoginResponse>();
                this.dataDesignator.SetLoggedInId(faceLoginResponse.faceUserId);
                this.dataDesignator.SetLoggedInAddress(faceLoginResponse.wallet.Address);
            }, responseTask);
        }
        
        public void Logout()
        {
            Task<FaceRpcResponse> responseTask = this.face.wallet.Logout();
            this.actionQueue.Enqueue(response =>
            {
                string result = JsonConvert.SerializeObject(response);
                Debug.Log($"Result: {result}");
                this.dataDesignator.Initialize();
                this.inputDesignator.EnableDropdown(true);
                this.face.Disconnect();
            }, responseTask);
        }

        public void GetBalance()
        {
            this.ValidateIsLoggedIn();
            
            Task<FaceRpcResponse> responseTask = this.face.wallet.GetBalance(this.dataDesignator.loggedInAddress.text);
            this.actionQueue.Enqueue(response =>
            {
                string result = JsonConvert.SerializeObject(response);
                Debug.Log($"Result: {result}");
                this.dataDesignator.SetCoinBalance(NumberFormatter.DivideHexWithDecimals(response.CastResult<string>(), 18));
                response.CastResult<string>();
            }, responseTask);
        }

        // public async void SendNativeCoinTransaction()
        // {
        //     this.ValidateIsLoggedIn();
        //
        // string amount =
        //     NumberFormatter.DecimalStringToHexadecimal(
        //         NumberFormatter.DecimalStringToIntegerString(this.inputDesignator.amount.text, 18));
        // RawTransaction request = RawTransaction.Of(this.dataDesignator.loggedInAddress.text,
        //     this.inputDesignator.to.text,
        //     string.Format($"0x{amount}"), null);
        // FaceRpcResponse response = await this._face.wallet.SendTransaction(request);
        // this.dataDesignator.SetResult(string.Format($"TX Hash - {response.result}"));
        //
        // this.GetBalance();
        // }
        //
        // public async void SendErc20Transaction()
        // {
        //     this.ValidateIsLoggedIn();
        //
        //     string amount = this.inputDesignator.erc20Amount.text;
        //     int decimals = await this.GetErc20Decimals();
        //     string data =
        //         this._face.dataFactory.CreateErc20SendData(this.inputDesignator.erc20TokenAddress.text,
        //             this.inputDesignator.erc20To.text, amount,
        //             decimals);
        //     // FIXME: after fix server(price api) to accept token address ignore case, remove ToLower() method.
        //     RawTransaction request = RawTransaction.Of(this.dataDesignator.loggedInAddress.text,
        //         this.inputDesignator.erc20TokenAddress.text.ToLower(), "0x0",
        //         data);
        //     FaceRpcResponse response = await this._face.wallet.SendTransaction(request);
        //     this.dataDesignator.SetResult(string.Format($"ERC20 tx Hash - {response.result}"));
        //
        //     this.GetBalance();
        // }
        //
        // public async void GetErc20Balance()
        // {
        //     this.ValidateIsLoggedIn();
        //
        //     string data =
        //         this._face.dataFactory.CreateErc20GetBalanceData(this.inputDesignator.erc20BalanceInquiryAddress.text,
        //             this.dataDesignator.loggedInAddress.text);
        //     RawTransaction request =
        //         RawTransaction.Of(null, this.inputDesignator.erc20BalanceInquiryAddress.text, "0x0", data);
        //     FaceRpcResponse response = await this._face.wallet.Call(request);
        //     this.dataDesignator.SetErc20Balance(NumberFormatter.DivideHexWithDecimals(response.result.ToString(), 18));
        // }
        //
        // public async void SendErc721Transaction()
        // {
        //     this.ValidateIsLoggedIn();
        //
        //     string data = this._face.dataFactory.CreateErc721SendData(this.inputDesignator.erc721NftAddress.text,
        //         this.dataDesignator.loggedInAddress.text, this.inputDesignator.erc721To.text,
        //         this.inputDesignator.erc721TokenId.text);
        //     RawTransaction request = RawTransaction.Of(this.dataDesignator.loggedInAddress.text,
        //         this.inputDesignator.erc721NftAddress.text,
        //         "0x0",
        //         data);
        //     FaceRpcResponse response = await this._face.wallet.SendTransaction(request);
        //     this.dataDesignator.SetResult(string.Format($"ERC721 tx Hash - {response.result}"));
        //
        //     this.GetBalance();
        // }
        //
        // public async void SendErc1155Transaction()
        // {
        //     this.ValidateIsLoggedIn();
        //
        //     string data = this._face.dataFactory.CreateErc1155SendBatchData(this.inputDesignator.erc1155NftAddress.text,
        //         this.dataDesignator.loggedInAddress.text, this.inputDesignator.erc1155To.text,
        //         this.inputDesignator.erc1155TokenId.text,
        //         this.inputDesignator.erc1155Quantity.text);
        //     RawTransaction request = RawTransaction.Of(this.dataDesignator.loggedInAddress.text,
        //         this.inputDesignator.erc1155NftAddress.text,
        //         "0x0",
        //         data);
        //     FaceRpcResponse response = await this._face.wallet.SendTransaction(request);
        //     this.dataDesignator.SetResult(string.Format($"ERC1155 tx Hash - {response.result}"));
        //
        //     this.GetBalance();
        // }
        //
        // public async void SignMessage()
        // {
        //     this.ValidateIsLoggedIn();
        //
        //     FaceRpcResponse response = await this._face.wallet.Sign(this.inputDesignator.messageToSign.text);
        //     this.dataDesignator.SetResult(string.Format($"Signed Message - {response.result}"));
        // }
        //
        // private async void GetMainAccount()
        // {
        //     FaceRpcResponse getAddressesResponse = await this._face.wallet.GetAddresses();
        //     this.dataDesignator.SetLoggedInAddress(((FaceArrayResponse)getAddressesResponse.result).response[0]);
        // }
        //
        // private async Task<int> GetErc20Decimals()
        // {
        //     string data =
        //         this._face.dataFactory.CreateErc20GetDecimalsData(this.inputDesignator.erc20TokenAddress.text);
        //     RawTransaction request = RawTransaction.Of(null, this.inputDesignator.erc20TokenAddress.text, "0x0", data);
        //     FaceRpcResponse response = await this._face.wallet.Call(request);
        //     return int.Parse(NumberFormatter.HexadecimalToDecimal(response.result.ToString()).ToStringInvariant());
        // }

        private void ValidateIsLoggedIn()
        {
            string loggedInId = this.dataDesignator.loggedInId.text;
            string loggedInAddress = this.dataDesignator.loggedInAddress.text;
            if (String.IsNullOrEmpty(loggedInAddress) || String.IsNullOrEmpty(loggedInId))
            {
                throw new UnauthorizedAccessException("Not logged in yet.");
            }
        }
    }
}