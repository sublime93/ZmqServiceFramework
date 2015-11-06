using Common;
using NetMQ;

namespace Client
{
    public class Proxy<T>
    {
        public NetMQContext Context { get; set; }
        public string EndpointAddress { get; set; }
        public T Service { get; private set; }

        public Proxy(NetMQContext context, string endpointAddress, ISerializer serializer)
        {
            EndpointAddress = endpointAddress;
            Context = context;

            Service = ServiceProxy<T>.Create(Context, EndpointAddress, serializer);
        }

        public Proxy(NetMQContext context, string endpointAddress)
        {
            EndpointAddress = endpointAddress;
            Context = context;

            Service = ServiceProxy<T>.Create(Context, EndpointAddress, new Serializer());
        }


    }

}
