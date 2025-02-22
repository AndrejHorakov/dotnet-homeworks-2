using System;
using System.Data;
using System.Threading;

namespace Hw3;

public class SingleInitializationSingleton
{
    // Lazy initialization
    private static Lazy<SingleInitializationSingleton> _singleton = 
        new Lazy<SingleInitializationSingleton>(() => new SingleInitializationSingleton());
    
    private static readonly object Locker = new();

    private static volatile bool _isInitialized = false;

    public const int DefaultDelay = 3_000;
    
    public int Delay { get; }

    private SingleInitializationSingleton(int delay = DefaultDelay)
    {
        Delay = delay;
        // imitation of complex initialization logic
        Thread.Sleep(delay);
    }

    public static void Reset()
    {
        if (!_isInitialized)
            return;
        lock (Locker)
        {
            if (_isInitialized)
            {
                _singleton = new Lazy<SingleInitializationSingleton>(() => new SingleInitializationSingleton());
                _isInitialized = false;
            }
        }
    }

    public static void Initialize(int delay)
    {
        if (_isInitialized)
        {
            throw new InvalidOperationException();
        }
        lock (Locker)
        {
            if (!_isInitialized)
            {
                _singleton = new Lazy<SingleInitializationSingleton>(() => new SingleInitializationSingleton(delay));
                _isInitialized = true;
            }
        }
    }

    public static SingleInitializationSingleton Instance => _singleton.Value;

}