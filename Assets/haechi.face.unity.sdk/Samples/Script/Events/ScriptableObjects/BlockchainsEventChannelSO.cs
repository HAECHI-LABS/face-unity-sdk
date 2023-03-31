using System.Collections.Generic;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Blockchains Event Channel")]
public class BlockchainsEventChannelSO : ScriptableObject
{
    public UnityAction<List<Blockchain>> OnEventRaised;

    public void RaiseEvent(List<Blockchain> blockchains)
    {
        this.OnEventRaised?.Invoke(blockchains);
    }
}