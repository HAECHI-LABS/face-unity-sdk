using System;
using System.Collections.Generic;
using System.Linq;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;

[CreateAssetMenu(fileName = "App State", menuName = "App State")]
public class AppStateSO : ReadOnlyAppState
{
    [Tooltip("A flag whether current dapp is for DevSampleDapp or not")]
    public bool IsDev;
    
    private const string SAMPLE_API_KEY =
        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCS23ncDS7x8nmTuK1FFN0EfYo0vo6xhTBMBNWVbQsufv60X8hv3-TbAQ3JIyMEhLo-c-31oYrvrQ0G2e9j8yvJYEUnLuE-PaABo0y3V5m9g_qdTB5p9eEfqZlDrcUl1zUr4W7rJwFwkTlAFSKOqVCPnm8ozmcMyyrEHgl2AbehrQIDAQAB";

    private const string PM_SAMPLE_API_KEY =
        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDFhGcETETTZVrysoeadS_HGjY3p_jxFkZMR3aE2Sy-d7kjn_WJLYd5f3voEK9mbmLvglt3SGLw08xh_mVFD3rP8QoCNmjkyCbMIeszpCtudAAq1Ar37ZWCKQArWlhXB9EtTU9e3E4FBcBW9vqaPxiT8sk2MBFkCeinNSDHTdQ2uwIDAQAB";

    private const string SAMPLE_PRIVATE_KEY = 
        "MIICdQIBADANBgkqhkiG9w0BAQEFAASCAl8wggJbAgEAAoGBAJLbedwNLvHyeZO4rUUU3QR9ijS+jrGFMEwE1ZVtCy5+/rRfyG/f5NsBDckjIwSEuj5z7fWhiu+tDQbZ72PzK8lgRScu4T49oAGjTLdXmb2D+p1MHmn14R+pmUOtxSXXNSvhbusnAXCROUAVIo6pUI+ebyjOZwzLKsQeCXYBt6GtAgMBAAECgYAEfQa9bf24UVPb6vIIwXl70KZvtD9CN7LhL+ijN4D2+9SnCKJkoPAqrV6Rfixsz+2tSPfF4RkQ+DYEtpZ1dJIq+kNxqRjb7TEHcduYYQwgkJZe2LPd1LS5bnvLGSbGMHHy7+MYNm6M/ghdHoDU+tkYLNFT19BX7MKbBWQPpoH/gQJBAJllv/CZQBhofxLZO0xsM8xcxTo3MFQoos89+Kdym+a8i/WqD49IgIsiK3adn/GCtjSeKJhPlrd5iNUqTBywUk0CQQD1Fdv9q++RmpuhD6LQtGzeeoNzld7xRjWjHVwHvp7/6xeSCyO8sHKydUF/NmV+Jy8vFpJn6b1AvagtgqALanzhAkBaP1eeWLsx4QCp+S3+90W+PPI4HtILIWEv5jjNYws/w7vgC25eEPy3XqINhgzcjNdfu5EMkv6L8S/Eob7nvgCdAkALF4ArTNq8xjiA44pE08WRlA3a7091r+3BghSmLRRZFLSuYV6urXWjafca4MVbHj7ebLEXjtaH1Y2E8cJ4gctBAkBPXs2bRZpI5ULwyYknWaq77gfuappmShgiCv7TUKixt5KqZy8DUU13x/WTUCWjthF/lmgkVq+FvsnG49dF8TM7";

    private const string PM_SAMPLE_PRIVATE_KEY =
        "MIICXAIBAAKBgQDFhGcETETTZVrysoeadS_HGjY3p_jxFkZMR3aE2Sy-d7kjn_WJLYd5f3voEK9mbmLvglt3SGLw08xh_mVFD3rP8QoCNmjkyCbMIeszpCtudAAq1Ar37ZWCKQArWlhXB9EtTU9e3E4FBcBW9vqaPxiT8sk2MBFkCeinNSDHTdQ2uwIDAQABAoGALd1K1pKnQLc-YTDXkCQ6De-mO1JD2iej9z545OxRGYFPelhAebQayzmxGfgV4qErVBZfCtnCL83enbqm5Vxs1ccJi7sbsT8WANf3r2Vs1Ddg-PExdlvPjEwgvItksGh8dcaKAlC2N8vav4psJFHKLM8zfXX5lKZd6FaMscHI_RECQQD_ZR4c6R1PT9Wpnt8SfIwKIkv_4Ez4Z2zA-NN0FxLLO4BxhhhIJwD7OfNyvcupaOg1iM4sCtxVgRBAYpJTDGwNAkEAxfwvaxiv1zDt0uTAArDXgl-ZA5CVidsl_5zhmojMPbbG4enDBGGMZoh8H-G4HIZHOGsoPCkShbJnI8KtLtDT5wJANnHE-J7Na8H2J4HBxjwc0nA7SWH0pqPNsFmbIk1k6URW_lx6wq7l66iP-G7zgrrlfbwbT5JFJEkuQLuCvAeS5QJBAJZWohc73sKUN_NtmlQwvuUT_JgVYUgWLiuEkFeNSzINqcYAgQLBpCBzwXVgLDMUz94KmLneIl99kg44lNM45b8CQEIxv-OnXq0u6tCdp_rhRxVH0ywlA3lXYsXQ8HxbA8dqRiSQKR8AIaOtG8_iCk4ymtYAcFhM9HjR1mtdBkXLflU=";

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
    
    [Tooltip("Current selected network")] 
    [SerializeField] private BlockchainNetwork _currentNetwork;

    [SerializeField] private string _apiKey;
    [SerializeField] private string _privateKey;
    [SerializeField] private string _multiStageId;

    /**
     * FT
     */
    [SerializeField] private string _erc20Balance;

    [Header("Listening on")] 
    [SerializeField] private StringEventChannelSO _onProfileChange;
    [SerializeField] private StringEventChannelSO _onApiKeyChange;
    [SerializeField] private StringEventChannelSO _onPrivateKeyChange;
    [SerializeField] private StringEventChannelSO _onMultiStageIdChange;
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
    [SerializeField] private VoidEventChannelSO _multiStageIdUpdated;
    
    private void OnEnable()
    {
        Debug.Log("OnEnable");
        this._onProfileChange.OnEventRaised += this.SetCurrentProfile;
        this._onApiKeyChange.OnEventRaised += this.SetCurrentApiKey;
        this._onPrivateKeyChange.OnEventRaised += this.SetCurrentPrivateKey;
        this._onMultiStageIdChange.OnEventRaised += this.SetCurrentMultiStageId;
        
        this._onLoginEvent.OnEventRaised += this.OnLogin;
        this._onLogoutEvent.OnEventRaised += this.Initialize;
        this._onBalanceUpdated.OnEventRaised += this.UpdateBalance;
        this._onResultUpdated.OnEventRaised += this.UpdateResult;
        this._onERC20BalanceUpdated.OnEventRaised += this.UpdateERC20Balance;
    }

    private void OnDisable()
    {
        this._onProfileChange.OnEventRaised -= this.SetCurrentProfile;
        this._onApiKeyChange.OnEventRaised -= this.SetCurrentApiKey;
        this._onPrivateKeyChange.OnEventRaised -= this.SetCurrentPrivateKey;
        this._onMultiStageIdChange.OnEventRaised -= this.SetCurrentMultiStageId;
        
        this._onLoginEvent.OnEventRaised -= this.OnLogin;
        this._onLogoutEvent.OnEventRaised -= this.Initialize;
        this._onBalanceUpdated.OnEventRaised -= this.UpdateBalance;
        this._onResultUpdated.OnEventRaised -= this.UpdateResult;
        this._onERC20BalanceUpdated.OnEventRaised -= this.UpdateERC20Balance;
    }

    private void SetCurrentProfile(string env)
    {
        this._currentProfile = this.IsDev ? Profiles.ValueOf(env) : Profile.ProdMainnet;
        this._profileUpdated.RaiseEvent();
    }

    public void SetCurrentNetwork(BlockchainNetwork network)
    {
        this._currentNetwork = network;
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
    
    private void SetCurrentMultiStageId(string value)
    {
        this._multiStageId = value;
        this._multiStageIdUpdated.RaiseEvent();
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
        if (!string.IsNullOrEmpty(this._apiKey))
        {
            return this._apiKey;
        }

        return GetSampleApiKey();
    }

    public string GetSampleApiKey()
    {
        if (this.GetEnv() == Profile.ProdMainnet)
        {
            return PM_SAMPLE_API_KEY;
        }
        return SAMPLE_API_KEY;
    }

    public override string GetPrivateKey()
    {
        if (!string.IsNullOrEmpty(this._apiKey))
        {
            return this._apiKey;
        }

        return GetSamplePrivateKey();
    }

    public string GetSamplePrivateKey()
    {
        if (this.GetEnv() == Profile.ProdMainnet)
        {
            return PM_SAMPLE_PRIVATE_KEY;
        }
        return SAMPLE_PRIVATE_KEY;
    }
    
    public override string GetMultiStageId()
    {
        return this._multiStageId;
    }

    public override Profile GetEnv()
    {
        return this._currentProfile;
    }
    
    public override Blockchain GetBlockchain()
    {
        return BlockchainNetworks.Properties[this._currentNetwork].Blockchain;
    }

    public override BlockchainNetwork GetBlockchainNetwork()
    {
        return this._currentNetwork;
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