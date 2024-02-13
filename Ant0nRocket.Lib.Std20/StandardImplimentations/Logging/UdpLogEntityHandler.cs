using Ant0nRocket.Lib.Std20.Logging;
using System;
using System.Net.Sockets;
using System.Text;

namespace Ant0nRocket.Lib.Std20.StandardImplimentations.Logging
{
    /// <summary>
    /// Class will perform UDP translation of a log messages
    /// </summary>
    public class UdpLogEntityHandler : ILogEntityHandler
    {
        private readonly UdpClient _udpClient = default;

        public UdpLogEntityHandler(string hostname, int port)
        {
            try
            {
                _udpClient = new UdpClient(hostname, port);
            }
            catch (Exception ex)
            {
                SignalBus.Send(ex);
            }
        }

        public void Handle(LogEntity logEntity)
        {
            if (_udpClient == default) return;

            var logMessage =
                    $"{logEntity.DateTimeLocal:yyyy-MM-dd HH:mm:ss}|" +
                    $"{logEntity.LogLevel.ToString().ToUpper()}|" +
                    $"{logEntity.Message}";
            var bytes = Encoding.UTF8.GetBytes(logMessage);

            _udpClient.Send(bytes, bytes.Length);
        }
    }
}
