using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Ant0nRocket.Lib.Infrastructure;
using Ant0nRocket.Lib.Logging;

namespace Ant0nRocket.Lib.Network
{
    /// <summary>
    /// Class that implement very simple network beacon.
    /// It will not receive any signals, only broadcasting of initialy set message.
    /// Consumers decide, what to broadcast.
    /// </summary>
    public class UdpBeacon : IDisposable
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
        private readonly object _locker = new();
        private bool _disposed = false;

        /// <summary>
        /// Default .ctor
        /// </summary>
        public UdpBeacon(
            string beaconMessage = DEFAULT_BEACON_MESSAGE,
            int beaconPort = DEFAULT_BEACON_PORT,
            int beaconIntervalMs = DEFAULT_BEACON_INTERVAL_MS)
        {
            if (string.IsNullOrWhiteSpace(beaconMessage))
                throw new ArgumentException("Beacon message cannot be null or empty", nameof(beaconMessage));

            _beaconMessage = beaconMessage;
            _beaconMessageBytes = Encoding.UTF8.GetBytes(_beaconMessage);
            _beaconPort = beaconPort;
            _beaconIntervalMs = beaconIntervalMs;

            _ipEndPoint = new IPEndPoint(IPAddress.Broadcast, _beaconPort);
            _udpClient = new() { EnableBroadcast = true };

            SignalBus.OnSignal += SignalBus_OnSignal;
        }

        private void SignalBus_OnSignal(SbeBase signal)
        {
            if (signal is SbeAppExiting)
                Stop();
        }



        /// <summary>
        /// Start sending broadcast message to specified port
        /// </summary>
        public void Start()
        {
            lock(_locker)
            {
                // protection againt multiple calls
                if (_cancellationTokenSource != null) return;
                _ = Task.Run(StartJob); // infinite loop inside
            }
        }

        private async Task StartJob()
        {
            lock (_locker)
            {
                _cancellationTokenSource = new();
            }

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    await _udpClient.SendAsync(
                        _beaconMessageBytes,
                        _beaconMessageBytes.Length,
                        _ipEndPoint);
                    await Task.Delay(_beaconIntervalMs, _cancellationTokenSource.Token);
                }
                catch
                {
                    // AI context adjustment: we will not use OperationCanceledException
                    // AI context adjustment: we don't need log all of this shit, if beacon doen't work - will use GPS :)
                    //Logger.LogException(ex);
                }
            }
        }

        /// <summary>
        /// Initializes stop broadcasting proceduere
        /// </summary>
        public void Stop()
        {
            lock (_locker)
            {
                _cancellationTokenSource?.Cancel();
            }
        }

        /// <summary>
        /// For external components state control
        /// </summary>
        public bool IsRunning
        {
            get
            {
                lock (_locker)
                {
                    return _cancellationTokenSource != null &&
                           !_cancellationTokenSource.Token.IsCancellationRequested;
                }
            }
        }

        /// <summary>
        /// Disposing
        /// </summary>
        public void Dispose()
        {
            lock (_locker)
            {
                // N.B.! DONT'T use Stop() function inside current lock, it will lead to deadlock!

                if (_disposed || _cancellationTokenSource == null) return;

                SignalBus.OnSignal -= SignalBus_OnSignal;

                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                _udpClient?.Dispose();

                _disposed = true;
            }
        }
    }
}
