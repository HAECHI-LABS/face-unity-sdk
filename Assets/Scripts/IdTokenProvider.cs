using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using haechi.face.unity.sdk.Runtime.Utils;
using haechi.face.unity.sdk.Samples.Script;
using JetBrains.Annotations;
using UnityEngine;

public class IdTokenProvider : MonoBehaviour
{
    [SerializeField] private FaceWalletManager faceWalletManager;
    [SerializeField] private StringEventChannelSO _onBoraLoginWithIdtoken;
    [SerializeField] private VoidEventChannelSO _onLoginWithIdtoken;

    private GoogleSignInConfiguration _configuration;
#if UNITY_IOS
    private const string WebClientId = "478075746592-mkvlpvmo7hgo5bj9qjpbsgetcc6ae377.apps.googleusercontent.com";
#else
    private const string WebClientId =
        "478075746592-2eph96cegqojcd29r1bg62ur64d9bbql.apps.googleusercontent.com";
#endif

    private void OnEnable()
    {
        this._onBoraLoginWithIdtoken.OnEventRaised += this.LoginGoogleWithBoraConnect;
        this._onLoginWithIdtoken.OnEventRaised += this.LoginGoogle;
    }

    private void OnDisable()
    {
        this._onBoraLoginWithIdtoken.OnEventRaised -= this.LoginGoogleWithBoraConnect;
        this._onLoginWithIdtoken.OnEventRaised -= this.LoginGoogle;
    }


    private void Awake()
    {
        _configuration = new GoogleSignInConfiguration
        {
            WebClientId = WebClientId, RequestEmail = true, RequestIdToken = true,
            UseGameSignIn = false
        };
    }

    private void LoginGoogleWithBoraConnect(string bappUsn)
    {
        if (bappUsn == null)
        {
            throw new Exception("bappUsn is null even thouh you tried to login with bora connect.");
        }
        try
        {
#if UNITY_IOS
            LoginGoogleIphone(bappUsn);
#elif UNITY_ANDROID
            LoginGoogleAndroid(bappUsn);
#elif UNITY_WEBGL
            LoginGoogleWebGL(bappUsn);
#endif
        }
        catch (GoogleSignIn.SignInException e)
        {
            Debug.Log("GoogleSigninException: " + e.Status + " " + e.Message + " " +
                      e.InnerException);
            throw e;
        }
    }

    private void LoginGoogle()
    {
        try
        {
#if UNITY_IOS
            LoginGoogleIphone(null);
#elif UNITY_ANDROID
            LoginGoogleAndroid(null);
#elif UNITY_WEBGL
            LoginGoogleWebGL(null);
#endif
        }
        catch (GoogleSignIn.SignInException e)
        {
            Debug.Log("GoogleSigninException: " + e.Status + " " + e.Message + " " +
                      e.InnerException);
            throw e;
        }
    }

    /// <param name="bappUsn"></param> if bappUsn is null, it means that you are not using bora connect.
    private async void LoginGoogleIphone([CanBeNull] string bappUsn)
    {
        GoogleSignIn.Configuration = _configuration;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.AdditionalScopes = new[]
        {
            "email", "profile", "https://www.googleapis.com/auth/userinfo.email",
            "https://www.googleapis.com/auth/userinfo.profile",
            "openid"
        };
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
            (Task<GoogleSignInUser> task) => { _loginProcessIphone(task, bappUsn); },
            TaskScheduler.FromCurrentSynchronizationContext());
    }

    /// <param name="bappUsn"></param> if bappUsn is null, it means that you are not using bora connect.
    public async void LoginGoogleAndroid(string bappUsn)
    {
        GoogleSignIn.Configuration = _configuration;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.AdditionalScopes = new[]
        {
            "email", "profile", "https://www.googleapis.com/auth/userinfo.email",
            "https://www.googleapis.com/auth/userinfo.profile",
            "openid"
        };
        var user = await GoogleSignIn.DefaultInstance.SignIn();
        if (bappUsn != null)
        {
            faceWalletManager.BoraLoginWithIdtoken(user.IdToken, bappUsn);
        }
        else
        {
            faceWalletManager.LoginWithIdtoken(user.IdToken);
        }
    }

    /// <param name="bappUsn"></param> if bappUsn is null, it means that you are not using bora connect.
    public async void LoginGoogleWebGL(string bappUsn)
    {
        GoogleSignInForWebGL.GoogleSignIn(gameObject, "LoginProcessForWebGL");
    }

    private void _loginProcessIphone(Task<GoogleSignInUser> task, String bappUsn)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator =
                   task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                        (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.LogWarning("Google Calling Got Error: " + error.Status + " " +
                                     error.Message);
                }
                else
                {
                    Debug.LogWarning("Google Calling Got Unexpected Exception: " + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            if (Debug.isDebugBuild) Debug.Log("Google Calling Canceled");
        }
        else
        {
            if (Debug.isDebugBuild) Debug.Log("Google Calling Completed");
            if (Debug.isDebugBuild)
                Debug.Log("Google Calling Signed in DisplayName : " + task.Result.DisplayName);
            if (Debug.isDebugBuild)
                Debug.Log("Google Calling Signed in UserId      : " + task.Result.UserId);
            if (Debug.isDebugBuild)
                Debug.Log("Google Calling Signed in IdToken     : " + task.Result.IdToken);
            if (bappUsn != null)
            {
                this.faceWalletManager.BoraLoginWithIdtoken(task.Result.IdToken, bappUsn);
            }
            else
            {
                this.faceWalletManager.LoginWithIdtoken(task.Result.IdToken);
            }
        }
    }

    public void LoginProcessForWebGL(string idToken)
    {
        Debug.LogError("WEBGL login with idtoken in sample dapp is not supported yet.");
        this.faceWalletManager.LoginWithIdtoken(idToken);
    }
}