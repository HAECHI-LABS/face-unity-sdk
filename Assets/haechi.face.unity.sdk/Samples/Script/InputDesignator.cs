using System;
using System.Linq;
using haechi.face.unity.sdk.Runtime.Type;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace haechi.face.unity.sdk.Samples.Script
{
    public class InputDesignator : MonoBehaviour
    {
        [SerializeField] internal Button initializeBtn, loginBtn, logoutBtn, getBalanceBtn;
        [SerializeField] internal Button landscapeInitializeBtn, landscapeLoginBtn, landscapeLogoutBtn, landscapeGetBalanceBtn;

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
        
        public TMP_Dropdown profileDrd, blockchainDrd, networkDrd;
        public TMP_InputField apiKey;
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

        private void Start()
        {
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
        
        private void EnableConnectWalletSection(bool enable)
        {
            if (this.profileDrd != null)
            {
                this.profileDrd.interactable = enable;
                this.landscapeProfileDrd.interactable = enable;
            }
            
            if (this.blockchainDrd != null)
            {
                this.blockchainDrd.interactable = enable;
                this.landscapeBlockchainDrd.interactable = enable;
            }
            
            if (this.networkDrd != null)
            {
                this.networkDrd.interactable = enable;
                this.landscapeNetworkDrd.interactable = enable;
            }
            
            if (this.apiKey != null)
            {
                this.apiKey.interactable = enable;
                this.landscapeApiKey.interactable = enable;
            }
            
            this.initializeBtn.interactable = enable;
            this.landscapeInitializeBtn.interactable = enable;
        }

        private void EnableLogin(bool enable)
        {
            this.loginBtn.interactable = enable;
            this.landscapeLoginBtn.interactable = enable;
        }
        
        private void EnableLogout(bool enable)
        {
            this.logoutBtn.interactable = enable;
            this.landscapeLogoutBtn.interactable = enable;
        }
        
        private void EnableGetBalance(bool enable)
        {
            this.getBalanceBtn.interactable = enable;
            this.landscapeGetBalanceBtn.interactable = enable;
        }

        private void EnableTestSection(bool enable)
        {
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