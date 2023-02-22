using System;
using System.Collections.Generic;
using haechi.face.unity.sdk.Runtime;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;

[Serializable]
public class ContractData
{
    [SerializeField] private BlockchainNetwork blockchainNetwork = default;
    [SerializeField] 
    private string erc20Decimal18ContractAddress = default;
    [SerializeField] 
    private string erc20Decimal6ContractAddress = default;
    [SerializeField] 
    private string erc721ContractAddress = default;
    [SerializeField] 
    private string erc1155ContractAddress = default;

    public BlockchainNetwork BlockchainNetworkNetwork => this.blockchainNetwork;
    public string ERC20Decimal18 => this.erc20Decimal18ContractAddress;
    public string ERC20Decimal6 => this.erc20Decimal6ContractAddress;
    public string ERC721 => this.erc721ContractAddress;
    public string ERC1155 => this.erc1155ContractAddress;

}

[CreateAssetMenu(fileName = "Contracts", menuName = "Face/Contracts")]
public class ContractsSO : ScriptableObject
{
    [SerializeField] private List<ContractData> contractDataList = new List<ContractData>();

    public ContractData ContractAddresses(FaceSettings.Parameters parameters)
    {
        foreach (ContractData contractData in this.contractDataList)
        {
            if (parameters.Network.Equals(contractData.BlockchainNetworkNetwork))
            {
                return contractData;
            }
        }

        return null;
    }
}