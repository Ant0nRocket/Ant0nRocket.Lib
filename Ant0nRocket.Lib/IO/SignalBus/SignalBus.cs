using System;

namespace Ant0nRocket.Lib.IO.SignalBus
{
    /// <summary>
    /// Class that is used for sending signals between
    /// consumers of the class events and functions
    /// </summary>
    public static class SignalBus
    {
        /// <summary>
        /// Raised when some signal from some channel (or with no channel) received.
        /// Date specified in UTC format.
        /// </summary>
        public static event Action<string, string?, DateTime>? OnSignalMessage;

        /// <summary>
        /// Raised when some <see cref="SignalBusCode"/> received.
        /// </summary>
        public static event Action<SignalBusCode>? OnSignalBusCode;

        /// <summary>
        /// Raised when some exception reseived.
        /// Well, you better not raise your exception while handling this one(s).
        /// </summary>
        public static event Action<Exception>? OnException;

        /// <summary>
        /// Send the <paramref name="signalMessage"/> to everyone
        /// subscribed to <see cref="OnSignalMessage"/>
        /// </summary>
        public static void Send(string signalMessage, string? signalChannel = default)
        {
            try
            {
                OnSignalMessage?.Invoke(signalMessage, signalChannel, DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                Send(ex);
            }
        }

        /// <summary>
        /// Send the <paramref name="signalCode"/> (<see cref="SignalBusCode"/>)
        /// to everyone subscribed to <see cref="OnSignalBusCode"/>
        /// </summary>
        public static void Send(SignalBusCode signalCode)
        {
            try
            {
                OnSignalBusCode?.Invoke(signalCode);
            }
            catch (Exception ex)
            {
                Send(ex);
            }
        }

        /// <summary>
        /// Send some exception to subscribers.<br />
        /// <b>ATTENSION!</b> It is not under try/catch control!<br />
        /// If some subscriber will raise exception during handling
        /// current exception it will be very funny :)
        /// </summary>
        public static void Send(Exception exception)
        {
            OnException?.Invoke(exception);
        }
    }
}
