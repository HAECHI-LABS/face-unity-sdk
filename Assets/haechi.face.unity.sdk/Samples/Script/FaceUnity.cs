using System;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Exception;
using haechi.face.unity.sdk.Runtime.Type;
using haechi.face.unity.sdk.Runtime.Utils;
using Nethereum.Util;
using Newtonsoft.Json;
using UnityEngine;

namespace haechi.face.unity.sdk.Samples.Script
{
    public class FaceUnity : MonoBehaviour
    {
        private static string SAMPLE_API_KEY =
            "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCS23ncDS7x8nmTuK1FFN0EfYo0vo6xhTBMBNWVbQsufv60X8hv3-TbAQ3JIyMEhLo-c-31oYrvrQ0G2e9j8yvJYEUnLuE-PaABo0y3V5m9g_qdTB5p9eEfqZlDrcUl1zUr4W7rJwFwkTlAFSKOqVCPnm8ozmcMyyrEHgl2AbehrQIDAQAB";
        private static string SAMPLE_PRIVATE_KEY = 
            "MIICdQIBADANBgkqhkiG9w0BAQEFAASCAl8wggJbAgEAAoGBAJLbedwNLvHyeZO4rUUU3QR9ijS+jrGFMEwE1ZVtCy5+/rRfyG/f5NsBDckjIwSEuj5z7fWhiu+tDQbZ72PzK8lgRScu4T49oAGjTLdXmb2D+p1MHmn14R+pmUOtxSXXNSvhbusnAXCROUAVIo6pUI+ebyjOZwzLKsQeCXYBt6GtAgMBAAECgYAEfQa9bf24UVPb6vIIwXl70KZvtD9CN7LhL+ijN4D2+9SnCKJkoPAqrV6Rfixsz+2tSPfF4RkQ+DYEtpZ1dJIq+kNxqRjb7TEHcduYYQwgkJZe2LPd1LS5bnvLGSbGMHHy7+MYNm6M/ghdHoDU+tkYLNFT19BX7MKbBWQPpoH/gQJBAJllv/CZQBhofxLZO0xsM8xcxTo3MFQoos89+Kdym+a8i/WqD49IgIsiK3adn/GCtjSeKJhPlrd5iNUqTBywUk0CQQD1Fdv9q++RmpuhD6LQtGzeeoNzld7xRjWjHVwHvp7/6xeSCyO8sHKydUF/NmV+Jy8vFpJn6b1AvagtgqALanzhAkBaP1eeWLsx4QCp+S3+90W+PPI4HtILIWEv5jjNYws/w7vgC25eEPy3XqINhgzcjNdfu5EMkv6L8S/Eob7nvgCdAkALF4ArTNq8xjiA44pE08WRlA3a7091r+3BghSmLRRZFLSuYV6urXWjafca4MVbHj7ebLEXjtaH1Y2E8cJ4gctBAkBPXs2bRZpI5ULwyYknWaq77gfuappmShgiCv7TUKixt5KqZy8DUU13x/WTUCWjthF/lmgkVq+FvsnG49dF8TM7";
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
            this.face.Initialize(this._getFaceSettingsInput());
            this.dataDesignator.SetLoginInstruction();
            this.inputDesignator.SetWalletConnectedInputStatus();
        }
        
        public void SwitchNetwork()
        {
            Task<string> responseTask = this._switchNetworkAndGetBalanceAsync();

            this.actionQueue.Enqueue(responseTask, balance =>
            {
                this.dataDesignator.SetCoinBalance(balance);
                this.dataDesignator.SetResult("");
            }, this._defaultExceptionHandler);
        }
        
        private async Task<string> _switchNetworkAndGetBalanceAsync()
        {
            FaceSettings.Parameters faceSettings = this._getFaceSettingsInput();
            await this.face.Wallet().SwitchNetwork(faceSettings.Network);
            string balance = await this._getBalance(this.dataDesignator.loggedInAddress.text);
            return balance;
        }

        private FaceSettings.Parameters _getFaceSettingsInput()
        {
            string apiKey = this.inputDesignator.GetApiKey() != null
                ? this.inputDesignator.GetApiKey().text
                : SAMPLE_API_KEY;
            Profile environment = this.inputDesignator.GetProfileDrd() != null
                ? Profiles.ValueOf(this.inputDesignator.GetProfileDrd().captionText.text)
                : Profile.ProdTest;
            BlockchainNetwork network = this.inputDesignator.GetBlockchainDrd() != null && this.inputDesignator.GetProfileDrd() != null
                ? BlockchainNetworks.GetNetwork(this.inputDesignator.GetBlockchainDrd().captionText.text, this.inputDesignator.GetProfileDrd().captionText.text)
                : BlockchainNetworks.ValueOf(this.inputDesignator.GetNetworkDrd().captionText.text);
            string scheme = Application.identifier == "xyz.facewallet.unity.dev" ? "faceunity" : "faceunitydev";
            
            return new FaceSettings.Parameters
            {
                ApiKey = apiKey,
                Environment = environment,
                Network = network,
                Scheme = scheme
            };
        }

        public void LoginAndGetBalance()
        {
            Task<LoginResult> responseTask = this._loginAndGetBalanceAsync();

            this.actionQueue.Enqueue(responseTask, response =>
            {
                this.dataDesignator.SetLoggedInId(response.userId);
                this.dataDesignator.SetLoggedInAddress(response.userAddress);
                this.dataDesignator.SetCoinBalance(response.balance);
                this.dataDesignator.SetLogoutInstruction();
                this.inputDesignator.SetLoggedInInputStatus();                
            }, this._defaultExceptionHandler);
        }
        
        private async Task<LoginResult> _loginAndGetBalanceAsync()
        {
            Debug.Log("Start Login");
            FaceLoginResponse response = await this.face.Auth().Login();
            string address = response.wallet.Address;
            Debug.Log("Login balance");

            string balance = await this._getBalance(address);
            
            Debug.Log("Login done");

            return new LoginResult(balance, response.faceUserId, address);
        } 
        
        public async Task LoginWithIdTokenAndGetBalanceAsync(string idToken)
        {
            Debug.Log("Start Login");

            FaceLoginResponse response = await this.face.Auth().LoginWithIdToken(
                new FaceLoginIdTokenRequest(idToken, RsaSigner.Sign(
                    (this.inputDesignator.GetPrivateKey() == null
                        ? SAMPLE_PRIVATE_KEY
                        : this.inputDesignator.GetPrivateKey().text), idToken))
            );
            string address = response.wallet.Address;
            Debug.Log("Login balance");

            string balance = await this._getBalance(address);
            
            Debug.Log("Login done");

            LoginResult loginResult = new LoginResult(balance, response.faceUserId, address);
            this.dataDesignator.SetLoggedInId(loginResult.userId);
            this.dataDesignator.SetLoggedInAddress(loginResult.userAddress);
            this.dataDesignator.SetCoinBalance(loginResult.balance);
            this.dataDesignator.SetLogoutInstruction();
            this.inputDesignator.SetLoggedInInputStatus();         
        }
        
        public void GoogleLoginAndGetBalance()
        {
            this._directSocialLoginAndGetBalance(LoginProviderType.Google);
        }
        
        public void FacebookLoginAndGetBalance()
        {
            this._directSocialLoginAndGetBalance(LoginProviderType.Facebook);
        }
        
        public void AppleLoginAndGetBalance()
        {
            this._directSocialLoginAndGetBalance(LoginProviderType.Apple);
        }
        
        private void _directSocialLoginAndGetBalance(string provider)
        {
            Task<LoginResult> responseTask = this._directSocialLoginAndGetBalanceAsync(provider);

            this.actionQueue.Enqueue(responseTask, response =>
            {
                this.dataDesignator.SetLoggedInId(response.userId);
                this.dataDesignator.SetLoggedInAddress(response.userAddress);
                this.dataDesignator.SetCoinBalance(response.balance);
                
                this.dataDesignator.SetLogoutInstruction();
                this.inputDesignator.SetLoggedInInputStatus();
            }, this._defaultExceptionHandler);
        }

        private async Task<LoginResult> _directSocialLoginAndGetBalanceAsync(string provider)
        {
            FaceLoginResponse response = await this.face.Auth().DirectSocialLogin(provider);
            string address = response.wallet.Address;
            string balance = await this._getBalance(address);

            return new LoginResult(balance, response.faceUserId, address);
        }

        public void Logout()
        {
            Task<FaceRpcResponse> responseTask = this.face.Auth().Logout();

            this.actionQueue.Enqueue(responseTask, response =>
            {
                string result = JsonConvert.SerializeObject(response);
                Debug.Log($"Result: {result}");
                this.dataDesignator.InitializeDataStatus();
                this.inputDesignator.InitializeInputStatus();
                this.face.Disconnect();
            }, this._defaultExceptionHandler);
        }

        public void GetBalance()
        {
            this._validateIsLoggedIn();

            Task<string> responseTask = this._getBalance(this.dataDesignator.loggedInAddress.text);
            
            this.actionQueue.Enqueue(responseTask, response =>
            {
                this.dataDesignator.SetCoinBalance(response);
            }, this._defaultExceptionHandler);
        }
        
        public void GetErc20Balance()
        {
            this._validateIsLoggedIn();

            string data = this.face.dataFactory.CreateErc20GetBalanceData(
                this.inputDesignator.GetErc20BalanceInquiryAddress().text, this.dataDesignator.loggedInAddress.text);
            RawTransaction request =
                new RawTransaction(null, this.inputDesignator.GetErc20BalanceInquiryAddress().text, "0x0", data);

            Task<FaceRpcResponse> responseTask = this.face.Wallet().Call(request);

            this.actionQueue.Enqueue(responseTask, response =>
            {
                string result = JsonConvert.SerializeObject(response);
                Debug.Log($"Result: {result}");
                this.dataDesignator.SetErc20Balance(
                    NumberFormatter.DivideHexWithDecimals(response.CastResult<string>(), 18));
            }, this._defaultExceptionHandler);
        }

        public void SendNativeCoinTransaction()
        {
            Task<TransactionResult> transactionTask = this._sendTransactionTask(
                this.inputDesignator.GetTo().text,
                () => null,
                NumberFormatter.DecimalStringToHexadecimal(
                    NumberFormatter.DecimalStringToIntegerString(this.inputDesignator.GetAmount().text, 18))
            );
            this._sendTransactionQueue(transactionTask);
        }

        public void SendErc20Transaction()
        {
            this._sendTransactionQueue(this._sendErc20TransactionTask());
        }

        public void SendErc721Transaction()
        {
            Task<TransactionResult> transactionTask = this._sendTransactionTask(
                this.inputDesignator.GetErc721NftAddress().text,
                () => this.face.dataFactory.CreateErc721SendData(
                    this.inputDesignator.GetErc721NftAddress().text,
                    this.dataDesignator.loggedInAddress.text,
                    this.inputDesignator.GetErc721To().text,
                    this.inputDesignator.GetErc721TokenId().text)
            );
            this._sendTransactionQueue(transactionTask);
        }

        public void SendErc1155Transaction()
        {
            Task<TransactionResult> transactionTask = this._sendTransactionTask(
                this.inputDesignator.GetErc1155NftAddress().text,
                () => this.face.dataFactory.CreateErc1155SendBatchData(
                    this.inputDesignator.GetErc1155NftAddress().text,
                    this.dataDesignator.loggedInAddress.text,
                    this.inputDesignator.GetErc1155To().text,
                    this.inputDesignator.GetErc1155TokenId().text,
                    this.inputDesignator.GetErc1155Quantity().text)
            );
            this._sendTransactionQueue(transactionTask);
        }

        public void ConnectWallet()
        {
            this._validateIsLoggedIn();
            this.face.Wallet().ConnectOpenSea(this.dataDesignator.loggedInAddress.text);
             //this.face.Wallet().ConnectWallet( this.dataDesignator.loggedInAddress.text, this.inputDesignator.wcUrl.text);
        }

        public void SignMessage()
        {
            this._validateIsLoggedIn();

            Task<FaceRpcResponse> responseTask = this.face.Wallet().SignMessage(this.inputDesignator.GetMessageToSign().text);

            this.actionQueue.Enqueue(responseTask, response =>
            {
                string result = JsonConvert.SerializeObject(response);
                Debug.Log($"Result: {result}");
                this.dataDesignator.SetResult(string.Format($"Signed Message - {response.CastResult<string>()}"));
            }, this._defaultExceptionHandler);
        }
        
        private async Task<TransactionResult> _sendErc20TransactionTask()
        {
            int decimals = await this._getDecimals();
            return await this._sendTransactionTask(this.inputDesignator.GetErc20TokenAddress().text, () =>
            {
                string amount = this.inputDesignator.GetErc20Amount().text;
                return this.face.dataFactory.CreateErc20SendData(this.inputDesignator.GetErc20TokenAddress().text, this.inputDesignator.GetErc20To().text, amount, decimals);
            });
        }

        private void _validateIsLoggedIn()
        {
            string loggedInId = this.dataDesignator.loggedInId.text;
            string loggedInAddress = this.dataDesignator.loggedInAddress.text;
            if (String.IsNullOrEmpty(loggedInAddress) || String.IsNullOrEmpty(loggedInId))
            {
                throw new UnauthorizedException();
            }
        }
        
        private async Task<TransactionResult> _sendTransactionTask(string to, Func<string> dataCallback, string value = "0")
        {
            string loggedInAddress = this.dataDesignator.loggedInAddress.text;
            RawTransaction request = new RawTransaction(loggedInAddress, to, string.Format($"0x{value}"), dataCallback());
            try
            {
                TransactionRequestId transactionRequestId = await this.face.Wallet().SendTransaction(request);
                return new TransactionResult(await this._getBalance(loggedInAddress), string.Format($"TX Hash - {transactionRequestId.transactionId}"));
            }
            catch (FaceException ex)
            {
                return new TransactionResult(await this._getBalance(loggedInAddress), string.Format($"Error - {ex.Message}"));
            }
        }
        
        private void _sendTransactionQueue(Task<TransactionResult> transactionTask)
        {
            this._validateIsLoggedIn();

            this.actionQueue.Enqueue(transactionTask, response =>
            {
                this.dataDesignator.SetCoinBalance(response.balance);
                this.dataDesignator.SetResult(response.result);
            }, this._defaultExceptionHandler);
        }

        private async Task<string> _getBalance(string address)
        {
            FaceRpcResponse response = await this.face.Wallet().GetBalance(address);
            return NumberFormatter.DivideHexWithDecimals(response.CastResult<string>(), 18);
        }

        private async Task<int> _getDecimals()
        {
            string decimalsData =
                this.face.dataFactory.CreateErc20GetDecimalsData(this.inputDesignator.GetErc20TokenAddress().text);
            RawTransaction decimalsRequest =
                new RawTransaction(null, this.inputDesignator.GetErc20TokenAddress().text, "0x0", decimalsData);
            FaceRpcResponse response = await this.face.Wallet().Call(decimalsRequest);
            return int.Parse(NumberFormatter.HexadecimalToDecimal(response.CastResult<string>()).ToStringInvariant());
        }
        
        private void _defaultExceptionHandler(Exception ex)
        {
            this.dataDesignator.SetResult(ex.StackTrace);
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