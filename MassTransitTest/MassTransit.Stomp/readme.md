Stomp Transport for MassTransit
===============================
This library adds a STOMP transport layer to the open source messaging project MassTransit. It was designed to enable messaging between different platforms. 

# Note
If you are using a seperate subscription service you should use a control bus to receive the subscription information:

            var serviceBus = ServiceBusFactory
                .New(sbc =>
                         {
                             sbc.UseSubscriptionService(_configuration.SubscriptionServiceUri);
                             sbc.ReceiveFrom(_configuration.RuntimeServicesUri);
                             sbc.UseStomp();
                             sbc.UseControlBus();

                             sbc.Subscribe(subs => subs.LoadFrom(_container));
                         });
						 
Please check the included test fixtures for more details.

# Example
For a sample see the MassTransit-JS project.

# Links
* [MassTransit project]{http://masstransit-project.com/)
* [MassTransit JS]{https://github.com/enix/MassTransit-JS)
* [Ultralight message broker]{https://github.com/enix/ultralight)

# Requirments
* MassTransit 2.x build
* .net 3.5 or higer
* a STOMP message broker

# License
Apache 2.0 - see LICENSE