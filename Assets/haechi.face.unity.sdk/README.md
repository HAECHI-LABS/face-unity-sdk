## Feature

- This project is to guide how to embed Face SDK in Unity project. Implemented below very simple functions to help developers easily use FaceSDK.
  - Login, Signup
  - GetMyBalance
  - SendRawTransaction
- Unfortunately, this sample only runs on `Android`, `iOS` due to external Unity Assets.

## Requirements

- Unity Editor version over `2021.3.10f1`
- [UniWebView](https://docs.uniwebview.com/) License
  - We used external paid Unity Assets to easily implement multiple webview, because FaceWallet uses IFrame to secure users' crypto assets. On this repository, we do not provide UniWebView Unity Asset.
  - However, as [UniWebView terms](https://docs.uniwebview.com/guide/faq.html#can-i-embed-uniwebview-in-my-own-assets) says that we have limited license about this asset, we provide sample code that includes UniWebView through binary package in `.dll` assembly.
- Webview page to communicate between Unity and FaceSDK
  - We also provide sample webview page to guide how to communicate between Unity components and external dapp. See this [face-webview-sample](https://github.com/HAECHI-LABS/face-webview-sample) repository.

## Installation

1. As mentioned above, should download `FaceUnitySample.unitypackage` first.
2. Let's create Unity project through UnityHub. Create 2D project with project name you want. \
   ![스크린샷 2022-09-30 03 41 01](https://user-images.githubusercontent.com/32338616/193116376-3807c7ac-8cb8-4fa1-961b-4e4ae83b6048.png) 
3. On project tab, as you can see, there are two directories exist. All you need is `Assets` directory. Right click on mouse over `Assets` directory -> select `Import Package` -> `Custom Package`. Then, import `FaceUnitySample.unitypackage` that you downloaded on the first step. \
   ![스크린샷 2022-09-30 03 49 33](https://user-images.githubusercontent.com/32338616/193117922-be868e44-69d8-4568-ae90-b313b89a82cb.png) 
4. If `Import Unit Package` appears, select all and finish imports. Default value is all selected. It may take some time depends on running machine. \
   ![스크린샷 2022-09-30 03 51 42](https://user-images.githubusercontent.com/32338616/193117956-78cb7718-7482-46af-be42-8ab9080ef169.png) 
5. Go to `haechi.face.unity.sdk`/`Samples` directory, drag `SampleWebview.prefab` and drop on `hierarchy` space. If TMP Importer popup appears, import it. \
   ![스크린샷 2022-09-30 03 58 17](https://user-images.githubusercontent.com/32338616/193120361-7a9c2202-b411-4159-9c3e-81f9f429b144.png) \
   ![스크린샷 2022-09-30 03 58 56](https://user-images.githubusercontent.com/32338616/193120370-2d3ae0e2-c0b6-4440-b021-63370e9426ce.png) \
   ![스크린샷 2022-09-30 04 01 21](https://user-images.githubusercontent.com/32338616/193120382-979625f7-0817-406c-874a-748cd4c18e2e.png)
6. (Optional) For better ui change `Main Camera` background color to white(`FFFFFF`).\
   ![스크린샷 2022-09-30 04 07 30](https://user-images.githubusercontent.com/32338616/193120863-8899509f-fb67-4fea-923b-72c4e7c21539.png)
7. Add desired inputs on `SampleWebview.prefab`'s scripts. This should be done before building and running. \
   ![스크린샷 2022-09-30 04 15 08](https://user-images.githubusercontent.com/32338616/193122233-623c7185-476b-41fd-af20-59e4f8cd0273.png)
8. Now we are good to go! There are three basic ways to run our app. See below for more.
   ![스크린샷 2022-09-30 04 07 41](https://user-images.githubusercontent.com/32338616/193120873-6f7da2fe-fd89-4533-b6b6-09267f84c83c.png)

## Run Sample Code

### UnityEditor

1. It's very simple. Just click play shaped button. However, ui/ux would be better if you run on Android, iOS.
   ![스크린샷 2022-09-30 04 19 19](https://user-images.githubusercontent.com/32338616/193123302-b3cf0f51-1f4d-4a3b-995f-4067c567a20b.png)


### Android

1. We need Android simulator from `AndroidStudio`, or hardware device. Refer [here](https://developer.android.com/studio/run/managing-avds). \
2. Once you prepared AVD or hardware device, you need to configure build settings from Unity editor. \
   - Go to `File` > `Build Settings..`. And then switch platform to `Android`. \
     ![스크린샷 2022-09-30 04 41 00](https://user-images.githubusercontent.com/32338616/193127525-7e58b95c-3bbe-404a-b4e9-5ac2b88d9b51.png)
   - Select device to run from `Run Device` menu.
   - On `Player Settings..` you can configure app icon and identities such as package name, version etc. Refer [this page](https://docs.unity3d.com/2021.3/Documentation/Manual/class-PlayerSettingsAndroid.html) for more info.
3. Execute `Build And Run`. It might take a lot of times depending on your machine. Now you can view same scene as you saw on UnityEditor player. \
   ![스크린샷 2022-09-30 04 49 14](https://user-images.githubusercontent.com/32338616/193128137-55f5e768-052b-43ae-8e80-ae66e54d2b0c.png)

### iOS

1. First, to build on iOS we need `MacOS` machine. Install `XCode` first to build iOS app.
2. On iOS also need virtual device or iPhone. See [here](https://developer.apple.com/documentation/xcode/running-your-app-in-simulator-or-on-a-device)
3. After switch platform to iOS on build setting, you need to choose whether you use virtual device or iPhone. Go to `Player Settings` > `Other Settings` > `Target SDK` and set target device type. \
   ![스크린샷 2022-09-30 04 58 30](https://user-images.githubusercontent.com/32338616/193130104-5a56775a-0911-4ade-8821-1222fd0b6b35.png)
4. Also you can choose icon etc on `Player Setting`. Refer [this page](https://docs.unity3d.com/2021.3/Documentation/Manual/class-PlayerSettingsiOS.html).
5. Execute `Build And Run`. It might take a lot of times depending on your machine. Now you can view same scene as you saw on UnityEditor player. \
   ![IMG_4971](https://user-images.githubusercontent.com/32338616/193131062-a6a6c1a5-edf8-4f7e-930e-0038415be9e6.png)

## Sample Codes

### How To Communicate with Webview Client

- In this sample repository, we are sending messages to client via native `JavaScript`, using `UniWebView`'s `EvaluateJavaScript` function.
    ```csharp
    public void DispatchEvent(string message, Func<string, bool> handler)
    {
        ...
        _webView.EvaluateJavaScript(
            $"window.dispatchEvent(new MessageEvent('message', {{ 'data': '{message}' }}));");
        ...
    }
    ```

### Request to Client
- Request message need to include two important parameters.
  - `id`: id will be used to check if request and response corresponding.
  - `method`: method determines which methods on the client will be executed.
- Request `params` parameter can be null.
    ```csharp
    public class FaceRpcRequest<T>
    {
        [SerializeField] internal int id;
        [SerializeField] internal string method;
        [SerializeField] [CanBeNull] internal T[] @params;
        ... 
    
        public FaceRpcRequest(FaceRpcMethod method, T[] parameters)
        {
            id = Random.Range(1, 100000);
            this.method = Enum.GetName(typeof(FaceRpcMethod), method);
            @params = parameters;
        }
    }
    ```

### Response from Client
- Client response scheme is uri type. For example, `uniwebview://faceSdk?data=data`.
    ```csharp
    void WrappedHandler(UniWebView webview, UniWebViewMessage message)
    {
        Debug.Log($"Received Message: {message.RawMessage}");
        JsonConvert.SerializeObject(message.Args);
        var jsonResponse = JsonConvert.SerializeObject(message.Args);
    
        Debug.Log($"Message Handled: {jsonResponse}");
        handler(jsonResponse);
    
        _webView.OnMessageReceived -= WrappedHandler;
    }
    ```
- Note that response should start with `uniwebview` to properly receive messages as long as you chose UniWebView for webview library. [More info](https://docs.uniwebview.com/api/uniwebviewmessage.html)

### Supported Method in Sample
- We support the below methods from client. Of course, you can change naming if not using sample webview client.
    ```csharp
    public enum FaceRpcMethod
    {
        face_logInSignUp,
        face_logOut,
        face_getMyBalance,
        face_getBalance,
        face_sendTransaction,
    }
    ```
