using UnityEngine;
using UnityEngine.Events;


public class FTQueryData
{
    public string ContractAddress;
}

[CreateAssetMenu(menuName = "Events/FT Query Data Event Channel")]
public class FTQueryDataChannelSO : ScriptableObject
{
    public UnityAction<FTQueryData> OnEventRaised;

    public void RaiseEvent(FTQueryData data)
    {
        this.OnEventRaised?.Invoke(data);
    }
}