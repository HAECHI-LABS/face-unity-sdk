using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class LoginData
{
    public string UserId;
    public string UserAddress;
    public string Balance;
    public string Result;
    
    public LoginData UpdateBalance(string newBalance)
    {
        return new LoginData
        {
            UserId = this.UserId,
            UserAddress = this.UserAddress,
            Balance = newBalance,
            Result = this.Result,
        };
    }
    
    public LoginData UpdateResult(string newResult)
    {
        return new LoginData
        {
            UserId = this.UserId,
            UserAddress = this.UserAddress,
            Balance = this.Balance,
            Result = newResult,
        };
    }
}

[CreateAssetMenu(menuName = "Events/Login Data Event Channel")]
public class LoginDataChannelSO : ScriptableObject
{
    public UnityAction<LoginData> OnEventRaised;

    public void RaiseEvent(LoginData data)
    {
        this.OnEventRaised?.Invoke(data);
    }
}