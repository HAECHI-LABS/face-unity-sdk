using System.Collections;
using System.Text;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Module;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace haechi.face.unity.sdk.Runtime.Client
{
    public class UnityWebRequestService
    {
        private TaskCompletionSource<string> _rpcResponsePromise = new TaskCompletionSource<string>();

        private MonoBehaviour _face;

        public UnityWebRequestService(MonoBehaviour face)
        {
            this._face = face;
        }

        public async Task<TR> GetResult<TR>()
        {
            return JsonConvert.DeserializeObject<TR>(await _rpcResponsePromise.Task);
        }

        public void Get(string uri)
        {
            this._face.StartCoroutine(this._get(uri));
        }

        public void Post(string uri, string requestBody)
        {
            this._face.StartCoroutine(this._post(uri, requestBody));
        }

        private IEnumerator _get(string uri)
        {
            UnityWebRequest www = UnityWebRequest.Get(uri);
            www.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            www.SetRequestHeader("X-Face-Dapp-Api-Key", FaceSettings.Instance.ApiKey());
            www.SetRequestHeader("X-Face-Dapp-Api-Hostname", Application.absoluteURL);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Iframe.ConsoleLog(www.error);
            }
            else
            {
                this._rpcResponsePromise.TrySetResult(www.downloadHandler.text);
            }
        }

        private IEnumerator _post(string uri, string requestBody)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(uri, requestBody))
            {
                byte[] jsonBody = new UTF8Encoding().GetBytes(requestBody);
                www.uploadHandler = new UploadHandlerRaw(jsonBody);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
                www.SetRequestHeader("X-Face-Dapp-Api-Key", FaceSettings.Instance.ApiKey());
                www.SetRequestHeader("X-Face-Dapp-Api-Hostname", Application.absoluteURL);
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError ||
                    www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Iframe.ConsoleLog(www.error);
                }
                else
                {
                    this._rpcResponsePromise.TrySetResult(www.downloadHandler.text);
                }
            }
        }
    }
}