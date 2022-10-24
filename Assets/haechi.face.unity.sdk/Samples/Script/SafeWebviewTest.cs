using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Face))]
[RequireComponent(typeof(ActionQueue))]
public class SafeWebviewTest : MonoBehaviour
{
    [SerializeField] private TMP_Text responseText;
    private ActionQueue _actionQueue;

    private Face _face;

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
}