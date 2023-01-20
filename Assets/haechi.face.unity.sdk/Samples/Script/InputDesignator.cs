using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace haechi.face.unity.sdk.Samples.Script
{
    public class InputDesignator : MonoBehaviour
    {
        [SerializeField] internal Button initializeBtn, switchNetworkBtn, loginBtn, googleLoginBtn, facebookLoginBtn, appleLoginBtn, logoutBtn, getBalanceBtn;
        [SerializeField] internal Button landscapeInitializeBtn, landscapeSwitchNetworkBtn, landscapeLoginBtn, landscapeGoogleLoginBtn, landscapeFacebookLoginBtn, landscapeAppleLoginBtn, landscapeLogoutBtn, landscapeGetBalanceBtn;
        [SerializeField] internal Button webInitializeBtn, webSwitchNetworkBtn, webLoginBtn, webGoogleLoginBtn, webFacebookLoginBtn, webAppleLoginBtn, webLogoutBtn, webGetBalanceBtn;

        [SerializeField] internal Button sendNativeCoinTransactionBtn,
            sendErc20TransactionBtn,
            getErc20BalanceBtn,
            sendErc721TransactionBtn,
            sendErc1155TransactionBtn,
            signMessageBtn;
        
        [SerializeField] internal Button landscapeSendNativeCoinTransactionBtn,
            landscapeSendErc20TransactionBtn,
            landscapeGetErc20BalanceBtn,
            landscapeSendErc721TransactionBtn,
            landscapeSendErc1155TransactionBtn,
            landscapeSignMessageBtn;
        [SerializeField] internal Button webSendNativeCoinTransactionBtn,
            webSendErc20TransactionBtn,
            webGetErc20BalanceBtn,
            webSendErc721TransactionBtn,
            webSendErc1155TransactionBtn,
            webSignMessageBtn;
        
        public TMP_Dropdown profileDrd, blockchainDrd, networkDrd;
        public TMP_InputField apiKey;
        public TMP_InputField wcUrl;
        public TMP_InputField to, amount;
        public TMP_InputField erc1155To, erc1155TokenId, erc1155Quantity, erc1155NftAddress;
        public TMP_InputField erc20To, erc20Amount, erc20TokenAddress, erc20BalanceInquiryAddress;
        public TMP_InputField erc721To, erc721TokenId, erc721NftAddress;
        public TMP_InputField messageToSign;

        public TMP_Dropdown landscapeProfileDrd, landscapeBlockchainDrd, landscapeNetworkDrd;
        public TMP_InputField landscapeApiKey;
        public TMP_InputField landscapeTo, landscapeAmount;

        public TMP_InputField landscapeErc1155To,
            landscapeErc1155TokenId,
            landscapeErc1155Quantity,
            landscapeErc1155NftAddress;

        public TMP_InputField landscapeErc20To,
            landscapeErc20Amount,
            landscapeErc20TokenAddress,
            landscapeErc20BalanceInquiryAddress;

        public TMP_InputField landscapeErc721To, landscapeErc721TokenId, landscapeErc721NftAddress;
        public TMP_InputField landscapeMessageToSign;
        
        public TMP_Dropdown webProfileDrd, webBlockchainDrd, webNetworkDrd;
        public TMP_InputField webApiKey;
        public TMP_InputField webTo, webAmount;

        public TMP_InputField webErc1155To,
            webErc1155TokenId,
            webErc1155Quantity,
            webErc1155NftAddress;

        public TMP_InputField webErc20To,
            webErc20Amount,
            webErc20TokenAddress,
            webErc20BalanceInquiryAddress;

        public TMP_InputField webErc721To, webErc721TokenId, webErc721NftAddress;
        public TMP_InputField webMessageToSign;

        private void Start()
        {
#if !UNITY_WEBGL
            if (this.profileDrd != null)
            {
                this.profileDrd.onValueChanged.AddListener(value => { SetDropdown(this.landscapeProfileDrd, value); });
                this.landscapeProfileDrd.onValueChanged.AddListener(value => { SetDropdown(this.profileDrd, value); });
            }

            if (this.blockchainDrd != null)
            {
                this.blockchainDrd.onValueChanged.AddListener(value =>
                {
                    SetDropdown(this.landscapeBlockchainDrd, value);
                });
                this.landscapeBlockchainDrd.onValueChanged.AddListener(value =>
                {
                    SetDropdown(this.blockchainDrd, value);
                });
            }

            if (this.networkDrd != null)
            {
                this.networkDrd.onValueChanged.AddListener(value =>
                {
                    SetDropdown(this.landscapeNetworkDrd, value);
                });
                this.landscapeNetworkDrd.onValueChanged.AddListener(value =>
                {
                    SetDropdown(this.networkDrd, value);
                });
            }

            if (this.apiKey != null)
            {
                this.apiKey.onValueChanged.AddListener(value => { SetInputText(this.landscapeApiKey, value); });
                this.landscapeApiKey.onValueChanged.AddListener(value => { SetInputText(this.apiKey, value); });   
            }

            this.to.onValidateInput += delegate(string s, int i, char c) { return char.ToLower(c); };
            this.to.onValueChanged.AddListener(value => { SetInputText(this.landscapeTo, value); });
            this.amount.onValueChanged.AddListener(value => { SetInputText(this.landscapeAmount, value); });
            this.erc1155To.onValidateInput += delegate(string s, int i, char c) { return char.ToLower(c); };
            this.erc1155To.onValueChanged.AddListener(value => { SetInputText(this.landscapeErc1155To, value); });
            this.erc1155TokenId.onValueChanged.AddListener(value =>
            {
                SetInputText(this.landscapeErc1155TokenId, value);
            });
            this.erc1155Quantity.onValueChanged.AddListener(value =>
            {
                SetInputText(this.landscapeErc1155Quantity, value);
            });
            this.erc1155NftAddress.onValidateInput += delegate(string s, int i, char c) { return char.ToLower(c); };
            this.erc1155NftAddress.onValueChanged.AddListener(value =>
            {
                SetInputText(this.landscapeErc1155NftAddress, value);
            });
            this.erc20To.onValidateInput += delegate(string s, int i, char c) { return char.ToLower(c); };
            this.erc20To.onValueChanged.AddListener(value => { SetInputText(this.landscapeErc20To, value); });
            this.erc20Amount.onValueChanged.AddListener(value => { SetInputText(this.landscapeErc20Amount, value); });
            this.erc20TokenAddress.onValidateInput += delegate(string s, int i, char c) { return char.ToLower(c); };
            this.erc20TokenAddress.onValueChanged.AddListener(value =>
            {
                SetInputText(this.landscapeErc20TokenAddress, value);
            });
            this.erc20BalanceInquiryAddress.onValidateInput += delegate(string s, int i, char c) { return char.ToLower(c); };
            this.erc20BalanceInquiryAddress.onValueChanged.AddListener(value =>
            {
                SetInputText(this.landscapeErc20BalanceInquiryAddress, value);
            });
            this.erc721To.onValidateInput += delegate(string s, int i, char c) { return char.ToLower(c); };
            this.erc721To.onValueChanged.AddListener(value => { SetInputText(this.landscapeErc721To, value); });
            this.erc721TokenId.onValueChanged.AddListener(
                value => { SetInputText(this.landscapeErc721TokenId, value); });
            this.erc721NftAddress.onValidateInput += delegate(string s, int i, char c) { return char.ToLower(c); };
            this.erc721NftAddress.onValueChanged.AddListener(value =>
            {
                SetInputText(this.landscapeErc721NftAddress, value);
            });
            this.messageToSign.onValueChanged.AddListener(
                value => { SetInputText(this.landscapeMessageToSign, value); });

            this.landscapeTo.onValidateInput += delegate(string s, int i, char c) { return char.ToLower(c); };
            this.landscapeTo.onValueChanged.AddListener(value => { SetInputText(this.to, value); });
            this.landscapeAmount.onValueChanged.AddListener(value => { SetInputText(this.amount, value); });
            this.landscapeErc1155To.onValidateInput += delegate(string s, int i, char c) { return char.ToLower(c); };
            this.landscapeErc1155To.onValueChanged.AddListener(value => { SetInputText(this.erc1155To, value); });
            this.landscapeErc1155TokenId.onValueChanged.AddListener(value =>
            {
                SetInputText(this.erc1155TokenId, value);
            });
            this.landscapeErc1155Quantity.onValueChanged.AddListener(value =>
            {
                SetInputText(this.erc1155Quantity, value);
            });
            this.landscapeErc1155NftAddress.onValidateInput += delegate(string s, int i, char c) { return char.ToLower(c); };
            this.landscapeErc1155NftAddress.onValueChanged.AddListener(value =>
            {
                SetInputText(this.erc1155NftAddress, value);
            });
            this.landscapeErc20To.onValidateInput += delegate(string s, int i, char c) { return char.ToLower(c); };
            this.landscapeErc20To.onValueChanged.AddListener(value => { SetInputText(this.erc20To, value); });
            this.landscapeErc20Amount.onValueChanged.AddListener(value => { SetInputText(this.erc20Amount, value); });
            this.landscapeErc20TokenAddress.onValidateInput += delegate(string s, int i, char c) { return char.ToLower(c); };
            this.landscapeErc20TokenAddress.onValueChanged.AddListener(value =>
            {
                SetInputText(this.erc20TokenAddress, value);
            });
            this.landscapeErc20BalanceInquiryAddress.onValidateInput += delegate(string s, int i, char c) { return char.ToLower(c); };
            this.landscapeErc20BalanceInquiryAddress.onValueChanged.AddListener(value =>
            {
                SetInputText(this.erc20BalanceInquiryAddress, value);
            });
            this.landscapeErc721To.onValidateInput += delegate(string s, int i, char c) { return char.ToLower(c); };
            this.landscapeErc721To.onValueChanged.AddListener(value => { SetInputText(this.erc721To, value); });
            this.landscapeErc721TokenId.onValueChanged.AddListener(
                value => { SetInputText(this.erc721TokenId, value); });
            this.landscapeErc721NftAddress.onValidateInput += delegate(string s, int i, char c) { return char.ToLower(c); };
            this.landscapeErc721NftAddress.onValueChanged.AddListener(value =>
            {
                SetInputText(this.erc721NftAddress, value);
            });
            this.landscapeMessageToSign.onValueChanged.AddListener(
                value => { SetInputText(this.messageToSign, value); });
#endif
        }

        public void InitializeInputStatus()
        {
            this.EnableConnectWalletSection(true);
            this.EnableLogin(false);
            this.EnableGetBalance(false);
            this.EnableLogout(false);
            this.EnableTestSection(false);
        }

        public void SetWalletConnectedInputStatus()
        {
            this.EnableConnectWalletSection(false);
            this.EnableLogin(true);
            this.EnableLogout(true);
        }
        
        public void DisableLoginInputStatus()
        {
            this.EnableLogin(false);
        }

        public void SetLoggedInInputStatus()
        {
            this.EnableLogin(false);
            this.EnableGetBalance(true);
            this.EnableTestSection(true);
        }
        
        public TMP_Dropdown GetProfileDrd()
        {
#if UNITY_WEBGL
            return this.webProfileDrd;
#else
            return this.profileDrd;
#endif
        }
        
        public TMP_Dropdown GetBlockchainDrd()
        {
#if UNITY_WEBGL
            return this.webBlockchainDrd;
#else
            return this.blockchainDrd;
#endif
        }
        
        public TMP_Dropdown GetNetworkDrd()
        {
#if UNITY_WEBGL
            return this.webNetworkDrd;
#else
            return this.networkDrd;
#endif
        }
        
        public TMP_InputField GetApiKey()
        {
#if UNITY_WEBGL
            return this.webApiKey;
#else
            return this.apiKey;
#endif
        }
        
        public TMP_InputField GetTo()
        {
#if UNITY_WEBGL
            return this.webTo;
#else
            return this.to;
#endif
        }
        
        public TMP_InputField GetAmount()
        {
#if UNITY_WEBGL
            return this.webAmount;
#else
            return this.amount;
#endif
        }
        
        public TMP_InputField GetErc1155To()
        {
#if UNITY_WEBGL
            return this.webErc1155To;
#else
            return this.erc1155To;
#endif
        }
        
        public TMP_InputField GetErc1155TokenId()
        {
#if UNITY_WEBGL
            return this.webErc1155TokenId;
#else
            return this.erc1155TokenId;
#endif
        }
        
        public TMP_InputField GetErc1155Quantity()
        {
#if UNITY_WEBGL
            return this.webErc1155Quantity;
#else
            return this.erc1155Quantity;
#endif
        }
        
        public TMP_InputField GetErc1155NftAddress()
        {
#if UNITY_WEBGL
            return this.webErc1155NftAddress;
#else
            return this.erc1155NftAddress;
#endif
        }
        
        public TMP_InputField GetErc20To()
        {
#if UNITY_WEBGL
            return this.webErc20To;
#else
            return this.erc20To;
#endif
        }
        
        public TMP_InputField GetErc20Amount()
        {
#if UNITY_WEBGL
            return this.webErc20Amount;
#else
            return this.erc20Amount;
#endif
        }
        
        public TMP_InputField GetErc20TokenAddress()
        {
#if UNITY_WEBGL
            return this.webErc20TokenAddress;
#else
            return this.erc20TokenAddress;
#endif
        }
        
        public TMP_InputField GetErc20BalanceInquiryAddress()
        {
#if UNITY_WEBGL
            return this.webErc20BalanceInquiryAddress;
#else
            return this.erc20BalanceInquiryAddress;
#endif
        }
        
        public TMP_InputField GetErc721To()
        {
#if UNITY_WEBGL
            return this.webErc721To;
#else
            return this.erc721To;
#endif
        }
        
        public TMP_InputField GetErc721TokenId()
        {
#if UNITY_WEBGL
            return this.webErc721TokenId;
#else
            return this.erc721TokenId;
#endif
        }
        
        public TMP_InputField GetErc721NftAddress()
        {
#if UNITY_WEBGL
            return this.webErc721NftAddress;
#else
            return this.erc721NftAddress;
#endif
        }
        
        public TMP_InputField GetMessageToSign()
        {
#if UNITY_WEBGL
            return this.webMessageToSign;
#else
            return this.messageToSign;
#endif
        }
        
        private void EnableConnectWalletSection(bool enable)
        {
#if UNITY_WEBGL
            if (this.apiKey != null)
            {
                this.webApiKey.interactable = enable;
            }

            this.webInitializeBtn.GetComponent<Button>().gameObject.SetActive(enable);
            this.webSwitchNetworkBtn.GetComponent<Button>().gameObject.SetActive(!enable);
#else
            if (this.apiKey != null)
            {
                this.apiKey.interactable = enable;
                this.landscapeApiKey.interactable = enable;
            }

            this.initializeBtn.GetComponent<Button>().gameObject.SetActive(enable);
            this.switchNetworkBtn.GetComponent<Button>().gameObject.SetActive(!enable);
            this.landscapeInitializeBtn.GetComponent<Button>().gameObject.SetActive(enable);
            this.landscapeSwitchNetworkBtn.GetComponent<Button>().gameObject.SetActive(!enable);
#endif
        }

        private void EnableLogin(bool enable)
        {
#if UNITY_WEBGL
            this.webLoginBtn.interactable = enable;
            this.webGoogleLoginBtn.interactable = enable;
            this.webFacebookLoginBtn.interactable = enable;
            this.webAppleLoginBtn.interactable = enable;
#else
            this.loginBtn.interactable = enable;
            this.landscapeLoginBtn.interactable = enable;
            this.googleLoginBtn.interactable = enable;
            this.landscapeGoogleLoginBtn.interactable = enable;
            this.facebookLoginBtn.interactable = enable;
            this.landscapeFacebookLoginBtn.interactable = enable;
            this.appleLoginBtn.interactable = enable;
            this.landscapeAppleLoginBtn.interactable = enable;
#endif
        }
        
        private void EnableLogout(bool enable)
        {
#if UNITY_WEBGL
            this.webLogoutBtn.interactable = enable;
#else
            this.logoutBtn.interactable = enable;
            this.landscapeLogoutBtn.interactable = enable;
#endif
        }
        
        private void EnableGetBalance(bool enable)
        {
#if UNITY_WEBGL
            this.webGetBalanceBtn.interactable = enable;
#else
            this.getBalanceBtn.interactable = enable;
            this.landscapeGetBalanceBtn.interactable = enable;
#endif
        }

        private void EnableTestSection(bool enable)
        {
#if UNITY_WEBGL
            this.webTo.interactable = enable;
            this.webAmount.interactable = enable;
            this.webSendNativeCoinTransactionBtn.interactable = enable;
            
            this.webErc20To.interactable = enable;
            this.webErc20Amount.interactable = enable;
            this.webErc20TokenAddress.interactable = enable;
            this.webSendErc20TransactionBtn.interactable = enable;
            
            this.webErc20BalanceInquiryAddress.interactable = enable;
            this.webGetErc20BalanceBtn.interactable = enable;
            
            this.webErc721To.interactable = enable;
            this.webErc721NftAddress.interactable = enable;
            this.webErc721TokenId.interactable = enable;
            this.webSendErc721TransactionBtn.interactable = enable;
            
            this.webErc1155To.interactable = enable;
            this.webErc1155Quantity.interactable = enable;
            this.webErc1155TokenId.interactable = enable;
            this.webErc1155NftAddress.interactable = enable;
            this.webSendErc1155TransactionBtn.interactable = enable;
            
            this.webMessageToSign.interactable = enable;
            this.webSignMessageBtn.interactable = enable;
#else
            this.to.interactable = enable;
            this.amount.interactable = enable;
            this.sendNativeCoinTransactionBtn.interactable = enable;
            this.landscapeTo.interactable = enable;
            this.landscapeAmount.interactable = enable;
            this.landscapeSendNativeCoinTransactionBtn.interactable = enable;
            
            this.erc20To.interactable = enable;
            this.erc20Amount.interactable = enable;
            this.erc20TokenAddress.interactable = enable;
            this.sendErc20TransactionBtn.interactable = enable;
            this.landscapeErc20To.interactable = enable;
            this.landscapeErc20Amount.interactable = enable;
            this.landscapeErc20TokenAddress.interactable = enable;
            this.landscapeSendErc20TransactionBtn.interactable = enable;
            
            this.erc20BalanceInquiryAddress.interactable = enable;
            this.getErc20BalanceBtn.interactable = enable;
            this.landscapeErc20BalanceInquiryAddress.interactable = enable;
            this.landscapeGetErc20BalanceBtn.interactable = enable;
            
            this.erc721To.interactable = enable;
            this.erc721NftAddress.interactable = enable;
            this.erc721TokenId.interactable = enable;
            this.sendErc721TransactionBtn.interactable = enable;
            this.landscapeErc721To.interactable = enable;
            this.landscapeErc721NftAddress.interactable = enable;
            this.landscapeErc721TokenId.interactable = enable;
            this.landscapeSendErc721TransactionBtn.interactable = enable;
            
            this.erc1155To.interactable = enable;
            this.erc1155Quantity.interactable = enable;
            this.erc1155TokenId.interactable = enable;
            this.erc1155NftAddress.interactable = enable;
            this.sendErc1155TransactionBtn.interactable = enable;
            this.landscapeErc1155To.interactable = enable;
            this.landscapeErc1155Quantity.interactable = enable;
            this.landscapeErc1155TokenId.interactable = enable;
            this.landscapeErc1155NftAddress.interactable = enable;
            this.landscapeSendErc1155TransactionBtn.interactable = enable;
            
            this.messageToSign.interactable = enable;
            this.signMessageBtn.interactable = enable;
            this.landscapeMessageToSign.interactable = enable;
            this.landscapeSignMessageBtn.interactable = enable;
#endif
        }
        
        private static void SetDropdown(TMP_Dropdown dropdown, int value)
        {
        
            dropdown.value = value;
        }

        private static void SetInputText(TMP_InputField inputField, string value)
        {
            inputField.text = value;
        }
    }
}