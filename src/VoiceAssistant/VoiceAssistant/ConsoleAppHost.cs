// /*
//  * Class: ConsoleAppHost.cs
//  * Author: lijianing
//  * Date: 2025-12-13
//  * Description: 
//  * Version: 1.0
//  */

using Autofac;
using VoiceAssistant.Log;
using VoiceAssistant.Service;

namespace VoiceAssistant;

public sealed class ConsoleAppHost : IDisposable, IAsyncDisposable
{
    private IContainer _container;

    public void Start()
    {
        ContainerBuilder builder = new ContainerBuilder();

        //  只注册 Service
        builder.RegisterModule<ServiceModule>();

        _container = builder.Build();

        //  初始化 ServiceLocator
        AutofacServiceLocator.Initialize(_container);
    }

    public void Dispose()
    {
        _container.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}

public class ServiceModule:Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<CentralCommunicationService>().AsSelf().SingleInstance();
        builder.RegisterType<CentralizedControlService>().AsSelf().WithParameter("deviceType","VOICE").SingleInstance();
    }
}