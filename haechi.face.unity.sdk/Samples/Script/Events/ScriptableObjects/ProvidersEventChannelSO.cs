using System.Collections.Generic;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Providers Event Channel")]
public class ProvidersEventChannelSO : ScriptableObject
{
    public UnityAction<List<LoginProviderType>> OnEventRaised;

    public void RaiseEvent(List<LoginProviderType> providers)
    {
        this.OnEventRaised?.Invoke(providers);
    }
}