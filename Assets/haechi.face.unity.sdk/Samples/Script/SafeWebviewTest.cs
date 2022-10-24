using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
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
        Task<RpcResponseMessage> responseTask = this._face.wallet.GetBalance();
        this._actionQueue.Enqueue(response =>
        {
            string result = JsonConvert.SerializeObject(response);
            Debug.Log($"GetBalance result: {result}");
            this.responseText.text = result;
        }, responseTask);
    }
}