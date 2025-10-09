namespace Ant0nRocket.Lib.Infrastructure
{
    /// <summary>
    /// Event class for <see cref="SignalBus"/>.
    /// Use it when app have been started (you decide when this moment happens)
    /// </summary>
    public record SbeAppStarted : SbeBase
    {
    }
}
