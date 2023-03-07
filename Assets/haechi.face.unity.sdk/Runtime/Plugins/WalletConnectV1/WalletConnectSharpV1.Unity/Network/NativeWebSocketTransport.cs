using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using WalletConnectSharpV1.Core.Events;
using WalletConnectSharpV1.Core.Events.Request;
using WalletConnectSharpV1.Core.Events.Response;
using WalletConnectSharpV1.Core.Models;
using WalletConnectSharpV1.Core.Network;
using NativeWebSocket;

namespace WalletConnectSharpV1.Unity.Network
{
    public class NativeWebSocketTransport : MonoBehaviour, ITransport
    {
        private bool opened = false;
        private bool closed = false;
        
        private WebSocket nextClient;
        private WebSocket client;
        private EventDelegator _eventDelegator;
        private bool wasPaused;
        private string currentUrl;
        private List<string> subscribedTopics = new List<string>();
        private Queue<NetworkMessage> _queuedMessages = new Queue<NetworkMessage>();

        private static NativeWebSocketTransport _instance;

        public static NativeWebSocketTransport GetInstance()
        {
            return _instance;
        }
        
        private void Awake()
        {
            _instance = this;
        }

        public bool Connected
        {
            get
            {
                return client != null && (client.State == WebSocketState.Open || client.State == WebSocketState.Closing) && opened;
            }
        }
        
        public void AttachEventDelegator(EventDelegator eventDelegator)
        {
            this._eventDelegator = eventDelegator;
        }

        public void Dispose()
        {
            this._eventDelegator.Clear();
            if (client != null)
            {
                client.CancelConnection();
            }
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<MessageReceivedEventArgs> OpenReceived;
        
        public string URL
        {
            get
            {
                return currentUrl;
            }
        }

        public async Task Open(string url, bool clearSubscriptions = true)
        {
            if (currentUrl != url || clearSubscriptions)
            {
                ClearSubscriptions();
            }

            currentUrl = url;
            
            await _socketOpen();
        }

        private async Task _socketOpen()
        {
            if (nextClient != null)
            {
                return;
            }

            string url = currentUrl;
            if (url.StartsWith("https"))
                url = url.Replace("https", "wss");
            else if (url.StartsWith("http"))
                url = url.Replace("http", "ws");

            nextClient = new WebSocket(url);
            
            TaskCompletionSource<bool> eventCompleted = new TaskCompletionSource<bool>(TaskCreationOptions.None);

            nextClient.OnOpen += async () =>
            {
                await CompleteOpen();
                
                // subscribe now
                if (this.OpenReceived != null)
                    OpenReceived(this, null);

                Debug.Log("[WebSocket] Opened " + url);
                this.opened = true;
                this.closed = false;
                eventCompleted.SetResult(true);
            };

            nextClient.OnMessage += OnMessageReceived;
            nextClient.OnClose += ClientTryReconnect;
            nextClient.OnError += (e) => {
                Debug.Log("[WebSocket] OnError " + e);
                HandleError(new Exception(e));
            };
            
            nextClient.Connect().ContinueWith(t => HandleError(t.Exception), TaskContinuationOptions.OnlyOnFaulted);

            Debug.Log("[WebSocket] Waiting for Open " + url);
            await eventCompleted.Task;
            Debug.Log("[WebSocket] Open Completed");
        }

        private void HandleError(Exception e)
        {
            Debug.LogError(e);
        }

        private async Task CompleteOpen()
        {
            await Close();
            this.client = this.nextClient;
            this.nextClient = null;
            await QueueSubscriptions();
            opened = true;
            await FlushQueue();
        }

        private async Task FlushQueue()
        {
            Debug.Log("[WebSocket] Flushing Queue");
            Debug.Log("[WebSocket] Queue Count: " + _queuedMessages.Count);
            while (_queuedMessages.Count > 0)
            {
                var msg = _queuedMessages.Dequeue();
                await SendMessage(msg);
            }

            Debug.Log("[WebSocket] Queue Flushed");
        }

        private Task QueueSubscriptions()
        {
            foreach (var topic in subscribedTopics)
            {
                this._queuedMessages.Enqueue(GenerateSubscribeMessage(topic));
            }

            Debug.Log("[WebSocket] Queued " + subscribedTopics.Count + " subscriptions");
            return Task.CompletedTask;
        }
        
        private async void ClientTryReconnect(WebSocketCloseCode closeCode)
        {
            Debug.Log("ClientTryReconnect");
            if (wasPaused)
            {
                Debug.Log("[WebSocket] Application paused, retry attempt aborted");
                return;
            }
            
            nextClient = null;
            await _socketOpen();
        }

        private async void OnMessageReceived(byte[] bytes)
        {
            string json = System.Text.Encoding.UTF8.GetString(bytes);
            
            Debug.Log($"OnMessageReceived: {json}");
            try
            {
                var msg = JsonConvert.DeserializeObject<NetworkMessage>(json);
                
                await SendMessage(new NetworkMessage()
                {
                    Payload = "",
                    Type = "ack",
                    Silent = true,
                    Topic = msg.Topic
                });

                if (this.MessageReceived != null)
                    MessageReceived(this, new MessageReceivedEventArgs(msg, this));
            }
            catch(Exception e)
            {
                Debug.Log("[WebSocket] Exception " + e.Message);
            }   
        }
        
        private void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (client != null && client.State == WebSocketState.Open)
            {
                client.DispatchMessageQueue();
            }
#endif
        }

        public async Task Close()
        {
            Debug.Log("Closing Websocket");
            try
            {
                if (client != null)
                {
                    client.OnClose -= ClientTryReconnect;
                    await client.Close();
                    this.opened = false;
                    this.closed = true;
                }
            }
            catch (WebSocketInvalidStateException e)
            {
                if (e.Message.Contains("WebSocket is not connected"))
                    Debug.LogWarning("Tried to close a websocket when it's already closed");
                else
                    throw;
            }
        }

        public async Task QueueAndFlush()
        {
            await this.QueueSubscriptions();
            await this.FlushQueue();
        }
        
        public async Task SendMessage(NetworkMessage message)
        {
            if (!Connected)
            {
                _queuedMessages.Enqueue(message);
                await _socketOpen();
            }
            else
            {
                string finalJson = JsonConvert.SerializeObject(message);
                
                await this.client.SendText(finalJson);   
            }
        }

        public async Task Subscribe(string topic)
        {
            Debug.Log("[WebSocket] Subscribe to " + topic);

            var msg = GenerateSubscribeMessage(topic);
            
            await SendMessage(msg);

            if (!subscribedTopics.Contains(topic))
            {
                subscribedTopics.Add(topic);
            }

            opened = true;
        }

        private NetworkMessage GenerateSubscribeMessage(string topic)
        {
            return new NetworkMessage()
            {
                Payload = "",
                Type = "sub",
                Silent = true,
                Topic = topic
            };
        }

        public async Task Subscribe<T>(string topic, EventHandler<JsonRpcResponseEvent<T>> callback) where T : JsonRpcResponse
        {
            await Subscribe(topic);

            _eventDelegator.ListenFor(topic, callback);
        }
        
        public async Task Subscribe<T>(string topic, EventHandler<JsonRpcRequestEvent<T>> callback) where T : JsonRpcRequest
        {
            await Subscribe(topic);

            _eventDelegator.ListenFor(topic, callback);
        }

        public void ClearSubscriptions()
        {
            Debug.Log("[WebSocket] Subs Cleared");
            subscribedTopics.Clear();
            _queuedMessages.Clear();
        }
    }
}