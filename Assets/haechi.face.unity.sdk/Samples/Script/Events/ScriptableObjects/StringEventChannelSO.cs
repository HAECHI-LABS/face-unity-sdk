using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/String Event Channel")]
public class StringEventChannelSO : ScriptableObject
{
    public UnityAction<String> OnEventRaised;

    public void RaiseEvent(string value)
    {
        this.OnEventRaised?.Invoke(value);
    }
}