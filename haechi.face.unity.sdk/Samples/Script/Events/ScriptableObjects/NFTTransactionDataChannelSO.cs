using UnityEngine;
using UnityEngine.Events;


public class NFTTransactionData
{
    public string Amount;
    public string ReceiverAddress;
    public string ContractAddress;
    public string TokenId;
    public string Quantity;
}

[CreateAssetMenu(menuName = "Events/NFT Transaction Data Event Channel")]
public class NFTTransactionDataChannelSO : ScriptableObject
{
    public UnityAction<NFTTransactionData> OnEventRaised;

    public void RaiseEvent(NFTTransactionData data)
    {
        this.OnEventRaised?.Invoke(data);
    }
}