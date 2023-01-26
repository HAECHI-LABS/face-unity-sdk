using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using haechi.face.unity.sdk.Runtime.Utils;
using haechi.face.unity.sdk.Samples.Script;
using UnityEngine;

public class IdTokenProvider : MonoBehaviour
{
    [SerializeField] internal FaceUnity _faceUnity;

    private GoogleSignInConfiguration _configuration;
#if UNITY_IPHONE
    private const string WebClientId = "965931844205-tt978gju3l3fa97amqp431falebfrqu7.apps.googleusercontent.com";
#else
    private const string WebClientId = "965931844205-q64ebkmn6atutksvi6b0rv2hkdm9gor5.apps.googleusercontent.com";
#endif
    
    
    private void Awake()
    {
        _configuration = new GoogleSignInConfiguration { WebClientId = WebClientId, RequestEmail = true, RequestIdToken = true, UseGameSignIn = false};
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public async void LoginGoogle()
    {
#if UNITY_IPHONE
        GoogleSignIn.Configuration = _configuration;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.AdditionalScopes = new[]
        {
            "email", "profile", "https://www.googleapis.com/auth/userinfo.email",
            "https://www.googleapis.com/auth/userinfo.profile",
            "openid"
        };
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(_loginProcess, TaskScheduler.FromCurrentSynchronizationContext());
#elif UNITY_ANDROID
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
        await _faceUnity.LoginWithIdTokenAndGetBalanceAsync(user.IdToken);
#elif UNITY_WEBGL
        GoogleSignInForWebGL.GoogleSignIn(gameObject, "LoginProcessForWebGL");
#endif
    }

#if UNITY_IPHONE
    private void _loginProcess(Task<GoogleSignInUser> task){
        if (task.IsFaulted) {
            using (IEnumerator<System.Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator ()) {
                if (enumerator.MoveNext ()) {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.LogWarning ("Google Calling Got Error: " + error.Status + " " + error.Message);
                } else {
                    Debug.LogWarning ("Google Calling Got Unexpected Exception: " + task.Exception);
                }
            }
        } else if (task.IsCanceled) {
            if (Debug.isDebugBuild) Debug.Log ("Google Calling Canceled");
        } else {
            if (Debug.isDebugBuild) Debug.Log ("Google Calling Completed");
            if (Debug.isDebugBuild) Debug.Log ("Google Calling Signed in DisplayName : " + task.Result.DisplayName);
            if (Debug.isDebugBuild) Debug.Log ("Google Calling Signed in UserId      : " + task.Result.UserId);
            if (Debug.isDebugBuild) Debug.Log ("Google Calling Signed in IdToken     : " + task.Result.IdToken);
            _faceUnity.LoginWithIdTokenAndGetBalanceAsync(task.Result.IdToken);
        }
    }
#elif UNITY_ANDROID
#elif UNITY_WEBGL
    public void LoginProcessForWebGL(string idToken)
    {
        _faceUnity.LoginWithIdTokenAndGetBalanceAsync(idToken);
    }
#endif
}
