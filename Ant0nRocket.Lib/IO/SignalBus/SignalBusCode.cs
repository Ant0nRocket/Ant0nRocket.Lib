namespace Ant0nRocket.Lib.IO.SignalBus
{
    /// <summary>
    /// Signal codes for <see cref="SignalBus.OnSignalBusCode"/>
    /// </summary>
    public enum SignalBusCode
    {
        /// <summary>
        /// Application (GUI part) can proceed system messages queue
        /// </summary>
        ProcessMessages,

        /// <summary>
        /// Application is about to exit
        /// </summary>
        ExitApp,
    }
}
