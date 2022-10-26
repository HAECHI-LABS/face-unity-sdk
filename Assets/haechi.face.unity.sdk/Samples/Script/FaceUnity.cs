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

            Task<LoginResult> responseTask = this.LoginAndGetBalanceAsync();

            this.actionQueue.Enqueue(response =>
            {
                this.dataDesignator.SetLoggedInId(response.userId);
                this.dataDesignator.SetLoggedInAddress(response.userAddress);
                this.dataDesignator.SetCoinBalance(response.balance);
            }, responseTask);
        }

        private async Task<LoginResult> LoginAndGetBalanceAsync()
        {
            FaceRpcResponse response = await this.face.wallet.LoginWithCredential();
            FaceLoginResponse faceLoginResponse = response.CastResult<FaceLoginResponse>();
            string address = faceLoginResponse.wallet.Address;
            string balance = await this.GetBalance(address);

            return new LoginResult(balance, faceLoginResponse.faceUserId, address);
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

            Task<string> responseTask = this.GetBalance(this.dataDesignator.loggedInAddress.text);
            
            this.actionQueue.Enqueue(response =>
            {
                this.dataDesignator.SetCoinBalance(response);
            }, responseTask);
        }
        
        public void GetErc20Balance()
        {
            this.ValidateIsLoggedIn();

            string data = this.face.dataFactory.CreateErc20GetBalanceData(
                this.inputDesignator.erc20BalanceInquiryAddress.text, this.dataDesignator.loggedInAddress.text);
            RawTransaction request =
                new RawTransaction(null, this.inputDesignator.erc20BalanceInquiryAddress.text, "0x0", data);

            Task<FaceRpcResponse> responseTask = this.face.wallet.Call(request);

            this.actionQueue.Enqueue(response =>
            {
                string result = JsonConvert.SerializeObject(response);
                Debug.Log($"Result: {result}");
                this.dataDesignator.SetErc20Balance(
                    NumberFormatter.DivideHexWithDecimals(response.CastResult<string>(), 18));
            }, responseTask);
        }

        public void SendNativeCoinTransaction()
        {
            Task<TransactionResult> transactionTask = this.SendTransactionTask(
                this.inputDesignator.to.text,
                () => null,
                NumberFormatter.DecimalStringToHexadecimal(
                    NumberFormatter.DecimalStringToIntegerString(this.inputDesignator.amount.text, 18))
            );
            this.SendTransactionQueue(transactionTask);
        }

        public void SendErc20Transaction()
        {
            this.SendTransactionQueue(this.SendErc20TransactionTask());
        }

        public void SendErc721Transaction()
        {
            Task<TransactionResult> transactionTask = this.SendTransactionTask(
                this.inputDesignator.erc721NftAddress.text,
                () => this.face.dataFactory.CreateErc721SendData(
                    this.inputDesignator.erc721NftAddress.text,
                    this.dataDesignator.loggedInAddress.text,
                    this.inputDesignator.erc721To.text,
                    this.inputDesignator.erc721TokenId.text)
            );
            this.SendTransactionQueue(transactionTask);
        }

        public void SendErc1155Transaction()
        {
            Task<TransactionResult> transactionTask = this.SendTransactionTask(
                this.inputDesignator.erc1155NftAddress.text,
                () => this.face.dataFactory.CreateErc1155SendBatchData(
                    this.inputDesignator.erc1155NftAddress.text,
                    this.dataDesignator.loggedInAddress.text,
                    this.inputDesignator.erc1155To.text,
                    this.inputDesignator.erc1155TokenId.text,
                    this.inputDesignator.erc1155Quantity.text)
            );
            this.SendTransactionQueue(transactionTask);
        }

        public void SignMessage()
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
        
        private async Task<TransactionResult> SendErc20TransactionTask()
        {
            int decimals = await this.GetDecimals();
            return await this.SendTransactionTask(this.inputDesignator.erc20TokenAddress.text, () =>
            {
                string amount = this.inputDesignator.erc20Amount.text;
                return this.face.dataFactory.CreateErc20SendData(this.inputDesignator.erc20TokenAddress.text, this.inputDesignator.erc20To.text, amount, decimals);
            });
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
        
        private async Task<TransactionResult> SendTransactionTask(string to, Func<string> dataCallback, string value = "0")
        {
            string loggedInAddress = this.dataDesignator.loggedInAddress.text;
            RawTransaction request = new RawTransaction(loggedInAddress, to, string.Format($"0x{value}"), dataCallback());
            TransactionRequestId transactionRequestId = await this.face.wallet.SendTransaction(request);

            string txHash = transactionRequestId.Error ?? transactionRequestId.TransactionId;
            string result = string.Format($"TX Hash - {txHash}");

            return new TransactionResult(await this.GetBalance(loggedInAddress), result);
        }
        
        private void SendTransactionQueue(Task<TransactionResult> transactionTask)
        {
            this.ValidateIsLoggedIn();

            this.actionQueue.Enqueue(response =>
            {
                this.dataDesignator.SetCoinBalance(response.balance);
                this.dataDesignator.SetResult(response.result);
            }, transactionTask);
        }

        private async Task<string> GetBalance(string address)
        {
            FaceRpcResponse response = await this.face.wallet.GetBalance(address);
            return NumberFormatter.DivideHexWithDecimals(response.CastResult<string>(), 18);
        }

        private async Task<int> GetDecimals()
        {
            string decimalsData =
                this.face.dataFactory.CreateErc20GetDecimalsData(this.inputDesignator.erc20TokenAddress.text);
            RawTransaction decimalsRequest =
                new RawTransaction(null, this.inputDesignator.erc20TokenAddress.text, "0x0", decimalsData);
            FaceRpcResponse response = await this.face.wallet.Call(decimalsRequest);
            return int.Parse(NumberFormatter.HexadecimalToDecimal(response.CastResult<string>()).ToStringInvariant());
        }
    }

    public class LoginResult
    {
        public string balance;
        public string userId;
        public string userAddress;

        public LoginResult(string balance, string userId, string userAddress)
        {
            this.balance = balance;
            this.userId = userId;
            this.userAddress = userAddress;
        }
    }

    public class TransactionResult
    {
        public string balance;
        public string result;

        public TransactionResult(string balance, string result)
        {
            this.balance = balance;
            this.result = result;
        }
    }
}