using System;
using System.Collections.Generic;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;

[CreateAssetMenu(fileName = "App State", menuName = "App State")]
public class AppStateSO : ReadOnlyAppState
{
    [Tooltip("A flag whether current dapp is for DevSampleDapp or not")]
    public bool IsDev;
    
    private const string SAMPLE_API_KEY =
        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCS23ncDS7x8nmTuK1FFN0EfYo0vo6xhTBMBNWVbQsufv60X8hv3-TbAQ3JIyMEhLo-c-31oYrvrQ0G2e9j8yvJYEUnLuE-PaABo0y3V5m9g_qdTB5p9eEfqZlDrcUl1zUr4W7rJwFwkTlAFSKOqVCPnm8ozmcMyyrEHgl2AbehrQIDAQAB";
    private const string SAMPLE_PRIVATE_KEY = 
        "MIICdQIBADANBgkqhkiG9w0BAQEFAASCAl8wggJbAgEAAoGBAJLbedwNLvHyeZO4rUUU3QR9ijS+jrGFMEwE1ZVtCy5+/rRfyG/f5NsBDckjIwSEuj5z7fWhiu+tDQbZ72PzK8lgRScu4T49oAGjTLdXmb2D+p1MHmn14R+pmUOtxSXXNSvhbusnAXCROUAVIo6pUI+ebyjOZwzLKsQeCXYBt6GtAgMBAAECgYAEfQa9bf24UVPb6vIIwXl70KZvtD9CN7LhL+ijN4D2+9SnCKJkoPAqrV6Rfixsz+2tSPfF4RkQ+DYEtpZ1dJIq+kNxqRjb7TEHcduYYQwgkJZe2LPd1LS5bnvLGSbGMHHy7+MYNm6M/ghdHoDU+tkYLNFT19BX7MKbBWQPpoH/gQJBAJllv/CZQBhofxLZO0xsM8xcxTo3MFQoos89+Kdym+a8i/WqD49IgIsiK3adn/GCtjSeKJhPlrd5iNUqTBywUk0CQQD1Fdv9q++RmpuhD6LQtGzeeoNzld7xRjWjHVwHvp7/6xeSCyO8sHKydUF/NmV+Jy8vFpJn6b1AvagtgqALanzhAkBaP1eeWLsx4QCp+S3+90W+PPI4HtILIWEv5jjNYws/w7vgC25eEPy3XqINhgzcjNdfu5EMkv6L8S/Eob7nvgCdAkALF4ArTNq8xjiA44pE08WRlA3a7091r+3BghSmLRRZFLSuYV6urXWjafca4MVbHj7ebLEXjtaH1Y2E8cJ4gctBAkBPXs2bRZpI5ULwyYknWaq77gfuappmShgiCv7TUKixt5KqZy8DUU13x/WTUCWjthF/lmgkVq+FvsnG49dF8TM7";
    
    [SerializeField] private ContractsSO _contracts;
    
    /**
     * Main
     */
    [SerializeField] private LoginData _loginData;
    
    /**
     * Connect and Login
     */
    
    [Tooltip("All environments that can be seen in the Connect page")]
    [SerializeField] private List<string> _profiles;
    
    [Tooltip("Current selected environment")] 
    [SerializeField] private Profile _currentProfile;
    
    [Tooltip("All blockchains that can be seen in the Connect page")]
    [SerializeField] private List<Blockchain> _blockchains;
    
    [Tooltip("Current selected blockchain")] 
    [SerializeField] private Blockchain _currentBlockchain;

    [SerializeField] private string _apiKey;
    [SerializeField] private string _privateKey;

    /**
     * FT
     */
    [SerializeField] private string _erc20Balance;

    [Header("Listening on")] 
    [SerializeField] private StringEventChannelSO _onProfileChange;
    [SerializeField] private StringEventChannelSO _onBlockchainChange;
    [SerializeField] private StringEventChannelSO _onApiKeyChange;
    [SerializeField] private StringEventChannelSO _onPrivateKeyChange;
    [SerializeField] private LoginDataChannelSO _onLoginEvent;
    [SerializeField] private VoidEventChannelSO _onLogoutEvent;
    [SerializeField] private StringEventChannelSO _onBalanceUpdated;
    [SerializeField] private StringEventChannelSO _onResultUpdated;
    [SerializeField] private StringEventChannelSO _onERC20BalanceUpdated;

    [Header("Broadcast to")] 
    [SerializeField] private VoidEventChannelSO _profileUpdated;
    [SerializeField] private VoidEventChannelSO _blockchainUpdated;
    [SerializeField] private VoidEventChannelSO _apiKeyUpdated;
    [SerializeField] private VoidEventChannelSO _privateKeyUpdated;
    
    private void OnEnable()
    {
        this._onProfileChange.OnEventRaised += this.SetCurrentProfile;
        this._onBlockchainChange.OnEventRaised += this.SetCurrentBlockchain;
        this._onApiKeyChange.OnEventRaised += this.SetCurrentApiKey;
        this._onPrivateKeyChange.OnEventRaised += this.SetCurrentPrivateKey;
        
        this._onLoginEvent.OnEventRaised += this.OnLogin;
        this._onLogoutEvent.OnEventRaised += this.Initialize;
        this._onBalanceUpdated.OnEventRaised += this.UpdateBalance;
        this._onResultUpdated.OnEventRaised += this.UpdateResult;
        this._onERC20BalanceUpdated.OnEventRaised += this.UpdateERC20Balance;
    }

    private void OnDisable()
    {
        this._onProfileChange.OnEventRaised -= this.SetCurrentProfile;
        this._onBlockchainChange.OnEventRaised -= this.SetCurrentBlockchain;
        this._onApiKeyChange.OnEventRaised -= this.SetCurrentApiKey;
        this._onPrivateKeyChange.OnEventRaised -= this.SetCurrentPrivateKey;
        
        this._onLoginEvent.OnEventRaised -= this.OnLogin;
        this._onLogoutEvent.OnEventRaised -= this.Initialize;
        this._onBalanceUpdated.OnEventRaised -= this.UpdateBalance;
        this._onResultUpdated.OnEventRaised -= this.UpdateResult;
        this._onERC20BalanceUpdated.OnEventRaised -= this.UpdateERC20Balance;
    }

    private void SetCurrentProfile(string env)
    {
        this._currentProfile = Profiles.ValueOf(env);
        this._profileUpdated.RaiseEvent();
    }

    private void SetCurrentBlockchain(string blockchain)
    {
        this._currentBlockchain = Blockchains.ValueOf(blockchain);
        this._blockchainUpdated.RaiseEvent();
    }

    private void SetCurrentApiKey(string value)
    {
        this._apiKey = value;
        this._apiKeyUpdated.RaiseEvent();
    }
    
    private void SetCurrentPrivateKey(string value)
    {
        this._privateKey = value;
        this._privateKeyUpdated.RaiseEvent();
    }
    
    private void OnLogin(LoginData loginData)
    {
        this._loginData = loginData;
    }
    
    private void UpdateBalance(string newBalance)
    {
        this._loginData = this._loginData.UpdateBalance(newBalance);
    }

    private void UpdateERC20Balance(string newBalance)
    {
        this._erc20Balance = newBalance;
    }

    private void UpdateResult(string newResult)
    {
        this._loginData = this._loginData.UpdateResult(newResult);
    }

    public override void Initialize()
    {
        Debug.Log("[AppStateSO] Initialize");
        this._loginData = null;
        this._erc20Balance = default;
        this._currentProfile = this.IsDev ? Profile.Local : Profile.ProdTest;
    }

    public override List<string> GetAllEnvironments()
    {
        return this._profiles;
    }
    
    public override List<Blockchain> GetAllBlockchains()
    {
        return this._blockchains;
    }

    public override string GetApiKey()
    {
        return string.IsNullOrEmpty(this._apiKey) ? SAMPLE_API_KEY : this._apiKey;
    }

    public override string GetPrivateKey()
    {
        return string.IsNullOrEmpty(this._privateKey) ? SAMPLE_PRIVATE_KEY : this._privateKey;
    }

    public override Profile GetEnv()
    {
        return this._currentProfile;
    }
    
    public override Blockchain GetBlockchain()
    {
        return this._currentBlockchain;
    }

    public override BlockchainNetwork GetBlockchainNetwork()
    {
        return BlockchainNetworks.GetNetwork(this._currentBlockchain, this._currentProfile);
    }

    public override string GetScheme()
    {
        return Application.identifier == "xyz.facewallet.unity.app" ? "faceunity" : "faceunitydev";
    }

    public override LoginData GetLoginData()
    {
        return this._loginData;
    }

    public override bool LoggedIn()
    {
        if (this._loginData == null)
        {
            return false;
        }

        return !String.IsNullOrEmpty(this._loginData.UserId) && !String.IsNullOrEmpty(this._loginData.UserAddress);
    }

    public ContractData GetCurrentBlockchainContractData()
    {
        if (!this.LoggedIn())
        {
            return null;
        }

        return this._contracts.ContractAddresses(this.GetBlockchainNetwork());
    }

    public override string GetERC20Balance()
    {
        return this._erc20Balance;
    }
}