namespace Ant0nRocket.Lib.Std20.Logging
{
    /// <summary>
    /// Interface that every log handler must fit
    /// </summary>
    public interface ILogEntityHandler
    {
        /// <summary>
        /// Handle <paramref name="logEntity"/>
        /// </summary>
        void Handle(LogEntity logEntity);
    }
}
