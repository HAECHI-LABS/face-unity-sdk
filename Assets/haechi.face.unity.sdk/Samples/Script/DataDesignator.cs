using TMPro;
using UnityEngine;

namespace haechi.face.unity.sdk.Samples.Script
{
    public class DataDesignator : MonoBehaviour
    {
        public TMP_Text loggedInAddress, loggedInId, coinBalance, result;
        public TMP_Text landscapeLoggedInAddress, landscapeLoggedInId, landscapeCoinBalance, landscapeResult;
        public TMP_Text webLoggedInAddress, webLoggedInId, webCoinBalance, webResult;
        public TMP_Text instruction, landscapeInstruction, webInstruction;
        public TMP_InputField erc20Balance, landscapeErc20Balance, webErc20Balance;

        public void SetLoggedInAddress(string address)
        {
            this.loggedInAddress.text = address.ToLower();
            this.landscapeLoggedInAddress.text = address.ToLower();
            this.webLoggedInAddress.text = address.ToLower();
        }

        public void SetLoggedInId(string userId)
        {
            this.loggedInId.text = userId;
            this.landscapeLoggedInId.text = userId;
            this.webLoggedInId.text = userId;
        }

        public void SetCoinBalance(string balance)
        {
            this.coinBalance.text = balance;
            this.landscapeCoinBalance.text = balance;
            this.webCoinBalance.text = balance;
        }

        public void SetResult(string response)
        {
            this.result.text = response;
            this.landscapeResult.text = response;
            this.webResult.text = response;
        }

        public void SetErc20Balance(string balance)
        {
            this.erc20Balance.text = balance;
            this.landscapeErc20Balance.text = balance;
            this.webErc20Balance.text = balance;
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
            this.webLoggedInId.text = null;
            this.webLoggedInAddress.text = null;
            this.webCoinBalance.text = null;
            this.webResult.text = null;
            this.webErc20Balance.text = null;
            this.SetInstruction("You must connect to the network first.");
        }
        
        public void SetLoginInstruction()
        {
            this.SetInstruction("Login first to use Face Wallet. To clear session, log out.");
        }
        
        public void SetLoginAfterSignupInstruction()
        {
            this.SetInstruction("Successfully signed up. Logout and login again.");
        }

        public void SetLogoutInstruction()
        {
            this.SetInstruction("If you log out, you can start from the beginning.");
        }
        
        private void SetInstruction(string text)
        {
            this.instruction.text = text;
            this.landscapeInstruction.text = text;
            this.webInstruction.text = text;
        }
    }
}