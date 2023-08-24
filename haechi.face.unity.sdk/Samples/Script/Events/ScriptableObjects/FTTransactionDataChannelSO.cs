using UnityEngine;
using UnityEngine.Events;


public class FTTransactionData
{
    public string Amount;
    public string ReceiverAddress;
    public string ContractAddress;
}

[CreateAssetMenu(menuName = "Events/FT Transaction Data Event Channel")]
public class FTTransactionDataChannelSO : ScriptableObject
{
    public UnityAction<FTTransactionData> OnEventRaised;

    public void RaiseEvent(FTTransactionData data)
    {
        this.OnEventRaised?.Invoke(data);
    }
}