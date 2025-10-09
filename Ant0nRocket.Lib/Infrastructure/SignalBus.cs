using System;

namespace Ant0nRocket.Lib.Infrastructure
{
    /// <summary>
    /// Class that is used for sending very basic signals between app components.
    /// It works like UDP broadcaster: it don't give a fuck if something happen on the
    /// other side (at all!).
    /// </summary>
    public static class SignalBus
    {
        /// <summary>
        /// Raised when some <see cref="SbeBase"/> class received.
        /// </summary>
        public static event Action<SbeBase>? OnSignal;

        /// <summary>
        /// Send the <paramref name="signalBusEvent"/> (<see cref="SbeBase"/>)
        /// to everyone subscribed to <see cref="OnSignal"/>
        /// </summary>
        public static void Send(SbeBase signalBusEvent)
        {
            try
            {
                OnSignal?.Invoke(signalBusEvent);
            }
            catch
            {
                // I told you, we don't give a fuck!
                // All problems here - are consumer's problems
            }
        }
    }
}
