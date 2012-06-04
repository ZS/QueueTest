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
    using Exceptions;
    using Magnum.Extensions;
    using Magnum.Threading;

    public class StompTransportFactory
        : ITransportFactory
    {
        private readonly ReaderWriterLockedDictionary<Uri, ConnectionHandler<StompConnection>> _connectionCache;
        private StompConnectionFactory _connectionFactory;
        private bool _disposed;

        /// <summary>
        ///   Initializes a new instance of the <see cref="StompTransportFactory" /> class.
        /// </summary>
        public StompTransportFactory()
        {
            _connectionCache = new ReaderWriterLockedDictionary<Uri, ConnectionHandler<StompConnection>>();
        }

        /// <summary>
        ///   Gets the scheme.
        /// </summary>
        public string Scheme
        {
            get { return "stomp"; }
        }

        public void SetConnectionFactory(StompConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        ///   Builds the loopback.
        /// </summary>
        /// <param name="settings"> The settings. </param>
        /// <returns> </returns>
        public IDuplexTransport BuildLoopback(ITransportSettings settings)
        {
            return new Transport(settings.Address, () => BuildInbound(settings), () => BuildOutbound(settings));
        }

        public IInboundTransport BuildInbound(ITransportSettings settings)
        {
            EnsureProtocolIsCorrect(settings.Address.Uri);

            var client = GetConnection(settings.Address);
            return new InboundStompTransport(settings.Address, client);
        }

        public IOutboundTransport BuildOutbound(ITransportSettings settings)
        {
            EnsureProtocolIsCorrect(settings.Address.Uri);

            var client = GetConnection(settings.Address);
            return new OutboundStompTransport(settings.Address, client);
        }

        public IOutboundTransport BuildError(ITransportSettings settings)
        {
            EnsureProtocolIsCorrect(settings.Address.Uri);

            var client = GetConnection(settings.Address);
            return new OutboundStompTransport(settings.Address, client);
        }

        /// <summary>
        ///   Ensures the protocol is correct.
        /// </summary>
        /// <param name="address"> The address. </param>
        private void EnsureProtocolIsCorrect(Uri address)
        {
            if (address.Scheme != Scheme)
                throw new EndpointException(address,
                                            string.Format("Address must start with 'stomp' not '{0}'", address.Scheme));
        }

        private ConnectionHandler<StompConnection> GetConnection(IEndpointAddress address)
        {
            return _connectionCache
                .Retrieve(address.Uri,
                          () =>
                              {
                                  var connection = _connectionFactory.Build(address.Uri);
                                  var connectionHandler = new ConnectionHandlerImpl<StompConnection>(connection);
                                  return connectionHandler;
                              });
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _connectionCache.Values.Each(x => x.Dispose());
                _connectionCache.Clear();

                _connectionCache.Dispose();
            }

            _disposed = true;
        }

        ~StompTransportFactory()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}