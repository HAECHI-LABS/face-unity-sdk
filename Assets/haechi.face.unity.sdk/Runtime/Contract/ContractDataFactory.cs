using System;
using System.Data;
using haechi.face.unity.sdk.Runtime.Utils;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Contract
{
    public class ContractDataFactory
    {
        private readonly Web3 _web3;

        public ContractDataFactory(Web3 web3)
        {
            this._web3 = web3;
        }

        public string CreateErc20SendData(string tokenAddress, string to, string amount, int decimals = 18)
        {
            try
            {
                Nethereum.Contracts.Contract erc20 = this._web3.Eth.GetContract(Abi.erc20ABI, tokenAddress);
                Function transferFunction = erc20.GetFunction("transfer");
                string value =
                    NumberFormatter.DecimalStringToHexadecimal(
                        NumberFormatter.DecimalStringToIntegerString(amount, decimals));
                return transferFunction.GetData(to, value);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw new DataException("Failed to create erc20 send data");
            }
        }

        public string CreateErc20GetBalanceData(string tokenAddress, string address)
        {
            try
            {
                Nethereum.Contracts.Contract erc20 = this._web3.Eth.GetContract(Abi.erc20ABI, tokenAddress);
                Function transferFunction = erc20.GetFunction("balanceOf");
                return transferFunction.GetData(address);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw new DataException("Failed to create erc20 get balance data");
            }
        }

        public string CreateErc20GetDecimalsData(string tokenAddress)
        {
            try
            {
                Nethereum.Contracts.Contract erc20 = this._web3.Eth.GetContract(Abi.erc20ABI, tokenAddress);
                Function transferFunction = erc20.GetFunction("decimals");
                return transferFunction.GetData();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw new DataException("Failed to create erc20 get decimals data");
            }
        }

        public string CreateErc721SendData(string nftAddress, string from, string to, string tokenId)
        {
            try
            {
                Nethereum.Contracts.Contract erc721 = this._web3.Eth.GetContract(Abi.erc721ABI, nftAddress);
                Function transferFunction = erc721.GetFunction("safeTransferFrom");
                return transferFunction.GetData(from, to, int.Parse(tokenId));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw new DataException("Failed to create erc721 send data");
            }
        }

        public string CreateErc1155SendBatchData(string nftAddress, string from, string to, string tokenId,
            string quantity)
        {
            try
            {
                Nethereum.Contracts.Contract erc1155 = this._web3.Eth.GetContract(Abi.erc1155ABI, nftAddress);
                Function transferFunction = erc1155.GetFunction("safeTransferFrom");
                return transferFunction.GetData(from, to, int.Parse(tokenId), int.Parse(quantity),
                    new HexBigInteger(0).ToHexByteArray());
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw new DataException("Failed to create erc1155 send data");
            }
        }
    }
}