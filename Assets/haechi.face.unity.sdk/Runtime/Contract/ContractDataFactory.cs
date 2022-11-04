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
        
        /// <param name="web3">Web3</param>
        public ContractDataFactory(Web3 web3)
        {
            this._web3 = web3;
        }

        /// <summary>
        /// Create sending ERC20 data.
        /// </summary>
        /// <param name="tokenAddress">ERC20 token contract address.</param>
        /// <param name="to">Receiver address.</param>
        /// <param name="amount">Token amount to send.</param>
        /// <param name="decimals">Token decimals.</param>
        /// <returns>ABI encoded data.</returns>
        /// <exception cref="DataException">Returns error if data creation fails.</exception>
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
            catch (System.Exception e)
            {
                Debug.LogError(e);
                throw new DataException("Failed to create erc20 send data");
            }
        }

        /// <summary>
        /// Create ERC20 balance inquiry data.
        /// </summary>
        /// <param name="tokenAddress">ERC20 token contract address.</param>
        /// <param name="address">Sending wallet's address.</param>
        /// <returns>ABI encoded data.</returns>
        /// <exception cref="DataException">Returns error if data creation fails.</exception>
        public string CreateErc20GetBalanceData(string tokenAddress, string address)
        {
            try
            {
                Nethereum.Contracts.Contract erc20 = this._web3.Eth.GetContract(Abi.erc20ABI, tokenAddress);
                Function transferFunction = erc20.GetFunction("balanceOf");
                return transferFunction.GetData(address);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                throw new DataException("Failed to create erc20 get balance data");
            }
        }

        /// <summary>
        /// Create ERC20 decimals inquiry data.
        /// </summary>
        /// <param name="tokenAddress">ERC20 token contract address.</param>
        /// <returns>ABI encoded data.</returns>
        /// <exception cref="DataException">Returns error if data creation fails.</exception>
        public string CreateErc20GetDecimalsData(string tokenAddress)
        {
            try
            {
                Nethereum.Contracts.Contract erc20 = this._web3.Eth.GetContract(Abi.erc20ABI, tokenAddress);
                Function transferFunction = erc20.GetFunction("decimals");
                return transferFunction.GetData();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                throw new DataException("Failed to create erc20 get decimals data");
            }
        }

        /// <summary>
        /// Create sending ERC721 data.
        /// </summary>
        /// <param name="nftAddress">ERC721 token contract address.</param>
        /// <param name="from">Sending wallet's address.</param>
        /// <param name="to">Receiver address.</param>
        /// <param name="tokenId">NFT token ID.</param>
        /// <returns>ABI encoded data.</returns>
        /// <exception cref="DataException">Returns error if data creation fails.</exception>
        public string CreateErc721SendData(string nftAddress, string from, string to, string tokenId)
        {
            try
            {
                Nethereum.Contracts.Contract erc721 = this._web3.Eth.GetContract(Abi.erc721ABI, nftAddress);
                Function transferFunction = erc721.GetFunction("safeTransferFrom");
                return transferFunction.GetData(from, to, int.Parse(tokenId));
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                throw new DataException("Failed to create erc721 send data");
            }
        }

        /// <summary>
        /// Create sending ERC1155 data.
        /// </summary>
        /// <param name="nftAddress">ERC1155 token contract address.</param>
        /// <param name="from">Sending wallet's address.</param>
        /// <param name="to">Receiver address.</param>
        /// <param name="tokenId">NFT token ID.</param>
        /// <param name="quantity">Quantity of sending ERC1155 nft.</param>
        /// <returns>ABI encoded data.</returns>
        /// <exception cref="DataException">Returns error if data creation fails.</exception>
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
            catch (System.Exception e)
            {
                Debug.LogError(e);
                throw new DataException("Failed to create erc1155 send data");
            }
        }
    }
}