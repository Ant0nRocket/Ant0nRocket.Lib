namespace Ant0nRocket.Lib.Std20
{
    /// <summary>
    /// Signal codes for <see cref="SignalBus.OnSignalCode"/>
    /// </summary>
    public enum SignalCode
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
