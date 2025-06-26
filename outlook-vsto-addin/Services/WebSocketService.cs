using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EffyDocOutlookAddin.Services
{
    public class WebSocketService : IDisposable
    {
        private ClientWebSocket webSocket;
        private readonly string websocketUrl;
        private CancellationTokenSource cancellationTokenSource;
        private bool isConnected = false;

        public event EventHandler<string> MessageReceived;
        public event EventHandler Connected;
        public event EventHandler Disconnected;

        public WebSocketService(string websocketUrl)
        {
            this.websocketUrl = websocketUrl;
            this.cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task ConnectAsync(string userEmail, string token)
        {
            try
            {
                webSocket = new ClientWebSocket();
                
                var uri = new Uri($"{websocketUrl}?user_email={Uri.EscapeDataString(userEmail)}&token={Uri.EscapeDataString(token)}");
                
                await webSocket.ConnectAsync(uri, cancellationTokenSource.Token);
                isConnected = true;
                
                Connected?.Invoke(this, EventArgs.Empty);
                
                // Start listening for messages
                _ = Task.Run(ListenForMessages);
            }
            catch (Exception ex)
            {
                throw new Exception($"WebSocket connection failed: {ex.Message}");
            }
        }

        private async Task ListenForMessages()
        {
            var buffer = new byte[4096];
            
            while (webSocket.State == WebSocketState.Open && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationTokenSource.Token);
                    
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        MessageReceived?.Invoke(this, message);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await DisconnectAsync();
                        break;
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"WebSocket error: {ex.Message}");
                    await DisconnectAsync();
                    break;
                }
            }
        }

        public async Task SendMessageAsync(object message)
        {
            if (!isConnected || webSocket.State != WebSocketState.Open)
            {
                throw new InvalidOperationException("WebSocket is not connected");
            }

            try
            {
                var json = JsonConvert.SerializeObject(message);
                var buffer = Encoding.UTF8.GetBytes(json);
                
                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send WebSocket message: {ex.Message}");
            }
        }

        public async Task SubscribeToDocumentAsync(string documentId)
        {
            var message = new
            {
                type = "subscribe_document",
                document_id = documentId
            };
            
            await SendMessageAsync(message);
        }

        public async Task UnsubscribeFromDocumentAsync(string documentId)
        {
            var message = new
            {
                type = "unsubscribe_document",
                document_id = documentId
            };
            
            await SendMessageAsync(message);
        }

        public async Task SendHeartbeatAsync()
        {
            var message = new
            {
                type = "heartbeat"
            };
            
            await SendMessageAsync(message);
        }

        public async Task DisconnectAsync()
        {
            if (isConnected && webSocket != null && webSocket.State == WebSocketState.Open)
            {
                try
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error closing WebSocket: {ex.Message}");
                }
            }
            
            isConnected = false;
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        public bool IsConnected => isConnected && webSocket?.State == WebSocketState.Open;

        public void Dispose()
        {
            cancellationTokenSource?.Cancel();
            
            if (isConnected)
            {
                _ = Task.Run(async () => await DisconnectAsync());
            }
            
            webSocket?.Dispose();
            cancellationTokenSource?.Dispose();
        }
    }
}