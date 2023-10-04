using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.BoraPortal;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Exception;
using haechi.face.unity.sdk.Runtime.Type;
using haechi.face.unity.sdk.Runtime.Utils;
using haechi.face.unity.sdk.Samples.Script;
using Nethereum.Util;
using Newtonsoft.Json;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Face))]
[RequireComponent(typeof(ActionQueue))]
public class FaceWalletManager : MonoBehaviour
{
    private Face _face;
    
    private ActionQueue _actionQueue;

    [SerializeField] private ReadOnlyAppState _appState;
    
    [Header("Listening on")]
    /** Connect & Login */
    [SerializeField] private VoidEventChannelSO _onConnect;
    [SerializeField] private VoidEventChannelSO _onLogin;
    [SerializeField] private ProvidersEventChannelSO _onLoginWithSelectedProviders;
    [SerializeField] private StringEventChannelSO _onSocialLogin;
    [SerializeField] private VoidEventChannelSO _onGetBalance;
    [SerializeField] private VoidEventChannelSO _onLogout;
    [SerializeField] private VoidEventChannelSO _onSwitchNetwork;
    
    /** Wallet Home */
    [SerializeField] private VoidEventChannelSO _onOpenAllBlockchainWalletHome;
    [SerializeField] private BlockchainsEventChannelSO _onOpenSelectedBlockchainWalletHome;

    /** FT */
    [SerializeField] private FTTransactionDataChannelSO _onSendPlatformCoin;
    [SerializeField] private FTTransactionDataChannelSO _onSendERC20;
    [SerializeField] private FTQueryDataChannelSO _onGetBalanceERC20;

    /** NFT */
    [SerializeField] private NFTTransactionDataChannelSO _onSendERC721;

    [SerializeField] private NFTTransactionDataChannelSO _onSendERC1155;

    /** BORA */
    [SerializeField] private VoidEventChannelSO _onConnectBora;
    [SerializeField] private VoidEventChannelSO _onCheckIsConnectedBora;

    /** Sign Message */
    [SerializeField] private StringEventChannelSO _onSignMessage;
    
    /** Wallet Connect */
    [SerializeField] private VoidEventChannelSO _onWalletConnect;

    [Header("Broadcast to")] 
    [SerializeField] private VoidEventChannelSO _connected;

    [SerializeField] private StringEventChannelSO _exceptionOccurred;

    [SerializeField] private LoginDataChannelSO _logined;
    [SerializeField] private VoidEventChannelSO _logouted;

    [SerializeField] private StringEventChannelSO _balanceUpdated;
    [SerializeField] private VoidEventChannelSO _networkSwitched;
    [SerializeField] private StringEventChannelSO _resultUpdated;
    
    [SerializeField] private StringEventChannelSO _updateERC20Balance;

    private void Awake()
    {
        this._face = this.GetComponent<Face>();
        this._actionQueue = this.GetComponent<ActionQueue>();
    }

    private void OnEnable()
    {
        this._onConnect.OnEventRaised += this.Connect;
        this._onLogin.OnEventRaised += this.Login;
        this._onLoginWithSelectedProviders.OnEventRaised += this.Login;
        this._onSocialLogin.OnEventRaised += this.SocialLogin;
        this._onGetBalance.OnEventRaised += this.GetBalance;
        this._onLogout.OnEventRaised += this.Logout;
        this._onSwitchNetwork.OnEventRaised += this.SwitchNetwork;
        this._onOpenAllBlockchainWalletHome.OnEventRaised += this.OpenWalletHome;
        this._onOpenSelectedBlockchainWalletHome.OnEventRaised += this.OpenWalletHome;
        this._onSendPlatformCoin.OnEventRaised += this.SendPlatformCoin;
        this._onSendERC20.OnEventRaised += this.SendERC20;
        this._onGetBalanceERC20.OnEventRaised += this.GetERC20Balance;
        this._onSendERC721.OnEventRaised += this.SendERC721Transaction;
        this._onSendERC1155.OnEventRaised += this.SendERC1155Transaction;
        this._onConnectBora.OnEventRaised += this.ConnectBora;
        this._onCheckIsConnectedBora.OnEventRaised += this.IsBoraConnected;
        this._onSignMessage.OnEventRaised += this.SignMessage;
        this._onWalletConnect.OnEventRaised += this.WalletConnect;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        this._appState.Initialize();
    }

    private void OnDisable()
    {
        this._onConnect.OnEventRaised -= this.Connect;
        this._onLogin.OnEventRaised -= this.Login;
        this._onLoginWithSelectedProviders.OnEventRaised -= this.Login;
        this._onSocialLogin.OnEventRaised -= this.SocialLogin;
        this._onGetBalance.OnEventRaised -= this.GetBalance;
        this._onLogout.OnEventRaised -= this.Logout;
        this._onSwitchNetwork.OnEventRaised -= this.SwitchNetwork;
        this._onOpenAllBlockchainWalletHome.OnEventRaised -= this.OpenWalletHome;
        this._onOpenSelectedBlockchainWalletHome.OnEventRaised -= this.OpenWalletHome;
        this._onSendPlatformCoin.OnEventRaised -= this.SendPlatformCoin;
        this._onSendERC20.OnEventRaised -= this.SendERC20;
        this._onGetBalanceERC20.OnEventRaised -= this.GetERC20Balance;
        this._onSendERC721.OnEventRaised -= this.SendERC721Transaction;
        this._onSendERC1155.OnEventRaised -= this.SendERC1155Transaction;
        this._onConnectBora.OnEventRaised -= this.ConnectBora;
        this._onCheckIsConnectedBora.OnEventRaised -= this.IsBoraConnected;
        this._onSignMessage.OnEventRaised -= this.SignMessage;
        this._onWalletConnect.OnEventRaised -= this.WalletConnect;
    }
    
    private void Connect()
    {
        this._face.Initialize(new FaceSettings.Parameters
        {
            ApiKey = this._appState.GetApiKey(),
            Environment = this._appState.GetEnv(),
            Network = this._appState.GetBlockchainNetwork(),
            Scheme = this._appState.GetScheme(),
        });
        this._connected.RaiseEvent();
    }

    private void SwitchNetwork()
    {
        Task<string> responseTask = this._switchNetworkAndGetBalanceAsync();
        this._actionQueue.Enqueue(responseTask, balance =>
        {
            this._networkSwitched.RaiseEvent(); // TODO: update corresponding blockchain network contract address
            this._balanceUpdated.RaiseEvent(balance);
        }, this._defaultExceptionHandler);
    }
    
    private async Task<string> _switchNetworkAndGetBalanceAsync()
    {
        await this._face.Wallet().SwitchNetwork(this._appState.GetBlockchainNetwork());
        return await this._getBalance(this._appState.GetLoginData().UserAddress);
    }

    private void Login()
    {
        Task<LoginResult> responseTask = this._loginAndGetBalanceAsync(null);

        this._actionQueue.Enqueue(responseTask, response =>
        {
            this._logined.RaiseEvent(new LoginData
            {
                UserId = response.userId,
                UserAddress = response.userAddress,
                Balance = response.balance,
                Result = $"UserVerificationToken: {response.userVerificationToken}"
            });
        }, this._defaultExceptionHandler);
    }
    
    private void Login(List<LoginProviderType> providers)
    {
        Task<LoginResult> responseTask = this._loginAndGetBalanceAsync(providers);

        this._actionQueue.Enqueue(responseTask, response =>
        {
            this._logined.RaiseEvent(new LoginData
            {
                UserId = response.userId,
                UserAddress = response.userAddress,
                Balance = response.balance,
                Result = $"UserVerificationToken: {response.userVerificationToken}"
            });
        }, this._defaultExceptionHandler);
    }

    private void SocialLogin(string loginProviderType)
    {
        this._directSocialLoginAndGetBalance(loginProviderType);
    }
    
    private void Logout()
    {
        Task<FaceRpcResponse> responseTask = this._face.Auth().Logout();

        this._actionQueue.Enqueue(responseTask, response =>
        {
            string result = JsonConvert.SerializeObject(response);
            Debug.Log($"Result: {result}");
            this._face.Disconnect();
            this._logouted.RaiseEvent();
        }, this._defaultExceptionHandler);
    }
    
    private void GetBalance()
    {
        this._validateIsLoggedIn();

        LoginData loginData = this._appState.GetLoginData();
        Task<string> responseTask = this._getBalance(loginData.UserAddress);
            
        this._actionQueue.Enqueue(responseTask, response =>
        {
            this._balanceUpdated.RaiseEvent(response);
        }, this._defaultExceptionHandler);
    }
    
    private void OpenWalletHome()
    {
        Task<FaceRpcResponse> responseTask = this._face.Wallet().OpenHome();
            
        this._actionQueue.Enqueue(responseTask, response =>
        {
            string result = JsonConvert.SerializeObject(response);
            Debug.Log($"Result: {result}");
        }, this._defaultExceptionHandler);
    }

    private void OpenWalletHome(List<Blockchain> blockchains)
    {
        Profile profile = this._appState.GetEnv();
            
        List<BlockchainNetwork> blockchainNetworks = blockchains.ConvertAll(blockchain =>
        {
            return BlockchainNetworks.GetNetwork(blockchain, profile);
        });
            
        Task<FaceRpcResponse> responseTask = this._face.Wallet().OpenHome(OpenHomeOption.Of(blockchainNetworks));
            
        this._actionQueue.Enqueue(responseTask, response =>
        {
            string result = JsonConvert.SerializeObject(response);
            Debug.Log($"Result: {result}");
        }, this._defaultExceptionHandler);
    }

    private void SendPlatformCoin(FTTransactionData data)
    {
        Task<TransactionResult> transactionTask = this._sendTransactionTask(
            data.ReceiverAddress,
            () => null,
            NumberFormatter.DecimalStringToHexadecimal(
                NumberFormatter.DecimalStringToIntegerString(data.Amount, 18))
        );
        this._sendTransactionQueue(transactionTask);
    }

    private void GetERC20Balance(FTQueryData data)
    {
        this._validateIsLoggedIn();

        string txData = this._face.dataFactory.CreateErc20GetBalanceData(
            data.ContractAddress, this._appState.GetLoginData().UserAddress);
        RawTransaction request =
            new RawTransaction(null, data.ContractAddress, "0x0", txData);

        Task<FaceRpcResponse> responseTask = this._face.Wallet().Call(request);

        this._actionQueue.Enqueue(responseTask, response =>
        {
            string result = JsonConvert.SerializeObject(response);
            Debug.Log($"Result: {result}");
            this._updateERC20Balance.RaiseEvent(NumberFormatter.DivideHexWithDecimals(response.CastResult<string>(), 18));
        }, this._defaultExceptionHandler);
    }

    private void SendERC20(FTTransactionData data)
    {
        this._sendTransactionQueue(this._sendErc20TransactionTask(data));
    }

    private void SendERC721Transaction(NFTTransactionData data)
    {
        Task<TransactionResult> transactionTask = this._sendTransactionTask(
            data.ContractAddress,
            () => this._face.dataFactory.CreateErc721SendData(
                data.ContractAddress,
                this._appState.GetLoginData().UserAddress,
                data.ReceiverAddress,
                data.TokenId)
        );
        this._sendTransactionQueue(transactionTask);
    }

    private void SendERC1155Transaction(NFTTransactionData data)
    {
        Task<TransactionResult> transactionTask = this._sendTransactionTask(
            data.ContractAddress,
            () => this._face.dataFactory.CreateErc1155SendBatchData(
                data.ContractAddress,
                this._appState.GetLoginData().UserAddress,
                data.ReceiverAddress,
                data.TokenId,
                data.Quantity)
        );
        this._sendTransactionQueue(transactionTask);
    }
    
    public void ConnectBora()
    {
        Debug.Log("[FaceWalletManager] ConnectBora(..)");
        this._validateIsLoggedIn();
        string bappUsn = this._appState.GetLoginData().UserId;
        string signatureMessage = $"{bappUsn}:{this._appState.GetLoginData().UserAddress}";
        string signature = RSASigner.Sign(this._appState.GetPrivateKey(), signatureMessage);
        BoraPortalConnectRequest request = new BoraPortalConnectRequest(
            bappUsn,
            signature);
        Task<FaceRpcResponse> responseTask = this._face.Bora().Connect(request);

        this._actionQueue.Enqueue(responseTask, response =>
        {
            string result = JsonConvert.SerializeObject(response);
            Debug.Log($"Result: {result}");
            BoraPortalConnectStatusResponse statusResponse = response.CastResult<BoraPortalConnectStatusResponse>();
            this._resultUpdated.RaiseEvent($"Bora Connect Status - {JsonConvert.SerializeObject(statusResponse)}");
        }, this._defaultExceptionHandler);
    }

    public async void IsBoraConnected()
    {
        Debug.Log("[FaceWalletManager] IsBoraConnected(..)");
        this._validateIsLoggedIn();
        string bappUsn = this._appState.GetLoginData().UserId;
        BoraPortalConnectStatusResponse statusResponse = await this._face.Bora().IsConnected(bappUsn);
        this._resultUpdated.RaiseEvent($"Bora Connect Status - {JsonConvert.SerializeObject(statusResponse)}");
    }
    
    public void SignMessage(string messageToSign)
    {
        this._validateIsLoggedIn();

        Task<FaceRpcResponse> responseTask = this._face.Wallet().SignMessage(messageToSign);

        this._actionQueue.Enqueue(responseTask, response =>
        {
            string result = JsonConvert.SerializeObject(response);
            Debug.Log($"Result: {result}");
            this._resultUpdated.RaiseEvent(string.Format($"Signed Message - {response.CastResult<string>()}"));
        }, this._defaultExceptionHandler);
    }

    private async void WalletConnect()
    {
        this._validateIsLoggedIn();
        await this._face.WalletConnect().ConnectOpenSea(this._appState.GetLoginData().UserAddress);
    }
    
    private async Task<LoginResult> _loginAndGetBalanceAsync([AllowNull] List<LoginProviderType> providers)
    {
        List<LoginProviderType> providerTypes = new List<LoginProviderType>() { LoginProviderType.Google, LoginProviderType.Apple };
        FaceLoginResponse response = await this._face.Auth().Login(providers);
        string address = response.wallet.Address;
        string userVerificationToken = response.userVerificationToken;
        Debug.Log($"User verification token: {userVerificationToken}");

        string balance = await this._getBalance(address);

        return new LoginResult(balance, response);
    } 
    
    private void _directSocialLoginAndGetBalance(string provider)
    {
        Task<LoginResult> responseTask = this._directSocialLoginAndGetBalanceAsync(provider);

        this._actionQueue.Enqueue(responseTask, response =>
        {
            this._logined.RaiseEvent(new LoginData
            {
                UserId = response.userId,
                UserAddress = response.userAddress,
                Balance = response.balance,
                Result = $"UserVerificationToken: {response.userVerificationToken}"
            });
        }, this._defaultExceptionHandler);
    }
    
    private async Task<LoginResult> _directSocialLoginAndGetBalanceAsync(string provider)
    {
        FaceLoginResponse response = await this._face.Auth().DirectSocialLogin(provider);
        string address = response.wallet.Address;
        string balance = await this._getBalance(address);

        return new LoginResult(balance, response);
    }
    
    private async Task<string> _getBalance(string address)
    {
        FaceRpcResponse response = await this._face.Wallet().GetBalance(address);
        return NumberFormatter.DivideHexWithDecimals(
            response.CastResult<string>(), 
            FaceSettings.Instance.Blockchain().GetPlatformCoinDecimals());
    }
    
    private void _validateIsLoggedIn()
    {
        if (!this._appState.LoggedIn())
        {
            throw new UnauthorizedException();
        }
    }
    
    private async Task<TransactionResult> _sendTransactionTask(string to, Func<string> dataCallback, string value = "0")
    {
        string loggedInAddress = this._appState.GetLoginData().UserAddress;
        RawTransaction request = new RawTransaction(loggedInAddress, to, string.Format($"0x{value}"), dataCallback());
        try
        {
            TransactionRequestId transactionRequestId = await this._face.Wallet().SendTransaction(request);
            return new TransactionResult(await this._getBalance(loggedInAddress), string.Format($"TX Hash - {transactionRequestId.transactionId}"));
        }
        catch (FaceException ex)
        {
            return new TransactionResult(await this._getBalance(loggedInAddress), string.Format($"Error - {ex.Message}"));
        }
    }
    
    private async Task<TransactionResult> _sendErc20TransactionTask(FTTransactionData data)
    {
        int decimals = await this._getDecimals(data);
        return await this._sendTransactionTask(data.ContractAddress, () =>
        {
            string amount = data.Amount;
            return this._face.dataFactory.CreateErc20SendData(data.ContractAddress, data.ReceiverAddress, amount, decimals);
        });
    }
    
    private void _sendTransactionQueue(Task<TransactionResult> transactionTask)
    {
        this._validateIsLoggedIn();

        this._actionQueue.Enqueue(transactionTask, response =>
        {
            this._balanceUpdated.RaiseEvent(response.balance);
            this._resultUpdated.RaiseEvent(response.result);
        }, this._defaultExceptionHandler);
    }
    
    private async Task<int> _getDecimals(FTTransactionData data)
    {
        string decimalsData =
            this._face.dataFactory.CreateErc20GetDecimalsData(data.ContractAddress);
        RawTransaction decimalsRequest =
            new RawTransaction(null, data.ContractAddress, "0x0", decimalsData);
        FaceRpcResponse response = await this._face.Wallet().Call(decimalsRequest);
        return int.Parse(NumberFormatter.HexadecimalToDecimal(response.CastResult<string>()).ToStringInvariant());
    }
    
    private void _defaultExceptionHandler(Exception ex)
    {
        this._exceptionOccurred.RaiseEvent(ex.Message);
    }
}