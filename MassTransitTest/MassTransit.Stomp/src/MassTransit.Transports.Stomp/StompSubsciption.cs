namespace MassTransit.Transports.Stomp
{
    using System;
    using log4net;
    using Magnum.Extensions;

    public class StompSubsciption :
        ConnectionBinding<StompConnection>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(StompSubsciption));
        private readonly IEndpointAddress _address;

        public StompSubsciption(IEndpointAddress address)
        {
            if (address == null) throw new ArgumentNullException("address");
            _address = address;
        }

        public void Bind(StompConnection connection)
        {
            if (Log.IsInfoEnabled)
                Log.Warn("Subscribing to {0}".FormatWith(_address.Uri.PathAndQuery));

            connection.Subscribe(_address.Uri.PathAndQuery);
        }

        public void Unbind(StompConnection connection)
        {
            if (Log.IsInfoEnabled)
                Log.Warn("Unsubscribing to {0}".FormatWith(_address.Uri.PathAndQuery));

            connection.Unsubscribe(_address.Uri.PathAndQuery);
        }
    }
}