using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;
using Common;
using MsgPack.Serialization;
using NetMQ;
using NetMQ.Sockets;

namespace Client
{


    public class ServiceProxy<T> : RealProxy
    {
        public NetMQContext Context { get; set; }
        public string EndpointAddress { get; set; }
        public ISerializer Serializer { get; set; }

        public ServiceProxy(NetMQContext context, string endpointAddress, ISerializer serializer) : base(typeof(T))
        {
            EndpointAddress = endpointAddress;
            Context = context;
            Serializer = serializer;
        }

        public static T Create(NetMQContext context, string endpointAddress, ISerializer serializer)
        {
            return (T)new ServiceProxy<T>(context, endpointAddress, serializer).GetTransparentProxy();
        }

        public T Create()
        {
            return (T)GetTransparentProxy();
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = (IMethodCallMessage)msg;
            var method = (MethodInfo)methodCall.MethodBase;

            try
            {
                using (var client = Context.CreateRequestSocket())
                {
                    client.Connect(EndpointAddress); //Connect to open new request socket

                    var netMqMsg = new NetMQMessage();
                    netMqMsg.Append(SignatureGenerator.GetMethodHash(method));

                    foreach (var arg in methodCall.Args)
                    {
                        //Serialize and append as new frame
                        netMqMsg.Append(Serializer.Serialize(arg));
                    }

                    //Send message
                    client.SendMultipartMessage(netMqMsg);

                    if (method.ReturnType != typeof(void))
                    {
                        //Wait for response
                        var responseBytes = client.ReceiveFrameBytes();
                        var response = Serializer.Deserialize(method.ReturnType, responseBytes);

                        return new ReturnMessage(response, null, 0, methodCall.LogicalCallContext, methodCall);
                    }

                    return new ReturnMessage(null, null); //return nothing because message return type is void
                }
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException && e.InnerException != null)
                {
                    return new ReturnMessage(e.InnerException, (IMethodCallMessage) msg);
                }

                return new ReturnMessage(e, (IMethodCallMessage) msg);
            }
        }
    }
}
