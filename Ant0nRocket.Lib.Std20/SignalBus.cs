using System;

namespace Ant0nRocket.Lib.Std20
{
    /// <summary>
    /// Class that is used for sending signals between
    /// consumers of the class events and functions
    /// </summary>
    public static class SignalBus
    {
        /// <summary>
        /// Raised when someone send a message with <see cref="Send(string)"/>
        /// </summary>
        public static event Action<string>? OnSignalMessage;

        /// <summary>
        /// Raised when someone send a <see cref="SignalCode"/> with <see cref="Send(SignalCode)"/>
        /// </summary>
        public static event Action<SignalCode>? OnSignalCode;

        /// <summary>
        /// Raised when someone send an exception with <see cref="Send(Exception)"/>
        /// </summary>
        public static event Action<Exception>? OnException;

        /// <summary>
        /// Send the <paramref name="signalMessage"/> to everyone
        /// subscribed to <see cref="OnSignalMessage"/>
        /// </summary>
        public static void Send(string signalMessage)
        {
            try
            {
                OnSignalMessage?.Invoke(signalMessage);
            }
            catch (Exception ex)
            {
                Send(ex);
            }
        }

        /// <summary>
        /// Send the <paramref name="signalCode"/> (<see cref="SignalCode"/>)
        /// to everyone subscribed to <see cref="OnSignalCode"/>
        /// </summary>
        public static void Send(SignalCode signalCode)
        {
            try
            {
                OnSignalCode?.Invoke(signalCode);
            }
            catch (Exception ex)
            {
                Send(ex);
            }
        }

        /// <summary>
        /// Send the <paramref name="exception"/> to everyone
        /// subscribed to <see cref="OnException"/>
        /// </summary>
        public static void Send(Exception exception)
        {
            OnException?.Invoke(exception);
        }
    }
}
