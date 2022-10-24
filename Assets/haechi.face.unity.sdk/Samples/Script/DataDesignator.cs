using TMPro;
using UnityEngine;

namespace haechi.face.unity.sdk.Samples.Script
{
    public class DataDesignator : MonoBehaviour
    {
        public TMP_Text loggedInAddress, loggedInId, coinBalance, result;
        public TMP_Text landscapeLoggedInAddress, landscapeLoggedInId, landscapeCoinBalance, landscapeResult;
        public TMP_InputField erc20Balance, landscapeErc20Balance;

        public void SetLoggedInAddress(string address)
        {
            this.loggedInAddress.text = address;
            this.landscapeLoggedInAddress.text = address;
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

        public void Initialize()
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
        }
    }
}