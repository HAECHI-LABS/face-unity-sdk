using System;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Utils;
using Nethereum.Util;
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
                
                this.GetBalance();
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
            }, responseTask);
        }

        public void SendNativeCoinTransaction()
        {
            this.ValidateIsLoggedIn();
            
            string amount = NumberFormatter.DecimalStringToHexadecimal(NumberFormatter.DecimalStringToIntegerString("0.0001", 18));
            RawTransaction request = new RawTransaction("0x27f3bfc6f7f886b5cb64f79b4031a4ab56fcb814",
                this.inputDesignator.to.text,
                string.Format($"0x{amount}"), null);

            this.SendTransaction(request);
        }
        
        public void SendErc20Transaction()
        {
            this.ValidateIsLoggedIn();
        
            string amount = this.inputDesignator.erc20Amount.text;
            int decimals = this.GetErc20Decimals();
            string data = this.face.dataFactory.CreateErc20SendData(this.inputDesignator.erc20TokenAddress.text,
                this.inputDesignator.erc20To.text, amount,
                decimals);
            RawTransaction request = new RawTransaction(this.dataDesignator.loggedInAddress.text,
                this.inputDesignator.erc20TokenAddress.text, "0x0",
                data);
            
            this.SendTransaction(request);
        }
        
        public async void GetErc20Balance()
        {
            this.ValidateIsLoggedIn();
        
            string data = this.face.dataFactory.CreateErc20GetBalanceData(this.inputDesignator.erc20BalanceInquiryAddress.text, this.dataDesignator.loggedInAddress.text);
            RawTransaction request = new RawTransaction(null, this.inputDesignator.erc20BalanceInquiryAddress.text, "0x0", data);
            
            Task<FaceRpcResponse> responseTask = this.face.wallet.Call(request);
            
            this.actionQueue.Enqueue(response =>
            {
                string result = JsonConvert.SerializeObject(response);
                Debug.Log($"Result: {result}");
                this.dataDesignator.SetErc20Balance(NumberFormatter.DivideHexWithDecimals(response.CastResult<string>(), 18));
            }, responseTask);
        }
        
        public async void SendErc721Transaction()
        {
            this.ValidateIsLoggedIn();
        
            string data = this.face.dataFactory.CreateErc721SendData(this.inputDesignator.erc721NftAddress.text,
                this.dataDesignator.loggedInAddress.text, this.inputDesignator.erc721To.text,
                this.inputDesignator.erc721TokenId.text);
            RawTransaction request = new RawTransaction(this.dataDesignator.loggedInAddress.text,
                this.inputDesignator.erc721NftAddress.text,
                "0x0",
                data);
            
            this.SendTransaction(request);
        }
        
        public async void SendErc1155Transaction()
        {
            this.ValidateIsLoggedIn();
        
            string data = this.face.dataFactory.CreateErc1155SendBatchData(this.inputDesignator.erc1155NftAddress.text,
                this.dataDesignator.loggedInAddress.text, this.inputDesignator.erc1155To.text,
                this.inputDesignator.erc1155TokenId.text,
                this.inputDesignator.erc1155Quantity.text);
            RawTransaction request = new RawTransaction(this.dataDesignator.loggedInAddress.text,
                this.inputDesignator.erc1155NftAddress.text,
                "0x0",
                data);
        
            this.SendTransaction(request);
        }
        
        public async void SignMessage()
        {
            this.ValidateIsLoggedIn();
        
            Task<FaceRpcResponse> responseTask = this.face.wallet.Sign(this.inputDesignator.messageToSign.text);
            
            this.actionQueue.Enqueue(response =>
            {
                string result = JsonConvert.SerializeObject(response);
                Debug.Log($"Result: {result}");
                this.dataDesignator.SetResult(string.Format($"Signed Message - {response.CastResult<string>()}"));
            }, responseTask);
        }

        private void SendTransaction(RawTransaction request)
        {
            Task<TransactionRequestId> transactionRequestId = this.face.wallet.SendTransaction(request);
            
            this.actionQueue.Enqueue(transaction =>
            {
                Debug.Log($"Result: {transaction}");
                string result = transaction.Error ?? transaction.TransactionId;
                this.dataDesignator.SetResult(string.Format($"TX Hash - {result}"));
                
                this.GetBalance();
            }, transactionRequestId);
        }

        private int GetErc20Decimals()
        {
            string data =
                this.face.dataFactory.CreateErc20GetDecimalsData(this.inputDesignator.erc20TokenAddress.text);
            RawTransaction request = new RawTransaction(null, this.inputDesignator.erc20TokenAddress.text, "0x0", data);
            Task<FaceRpcResponse> responseTask = this.face.wallet.Call(request);
            
            int @decimal = 0;
            this.actionQueue.Enqueue(response =>
            {
                string result = JsonConvert.SerializeObject(response);
                Debug.Log($"Result: {result}");
                @decimal = int.Parse(NumberFormatter.HexadecimalToDecimal(response.CastResult<string>()).ToStringInvariant());
            }, responseTask);

            return @decimal;
        }

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