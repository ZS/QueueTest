namespace MassTransit.Transports.Stomp
{
    using System;
    using Magnum.Extensions;
    using Ultralight.Client;
    using log4net;

    /// <summary>
    ///   Builds a <see cref="StompConnection" />
    /// </summary>
    public class StompConnectionFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(StompConnectionFactory));
        private readonly Func<Uri, StompClient> _clientBuilder;

        /// <summary>
        ///   Initializes a new instance of the <see cref="StompConnectionFactory" /> class.
        /// </summary>
        /// <param name="clientBuilder"> The client builder. </param>
        public StompConnectionFactory(Func<Uri, StompClient> clientBuilder)
        {
            if (clientBuilder == null) throw new ArgumentNullException("clientBuilder");

            _clientBuilder = clientBuilder;
        }

        /// <summary>
        ///   Builds the <see cref="StompConnection" /> to connect to the given location.
        /// </summary>
        /// <param name="location"> The location. </param>
        /// <returns> </returns>
        public StompConnection Build(Uri location)
        {
            var serverAddress = new UriBuilder("ws", location.Host, location.Port).Uri;

            if (Log.IsInfoEnabled)
                Log.Warn("Connecting {0}".FormatWith(location));

            return new StompConnection(_clientBuilder(serverAddress));
        }
    }
}