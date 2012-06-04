// Copyright 2011 Ernst Naezer, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

namespace MassTransit.Transports.Stomp
{
    using System;
    using System.Collections.Concurrent;
    using Ultralight;
    using Ultralight.Client;
    using log4net;

    /// <summary>
    /// Stompclient connection wrapper
    /// </summary>
    public class StompConnection
        : Connection
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (StompConnection));
        private readonly StompClient _stompClient;

        /// <summary>
        ///   Initializes a new instance of the <see cref="StompConnection" /> class.
        /// </summary>
        /// <param name="stompClient"> The stomp client. </param>
        public StompConnection(StompClient stompClient)
        {
            Messages = new ConcurrentQueue<StompMessage>();

            _stompClient = stompClient;
            _stompClient.OnMessage += m => Messages.Enqueue(m);
        }

        public ConcurrentQueue<StompMessage> Messages { get; set; }

        /// <summary>
        /// Subscribes to the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        public void Subscribe(string destination)
        {
            _stompClient.Subscribe(destination);
        }

        /// <summary>
        /// Unsubscribes from the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        public void Unsubscribe(string destination)
        {
            _stompClient.Unsubscribe(destination);
        }

        /// <summary>
        /// Sends the message to the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="message">The message.</param>
        public void Send(string destination, string message)
        {
            _stompClient.Send(destination, message);
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        public void Connect()
        {
            Disconnect();

            Log.Debug("Sending 'CONNECT' to server");
            _stompClient.Connect();
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_stompClient == null) return;

                if (_stompClient.IsConnected)
                {
                    if (Log.IsInfoEnabled)
                        Log.Debug("Sending 'DISCONNECT' to server");

                    _stompClient.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Failed to close STOMP connection.", ex);
            }
        }

        public void Dispose()
        {
        }
    }
}