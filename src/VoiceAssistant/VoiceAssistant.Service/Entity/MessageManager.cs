using Newtonsoft.Json;
using System.Collections.Concurrent;
using VoiceAssistant.Service.Entity;
using VoiceAssistant.Service.Entity.Payloads;

namespace VoiceAssistant.Service.Entity
{
    public sealed class MessageManager
    {
        private static readonly Lazy<MessageManager> _instance =
            new Lazy<MessageManager>(() => new MessageManager());

        private readonly ConcurrentDictionary<string, SentMessageInfo> _messages =
            new ConcurrentDictionary<string, SentMessageInfo>();

        public static MessageManager Instance => _instance.Value;

        private MessageManager()
        {
        }

        public void AddMessage<TPayload>(Message<TPayload> message)
        {
            var sentMessageInfo = new SentMessageInfo
            {
                Id = message.Id,
                Timestamp = message.Timestamp,
                PayloadType = typeof(TPayload).Name,
                JsonContent = JsonConvert.SerializeObject(message, Formatting.None),
                Status = MessageStatus.Sent
            };

            _messages[message.Id] = sentMessageInfo;
        }

        public SentMessageInfo? FindByOriginalId(string oriId)
        {
            _messages.TryGetValue(oriId, out var sentMessageInfo);
            return sentMessageInfo;
        }

        public void MarkResponded(string oriId, ResponseReceivedPayload responseReceived)
        {
            if (!_messages.TryGetValue(oriId, out SentMessageInfo? sentMessageInfo))
                return;

            sentMessageInfo.Status = MessageStatus.Responded;
            sentMessageInfo.ResponseJson =
                JsonConvert.SerializeObject(responseReceived, Formatting.None);
            sentMessageInfo.ResponseTime =
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public IEnumerable<SentMessageInfo> GetAllMessages()
        {
            return _messages.Values;
        }
    }
}