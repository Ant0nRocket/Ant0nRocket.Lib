using Ant0nRocket.Lib.IO.SignalBus;
using Ant0nRocket.Lib.Logging;
using System;
using System.Net.Sockets;
using System.Text;

namespace Ant0nRocket.Lib.StandardImplimentations.Logging
{
    /// <summary>
    /// Class will perform UDP translation of a log messages
    /// </summary>
    public class UdpLogEntityHandler : ILogEntityHandler
    {
        private readonly UdpClient _udpClient;

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
                    $"{logEntity.ThreadId}|" +
                    $"{logEntity.Message}";
            var bytes = Encoding.UTF8.GetBytes(logMessage);

            _udpClient.Send(bytes, bytes.Length);
        }
    }
}
