using VoiceAssistant.Service;

namespace VoiceAssistant;

class Program
{
    static void Main(string[] args)
    {
        using (ConsoleAppHost host = new ConsoleAppHost())
        {
            host.Start();

            Console.WriteLine("服务已启动，...");
            CentralCommunicationService _service = AutofacServiceLocator.Instance.Resolve<CentralCommunicationService>();
            CentralizedControlService service = AutofacServiceLocator.Instance.Resolve<CentralizedControlService>();
            _service.WebSocketProtocol.Connected += () =>
            {
                Console.WriteLine("已连接到中央通信服务。");
            };
            service.MessageReceived += (message) =>
            {
                Console.WriteLine("收到消息: " + message.Payload.GetType().Name);
            };
        }


       
    }
}