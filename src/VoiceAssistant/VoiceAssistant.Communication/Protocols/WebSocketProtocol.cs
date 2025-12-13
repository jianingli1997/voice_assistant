/*
 * Class: WebSocketProtocol.cs
 * Author: LiJiaNing
 * Date: 2025-12-12
 * Description: WebSocket 客户端类，支持自动重连、消息接收与发送
 * Version: 1.0
 */

#nullable disable
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using VoiceAssistant.Log;

namespace VoiceAssistant.Communication.Protocols
{
    public class WebSocketProtocol(string uri) : IDisposable
    {
        private ClientWebSocket _socket;
        private readonly CancellationTokenSource _cts = new();
        private readonly Uri _uri = new(uri);
        private bool _manuallyClosed;
        private bool _isReconnecting;
        private const int RetryDelay = 5000;
        private readonly object _lock = new();

        public event Action Connected;
        public event Action Disconnected;
        public event Action<string> StringReceived;
        public event Action<string> JsonReceived;

        public async Task ConnectAsync()
        {
            lock (_lock)
            {
                if (_socket?.State == WebSocketState.Open)
                    return;

                _manuallyClosed = false;
            }

            await TryConnectAsync();
        }

        private async Task TryConnectAsync()
        {
            while (!_cts.IsCancellationRequested && !_manuallyClosed)
            {
                try
                {
                    _socket = new ClientWebSocket();
                    await _socket.ConnectAsync(_uri, _cts.Token);

                    // 连接成功回调
                    Connected?.Invoke();

                    // 启动接收循环
                    _ = Task.Run(ReceiveLoopAsync);

                    // 连接成功后退出循环
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WebSocket] 连接失败: {ex.Message}, {RetryDelay / 1000}s 后重试...");
                    Disconnected?.Invoke();
                }

                await Task.Delay(RetryDelay, _cts.Token);
            }
        }

        private async Task ReceiveLoopAsync()
        {
            var buffer = new byte[8192];

            try
            {
                while (_socket.State == WebSocketState.Open && !_cts.Token.IsCancellationRequested)
                {
                    int count = 0;
                    WebSocketReceiveResult result;

                    do
                    {
                        result = await _socket.ReceiveAsync(
                            new ArraySegment<byte>(buffer, count, buffer.Length - count),
                            _cts.Token);

                        if (result.MessageType != WebSocketMessageType.Close)
                            count += result.Count;
                        else
                            break;
                    } while (!result.EndOfMessage);

                    if (count <= 0) continue;
                    string message = Encoding.UTF8.GetString(buffer, 0, count).Trim();
                    LogHelper.WriteDebugLog("Receive:" + message, "CC");

                    if (message.StartsWith("{") && message.EndsWith("}"))
                        JsonReceived?.Invoke(message);
                    else
                        StringReceived?.Invoke(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[WebSocket] 接收异常: " + ex.Message);
            }
            finally
            {
                Disconnected?.Invoke();

                if (!_manuallyClosed)
                    await StartReconnectAsync();
            }
        }

        private async Task StartReconnectAsync()
        {
            lock (_lock)
            {
                if (_isReconnecting) return;
                _isReconnecting = true;
            }

            try
            {
                await TryConnectAsync();
            }
            finally
            {
                _isReconnecting = false;
            }
        }

        public async Task SendStringAsync(string message)
        {
            if (_socket?.State != WebSocketState.Open)
            {
                LogHelper.WriteInfoLog("WebsocketState", "CC");
                return;
            }

            var data = Encoding.UTF8.GetBytes(message);
            await _socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, _cts.Token);
            LogHelper.WriteDebugLog("Send:" + message, "CC");
        }

        public async Task SendJsonAsync(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            await SendStringAsync(json);
        }

        public void Disconnect()
        {
            try
            {
                lock (_lock)
                    _manuallyClosed = true;

                if (_socket?.State == WebSocketState.Open)
                    _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None).Wait();

                _cts.Cancel();
                Disconnected?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[WebSocket] 断开异常: " + ex.Message);
            }
        }

        public async Task ForceReconnectAsync()
        {
            try
            {
                lock (_lock)
                    _manuallyClosed = false;

                if (_socket?.State == WebSocketState.Open)
                    await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "ForceReconnect",
                        CancellationToken.None);

                await StartReconnectAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[WebSocket] ForceReconnect 异常: " + ex.Message);
            }
        }

        public void Dispose()
        {
            _socket?.Dispose();
            _cts?.Dispose();
        }
    }
}