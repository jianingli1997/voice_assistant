using Newtonsoft.Json;
using System.Timers;
using VoiceAssistant.Communication.Protocols;
using VoiceAssistant.Log;
using VoiceAssistant.Service.Entity;
using VoiceAssistant.Service.Entity.Commands;
using VoiceAssistant.Service.Entity.Payloads;
using VoiceAssistant.Service.Utils;
using Timer = System.Timers.Timer;

namespace VoiceAssistant.Service
{
    public class CentralizedControlService : IDisposable
    {
        private readonly CentralCommunicationService _centralCommunicationService;
        private readonly string _deviceType;
        private readonly MessageManager _messageManager = MessageManager.Instance;

        private readonly Timer _heartbeatTimer = new Timer();
        private readonly Timer _timeoutTimer = new Timer();
        private DateTime _lastHeartbeatTime;
        private int _timeout;

        private readonly JsonSerializerSettings _jsonSettings;

        private WebSocketProtocol WebSocketProtocol => _centralCommunicationService.WebSocketProtocol;

        public event Action DisConnected;
        public event Action CheckInFinished;
        public event Action<Message<ResponseReceivedPayload>> MessageReceived;


        public CentralizedControlService(CentralCommunicationService centralCommunicationService, string deviceType)
        {
            _jsonSettings = new JsonSerializerSettings();
            _jsonSettings.Converters.Add(new PayloadJsonConverter());
            _centralCommunicationService = centralCommunicationService;
            _deviceType = deviceType;

            WebSocketProtocol.Connected += WebSocketProtocol_Connected;
            WebSocketProtocol.JsonReceived += OnMessageReceived;
            WebSocketProtocol.StringReceived += OnStringReceived;
            WebSocketProtocol.Disconnected += WebSocketProtocol_Disconnected;

            _heartbeatTimer.Elapsed += _heartbeatTimer_Elapsed;
            _timeoutTimer.Elapsed += _timeoutTimer_Elapsed;
        }

        private void WebSocketProtocol_Disconnected()
        {
            StopHeartbeatTimer();
            DisConnected?.Invoke();
        }

        private void WebSocketProtocol_Connected()
        {
            // 可放连接成功逻辑
        }

        private async void _heartbeatTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                await WebSocketProtocol.SendStringAsync("ping");
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message, nameof(CentralizedControlService));
                StopHeartbeatTimer();
            }
        }

        private async void _timeoutTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                double elapsed = (DateTime.Now - _lastHeartbeatTime).TotalMilliseconds;
                if (!(elapsed >= _timeout)) return;
                StopHeartbeatTimer();
                await WebSocketProtocol.ForceReconnectAsync();
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message, nameof(CentralizedControlService));
            }
        }

        private void StartHeartbeatTimer(int heartbeatInterval, int timeout)
        {
            _heartbeatTimer.Interval = heartbeatInterval * 1000;
            _timeout = timeout * 1000;
            _heartbeatTimer.Start();
            _timeoutTimer.Interval = 1000;
            _timeoutTimer.Start();
            _lastHeartbeatTime = DateTime.Now;
        }

        private void StopHeartbeatTimer()
        {
            _heartbeatTimer.Stop();
            _timeoutTimer.Stop();
        }

        private void UpdateHeartbeatTime() => _lastHeartbeatTime = DateTime.Now;

        private void OnStringReceived(string message)
        {
            if (message.Trim() == "pong")
                UpdateHeartbeatTime();
        }

        private void OnMessageReceived(string json)
        {
            if (string.IsNullOrEmpty(json)) return;

            json = json.Trim();
            if (!json.StartsWith("{") || !json.EndsWith("}")) return;

            Message<ReceivedPayloadBase>? message = JsonConvert.DeserializeObject<Message<ReceivedPayloadBase>>(json, _jsonSettings);
            switch (message?.Payload)
            {
                case StatusReceivedPayload status:
                    int timeout = status.HeartbeatInterval * status.HeartbeatTimes;
                    StartHeartbeatTimer(status.HeartbeatInterval, timeout);
                    SendCheckInCommand();
                    break;

                case ResponseReceivedPayload response:
                    HandleEipResponse(new Message<ResponseReceivedPayload>
                    {
                        Id = message.Id,
                        Payload = response,
                        Timestamp = message.Timestamp
                    });
                    break;
            }
        }

        private void HandleEipResponse(Message<ResponseReceivedPayload> msg)
        {
            if (_messageManager.FindByOriginalId(msg.Payload.OriginalId) == null)
            {
                MessageReceived?.Invoke(msg);
            }
            else
            {
                HandleLocalCommandResponse(msg);
            }
        }

        private void HandleLocalCommandResponse(Message<ResponseReceivedPayload> msg)
        {
            var payload = msg.Payload;
            if (payload != null && payload.ReturnCode == 0)
            {
                _messageManager.MarkResponded(msg.Id, payload);
            }
            else
            {
                MessageReceived?.Invoke(msg);
            }
        }

        private async void SendCheckInCommand()
        {
            CheckInCommand checkInCommand = new CheckInCommand
            {
                Id = $"cmd_{GetBeijingTimestamp()}",
                Timestamp = GetBeijingTimestamp(),
                Payload = new CheckInPayload
                {
                    DeviceType = _deviceType
                }
            };

            await WebSocketProtocol.SendJsonAsync(checkInCommand);
            CheckInFinished?.Invoke();
        }

        private long GetBeijingTimestamp()
        {
            return (long)(DateTime.UtcNow.AddHours(8) - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public async void SendGetCommand(GetCommand command)
        {
            var payload = new GetPayload { Command = command };
            var message = new Message<GetPayload>
            {
                Id = $"cmd_{GetBeijingTimestamp()}",
                Timestamp = GetBeijingTimestamp(),
                Payload = payload
            };
            _messageManager.AddMessage(message);
            await WebSocketProtocol.SendJsonAsync(message);
        }

        public async void SendSetCommand(SetCommand command, int[] values)
        {
            var parameter = new CommandParameter
            {
                Value = values.Length > 0 ? values[0] : 0,
                Max = values.Length > 1 ? values[1] : 0,
                Min = values.Length > 2 ? values[2] : 0
            };

            var payload = new SetPayload
            {
                Command = command,
                Parameter = parameter
            };

            var message = new Message<SetPayload>
            {
                Id = $"cmd_{GetBeijingTimestamp()}",
                Timestamp = GetBeijingTimestamp(),
                Payload = payload
            };

            _messageManager.AddMessage(message);
            await WebSocketProtocol.SendJsonAsync(message);
        }


        public void Dispose()
        {
            _heartbeatTimer?.Dispose();
            _timeoutTimer?.Dispose();
        }
    }
}