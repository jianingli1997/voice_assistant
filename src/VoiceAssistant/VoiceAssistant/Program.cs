using VoiceAssistant.Service;
using VoiceAssistant.Service.Entity.Commands;
using VoiceAssistant.Utils;

namespace VoiceAssistant;

class Program
{
    static void Main(string[] args)
    {
        // AudioHelper.PlayWav("/usr/share/sounds/alsa/Front_Right.wav");
        
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
            service.ExecutionResultReceived += (result) =>
            {
                Console.WriteLine("执行结果: " + result);
            };
            var result=Console.ReadLine();
            if (result == "1")
            {
                service.SendSetCommand(SetCommand.Brightness, [1,10,0]);
            }

            Console.ReadLine();
        }
    }
}