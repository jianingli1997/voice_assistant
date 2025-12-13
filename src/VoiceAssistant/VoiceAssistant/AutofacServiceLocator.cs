// /*
//  * Class: AutofacServiceLocator.cs
//  * Author: lijianing
//  * Date: 2025-12-13
//  * Description: 
//  * Version: 1.0
//  */

using Autofac;

namespace VoiceAssistant;

public class AutofacServiceLocator
{
    private static AutofacServiceLocator? _instance;
    private static readonly object Lock = new();

    private readonly IComponentContext _context;

    private AutofacServiceLocator(IComponentContext context)
    {
        _context = context;
    }

    public static void Initialize(IComponentContext context)
    {
        lock (Lock)
        {
            if (_instance != null)
                return;

            _instance = new AutofacServiceLocator(context);
        }
    }

    public static AutofacServiceLocator Instance
    {
        get
        {
            lock (Lock)
            {
                if (_instance == null)
                    throw new InvalidOperationException(
                        "AutofacServiceLocator is not initialized. Call Initialize first.");

                return _instance;
            }
        }
    }

    public T Resolve<T>()
    {
        return _context.Resolve<T>();
    }

    public object Resolve(Type type)
    {
        return _context.Resolve(type);
    }


}