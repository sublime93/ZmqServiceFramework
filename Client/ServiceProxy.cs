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
        public RequestSocket Client { get; set; }

        private ServiceProxy(RequestSocket client) : base(typeof(T))
        {
            Client = client;
        }

        public static T Create(RequestSocket client)
        {
            return (T)new ServiceProxy<T>(client).GetTransparentProxy();
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = (IMethodCallMessage)msg;
            var method = (MethodInfo)methodCall.MethodBase;

            try
            {
                Console.WriteLine("Before invoke: " + method.Name);


                var netMqMsg = new NetMQMessage();
                netMqMsg.Append(SignatureGenerator.GetMethodHash(method));

                foreach (var arg in methodCall.Args)
                {
                    //Get serializer for type
                    var ser = MessagePackSerializer.Get(arg.GetType());

                    //Serialize 
                    var bytes = ser.PackSingleObject(arg);

                    //Append as new frame
                    netMqMsg.Append(bytes);
                }

                //Send message
                Client.SendMultipartMessage(netMqMsg);

                if (method.ReturnType != typeof(void))
                {

                    //Wait for response
                    var responseBytes = Client.ReceiveFrameBytes();
                    var deSer = MessagePackSerializer.Get(method.ReturnType);
                    var response = deSer.UnpackSingleObject(responseBytes);

                    return new ReturnMessage(response, null, 0, methodCall.LogicalCallContext, methodCall);
                }


                Console.WriteLine("After invoke: " + method.Name);
                return new ReturnMessage(null, null);
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
                if (e is TargetInvocationException && e.InnerException != null)
                {
                    return new ReturnMessage(e.InnerException, msg as IMethodCallMessage);
                }

                return new ReturnMessage(e, msg as IMethodCallMessage);
            }
        }
    }


    //public class ServiceProxy<T> : RealProxy
    //{
    //    private readonly T _instance;

    //    private ServiceProxy(T instance)
    //        : base(typeof(T))
    //    {
    //        _instance = instance;
    //    }

    //    public static T Create(T instance)
    //    {
    //        return (T)new ServiceProxy<T>(instance).GetTransparentProxy();
    //    }

    //    public override IMessage Invoke(IMessage msg)
    //    {
    //        var methodCall = (IMethodCallMessage)msg;
    //        var method = (MethodInfo)methodCall.MethodBase;

    //        try
    //        {
    //            Console.WriteLine("Before invoke: " + method.Name);
    //            var result = method.Invoke(_instance, methodCall.InArgs);
    //            Console.WriteLine("After invoke: " + method.Name);
    //            return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine("Exception: " + e);
    //            if (e is TargetInvocationException && e.InnerException != null)
    //            {
    //                return new ReturnMessage(e.InnerException, msg as IMethodCallMessage);
    //            }

    //            return new ReturnMessage(e, msg as IMethodCallMessage);
    //        }
    //    }
    //}
}
