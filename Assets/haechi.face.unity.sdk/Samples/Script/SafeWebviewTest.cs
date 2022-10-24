using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Settings;
using Nethereum.JsonRpc.Client.RpcMessages;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using JsonConvert = Unity.Plastic.Newtonsoft.Json.JsonConvert;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Face))]
[RequireComponent(typeof(ActionQueue))]
public class SafeWebviewTest : MonoBehaviour
{
    [SerializeField] private TMP_Text responseText;

    private Face _face;
    private ActionQueue _actionQueue;

    private void Awake()
    {
        this._face = this.GetComponent<Face>();
        this._face.Initialize(new FaceSettings.Parameters
        {
            ApiKey = "bx89CFIGB12EYcSjcAmgeBRViLr4QSwfce/kCfLj7FLa9w83fh5sd7qGTjv5w8ib9Iq9jXERZD8oxAkknroVQCjlulivVgeLn7wI6Pg0hQiAKWG9GSpvcXpqUpkL1bzNZKNfZNulMlxws6OkVFqbmUHoX4VF1TXrDSZeQetPjK4u4pJH/NosXFn1CaVFCHneM7wc/9ry9p0MmNhXe5t9Nai6UD4JlLyheW8MIuxqTXU=",
            Environment = "Dev",
            Blockchain = "ETHEREUM"
        });
        this._actionQueue = this.GetComponent<ActionQueue>();
    }

    public void OnClickLogin()
    {
        Task<FaceRpcResponse> responseTask = this._face.wallet.LoginWithCredential();
        this._actionQueue.Enqueue(response =>
        {
            FaceLoginResponse faceLoginResponse = response.Result.ToObject<FaceLoginResponse>();
            this.responseText.text = faceLoginResponse.faceUserId;
        }, responseTask);
    }

    public void OnClickBalance()
    {
        Task<FaceRpcResponse> responseTask = this._face.wallet.GetBalance();
        this._actionQueue.Enqueue(response =>
        {
            string result = JsonConvert.SerializeObject(response);
            Debug.Log($"Result: {result}");
            this.responseText.text = response.CastResult<string>();
        }, responseTask);
    }
}