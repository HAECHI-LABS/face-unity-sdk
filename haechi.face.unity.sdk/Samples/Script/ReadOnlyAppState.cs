using System.Collections.Generic;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;

public abstract class ReadOnlyAppState : ScriptableObject
{
    public abstract void Initialize();
    
    /**
     * Connect
     */
    public abstract List<string> GetAllEnvironments();
    public abstract List<Blockchain> GetAllBlockchains();
    
    public abstract string GetApiKey();
    public abstract string GetPrivateKey();
    public abstract string GetMultiStageId();
    public abstract Profile GetEnv();
    
    public abstract Blockchain GetBlockchain();
    public abstract BlockchainNetwork GetBlockchainNetwork();

    public abstract string GetScheme();
    
    /**
     * Login
     */

    public abstract LoginData GetLoginData();

    public abstract bool LoggedIn();
    
    /**
     * FT
     */
    
    public abstract string GetERC20Balance();
}