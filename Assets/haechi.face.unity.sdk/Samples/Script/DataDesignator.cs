using TMPro;
using UnityEngine;

namespace haechi.face.unity.sdk.Samples.Script
{
    public class DataDesignator : MonoBehaviour
    {
        public TMP_Text loggedInAddress, loggedInId, coinBalance, result;
        public TMP_Text landscapeLoggedInAddress, landscapeLoggedInId, landscapeCoinBalance, landscapeResult;
        public TMP_Text instruction, connectOpenseaResult, landscapeInstruction, landscapeConnectOpenseaResult;
        public TMP_InputField erc20Balance, landscapeErc20Balance;

        public void SetLoggedInAddress(string address)
        {
            this.loggedInAddress.text = address.ToLower();
            this.landscapeLoggedInAddress.text = address.ToLower();
        }

        public void SetLoggedInId(string userId)
        {
            this.loggedInId.text = userId;
            this.landscapeLoggedInId.text = userId;
        }

        public void SetCoinBalance(string balance)
        {
            this.coinBalance.text = balance;
            this.landscapeCoinBalance.text = balance;
        }

        public void SetResult(string response)
        {
            this.result.text = response;
            this.landscapeResult.text = response;
        }

        public void SetErc20Balance(string balance)
        {
            this.erc20Balance.text = balance;
            this.landscapeErc20Balance.text = balance;
        }

        public void InitializeDataStatus()
        {
            this.loggedInId.text = null;
            this.loggedInAddress.text = null;
            this.coinBalance.text = null;
            this.result.text = null;
            this.erc20Balance.text = null;
            this.landscapeLoggedInId.text = null;
            this.landscapeLoggedInAddress.text = null;
            this.landscapeCoinBalance.text = null;
            this.landscapeResult.text = null;
            this.landscapeErc20Balance.text = null;
            string message = "You must connect to the network first.";
            this.SetInstruction(message);
            this.SetConnectOpenseaResult(message);
        }
        
        public void SetLoginInstruction()
        {
            this.SetInstruction("Login first to use Face Wallet. To clear session, log out.");
            this.SetConnectOpenseaResult("Login first to connect with Opensea.");
        }
        
        public void SetLogoutInstruction()
        {
            this.SetInstruction("If you log out, you can start from the beginning.");
            this.SetConnectOpenseaResult("You can now connect with Opensea.");
        }

        public void SetConnectOpenseaSucceeded()
        {
            this.SetConnectOpenseaResult("Opensea connected successfully.");
        }
        
        public void SetConnectOpenseaFailed(string exception)
        {
            this.SetConnectOpenseaResult($"Opensea did not connect. Reason: {exception}");
        }

        private void SetInstruction(string text)
        {
            this.instruction.text = text;
            this.landscapeInstruction.text = text;
        }

        private void SetConnectOpenseaResult(string text)
        {
            this.connectOpenseaResult.text = text;
            this.landscapeConnectOpenseaResult.text = text;
        }
    }
}