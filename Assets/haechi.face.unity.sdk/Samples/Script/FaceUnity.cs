using System;
using System.Linq;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Type;
using haechi.face.unity.sdk.Runtime.Utils;
using Nethereum.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace haechi.face.unity.sdk.Samples.Script
{
    public class FaceUnity : MonoBehaviour
    {
        public delegate void PostProcess();

        [SerializeField] internal string webviewUri;

        [SerializeField] internal Button loginBtn, logoutBtn, getBalanceBtn;

        [SerializeField] internal Button sendNativeCoinTransactionBtn,
            sendErc20TransactionBtn,
            getErc20BalanceBtn,
            sendErc721TransactionBtn,
            sendErc1155TransactionBtn,
            signMessageBtn;

        public TMP_Dropdown profileDrd, blockchainDrd;
        public TMP_InputField to, value;
        public TMP_InputField erc1155To, erc1155TokenId, erc1155Quantity, erc1155NftAddress;
        public TMP_InputField erc20To, erc20Value, erc20TokenAddress, erc20BalanceInquiryAddress, erc20Balance;
        public TMP_InputField erc721To, erc721TokenId, erc721NftAddress;
        public TMP_InputField messageToSign;
        public TMP_Text loginedAddress, loginedId, coinBalance, result;
        private Face _face;

        private void Awake()
        {
            this.profileDrd.ClearOptions();
            this.profileDrd.AddOptions(Enum.GetNames(typeof(Profile)).ToList());
            this.blockchainDrd.ClearOptions();
            this.blockchainDrd.AddOptions(Enum.GetNames(typeof(Blockchain)).ToList());

            this._face = new Face(this.webviewUri);
        }

        /**
         * Should initialize Face first for webview client to use Face SDK
         */
        public async Task InitializeFace()
        {
            string apiKey = ""; // TODO: receive from text field
            string blockchain = this.blockchainDrd.captionText.text;
            string profile = this.profileDrd.captionText.text;
            FaceEnvironments environments =
                new FaceEnvironments(NetworkResolver.GetNetwork(blockchain, profile), profile, apiKey);
            await this._face.wallet.InitializeFaceSdk(environments);
        }

        public async void SwitchNetwork()
        {
            string blockchain = this.blockchainDrd.captionText.text;
            string profile = this.profileDrd.captionText.text;
            await this._face.wallet.SwitchNetwork(NetworkResolver.GetNetwork(blockchain, profile));

            this.GetBalance();
        }

        public async void LoginAndGetBalance()
        {
            await this.InitializeFace();
            
            FaceRpcResponse loginResponse = await this._face.wallet.LoginWithCredential();
            this.loginedId.text = ((FaceLoginResponse)loginResponse.result).faceUserId;

            FaceRpcResponse getAddressesResponse = await this._face.wallet.GetAddresses();
            this.loginedAddress.text = ((FaceArrayResponse)getAddressesResponse.result).response[0];

            this.GetBalance();
        }

        public async void GetMainAccount()
        {
            FaceRpcResponse getAddressesResponse = await this._face.wallet.GetAddresses();
            this.loginedAddress.text = ((FaceArrayResponse)getAddressesResponse.result).response[0];
        }

        public async void Logout()
        {
            await this._face.wallet.Logout();
            this.loginedId.text = null;
            this.loginedAddress.text = null;
            this.coinBalance.text = null;
            this.result.text = null;
        }

        public async void GetBalance()
        {
            FaceRpcResponse response = await this._face.wallet.GetBalance(this.loginedAddress.text);
            this.coinBalance.text = NumberFormatter.DivideHexWithDecimals(response.result.ToString(), 18);
        }

        public async void SendNativeCoinTransaction()
        {
            string amount =
                NumberFormatter.DecimalStringToHexadecimal(
                    NumberFormatter.DecimalStringToIntegerString(this.value.text, 18));
            RawTransaction request = new RawTransaction(this.loginedAddress.text, this.to.text,
                string.Format($"0x{amount}"), null);
            FaceRpcResponse response = await this._face.wallet.SendTransaction(request);
            this.result.text = string.Format($"TX Hash - {response.result}");

            this.GetBalance();
        }

        public async void SendErc20Transaction()
        {
            string amount = this.erc20Value.text;
            int decimals = await this.GetErc20Decimals();
            string data =
                this._face.dataFactory.CreateErc20SendData(this.erc20TokenAddress.text, this.erc20To.text, amount,
                    decimals);
            // FIXME: after fix server(price api) to accept token address ignore case, remove ToLower() method.
            RawTransaction request = new RawTransaction(this.loginedAddress.text, this.erc20TokenAddress.text.ToLower(), "0x0",
                data);
            FaceRpcResponse response = await this._face.wallet.SendTransaction(request);
            this.result.text = string.Format($"ERC20 tx Hash - {response.result}");

            this.GetBalance();
        }

        public async void GetErc20Balance()
        {
            string data =
                this._face.dataFactory.CreateErc20GetBalanceData(this.erc20BalanceInquiryAddress.text,
                    this.loginedAddress.text);
            RawTransaction request = new RawTransaction(null, this.erc20BalanceInquiryAddress.text, "0x0", data);
            FaceRpcResponse response = await this._face.wallet.Call(request);
            this.erc20Balance.text = NumberFormatter.DivideHexWithDecimals(response.result.ToString(), 18);
        }

        public async void SendErc721Transaction()
        {
            string data = this._face.dataFactory.CreateErc721SendData(this.erc721NftAddress.text,
                this.loginedAddress.text, this.erc721To.text, this.erc721TokenId.text);
            RawTransaction request = new RawTransaction(this.loginedAddress.text, this.erc721NftAddress.text, "0x0",
                data);
            FaceRpcResponse response = await this._face.wallet.SendTransaction(request);
            this.result.text = string.Format($"ERC721 tx Hash - {response.result}");

            this.GetBalance();
        }

        public async void SendErc1155Transaction()
        {
            string data = this._face.dataFactory.CreateErc1155SendBatchData(this.erc1155NftAddress.text,
                this.loginedAddress.text, this.erc1155To.text, this.erc1155TokenId.text, this.erc1155Quantity.text);
            RawTransaction request = new RawTransaction(this.loginedAddress.text, this.erc1155NftAddress.text, "0x0",
                data);
            FaceRpcResponse response = await this._face.wallet.SendTransaction(request);
            this.result.text = string.Format($"ERC1155 tx Hash - {response.result}");

            this.GetBalance();
        }

        public async void SignMessage()
        {
            FaceRpcResponse response = await this._face.wallet.Sign(this.messageToSign.text);
            this.result.text = string.Format($"Signed Message - {response.result}");
        }

        private async Task<bool> IsLoggedIn()
        {
            FaceRpcResponse response = await this._face.wallet.IsLoggedIn();
            return bool.Parse(response.result.ToString());
        }

        private async Task<int> GetErc20Decimals()
        {
            string data = this._face.dataFactory.CreateErc20GetDecimalsData(this.erc20TokenAddress.text);
            RawTransaction request = new RawTransaction(null, this.erc20TokenAddress.text, "0x0", data);
            FaceRpcResponse response = await this._face.wallet.Call(request);
            return int.Parse(NumberFormatter.HexadecimalToDecimal(response.result.ToString()).ToStringInvariant());
        }
    }
}