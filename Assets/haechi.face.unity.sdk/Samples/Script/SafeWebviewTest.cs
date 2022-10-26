using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Utils;
using Nethereum.Web3;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

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
            Blockchain = "POLYGON"
        });
        this._actionQueue = this.GetComponent<ActionQueue>();
    }

    public void OnClickLogin()
    {
        Task<FaceRpcResponse> responseTask = this._face.wallet.LoginWithCredential();
        this._actionQueue.Enqueue(response =>
        {
            FaceLoginResponse faceLoginResponse = response.CastResult<FaceLoginResponse>();
            this.responseText.text = JsonConvert.SerializeObject(faceLoginResponse);
        }, responseTask);
    }

    public async void OnClickSendNativeToken()
    {
        string amount =
            NumberFormatter.DecimalStringToHexadecimal(
                NumberFormatter.DecimalStringToIntegerString("0.0001", 18));
        RawTransaction request = new RawTransaction("0x27f3bfc6f7f886b5cb64f79b4031a4ab56fcb814",
            "0xb64DEf0FC5B70E256130Eb91f36B628d38b223C7",
            string.Format($"0x{amount}"), null);
        // Sample App에서는 async/await 쓰지 말자고 했지만 closeIframe 되었을 때 다음으로 잘 넘어가는지
        // 확인해보기 위해서 추가
        // FaceRpcResponse response = await this._face.wallet.SendTransaction(request);
        // Debug.Log($"CALLED: {response.Request}");
        // this._actionQueue.Enqueue(response =>
        // {
        //     Debug.Log($"Result: {response}");
        //     // this.responseText.text = response.CastResult<string>();
        // }, responseTask);
    }
    
    public void OnClickBalance()
    {
        Task<FaceRpcResponse> responseTask = this._face.wallet.GetBalance("0x8cF9491DAF6CB1bf81ee86e2e58525BEDbcf516b");
        this._actionQueue.Enqueue(response =>
        {
            string result = JsonConvert.SerializeObject(response);
            Debug.Log($"Result: {result}");
            this.responseText.text = response.CastResult<string>();
        }, responseTask);
    }
    
    public async void OnClickEstimateGas()
    {
        string data = this._face.dataFactory.CreateErc20SendData("0xfCe04dd232006d0da001F6D54Bb5a7fC969dBc08", "0xDD9724Ecd92487633EC0191Ba7737009127D260e", "0.0001", 18);
        RawTransaction rawTransaction = new RawTransaction("0xDD9724Ecd92487633EC0191Ba7737009127D260e",
            "0xfCe04dd232006d0da001F6D54Bb5a7fC969dBc08", "0x1000000000900000000",
            data);
        FaceRpcResponse response = await this._face.wallet.EstimateGas(rawTransaction);
        Debug.Log(response);
    }
}