using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ant0nRocket.Lib.Network
{
    /// <summary>
    /// Class that implement very simple network beacon.
    /// It will not receive any signals, only broadcasting of initialy set message.
    /// Consumers decide, what to broadcast.
    /// </summary>
    public class UdpBeacon : IAsyncDisposable
    {
        private const string DEFAULT_BEACON_MESSAGE = "..!..";
        private const int DEFAULT_BEACON_PORT = 17171;
        private const int DEFAULT_BEACON_INTERVAL_MS = 1717;

        private readonly string _beaconMessage;
        private readonly byte[] _beaconMessageBytes;
        private readonly int _beaconPort;
        private readonly int _beaconIntervalMs;

        private readonly UdpClient _udpClient;
        private readonly IPEndPoint _ipEndPoint;

        private CancellationTokenSource? _cancellationTokenSource;

        /// <summary>
        /// Default .ctor
        /// </summary>
        public UdpBeacon(string beaconMessage = DEFAULT_BEACON_MESSAGE, int beaconPort = DEFAULT_BEACON_PORT, int beaconIntervalMs = DEFAULT_BEACON_INTERVAL_MS)
        {
            if (string.IsNullOrWhiteSpace(beaconMessage))
                throw new InvalidEnumArgumentException(nameof(beaconMessage));

            _beaconMessage = beaconMessage;
            _beaconMessageBytes = Encoding.UTF8.GetBytes(_beaconMessage);
            _beaconPort = beaconPort;
            _beaconIntervalMs = beaconIntervalMs;

            _ipEndPoint = new IPEndPoint(IPAddress.Broadcast, _beaconPort);
            _udpClient = new(_ipEndPoint) { EnableBroadcast = true };
        }

        

        /// <summary>
        /// Start sending broadcast message to specified port
        /// </summary>
        public async Task<UdpBeacon> StartAsync()
        {
            _cancellationTokenSource = new();


            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                await _udpClient.SendAsync(_beaconMessageBytes, _beaconMessageBytes.Length);
                await Task.Delay(_beaconIntervalMs);
            }

            return this;
        }

        public void Dispose()
        {
            
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
