using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MassTransit;

namespace MassTransitTest
{
    public class Request : CorrelatedBy<int>
    {
        public int CorrelationId { get; set; }
        public string Text { get; set; }
    }

    public class Response : CorrelatedBy<int>
    {
        public int CorrelationId { get; set; }
        public bool Successful { get; set; }
    }
}
