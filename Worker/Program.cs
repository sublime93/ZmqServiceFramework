using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Common;
using DataContracts;
using MsgPack.Serialization;
using NetMQ;

namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            IService service = new Service();
            ISerializer serializer = new Serializer();
            var signatures = SignatureGenerator.GetSignatureList(service.GetType());

            using (var context = NetMQContext.Create())
            using (var server = context.CreateResponseSocket())
            {
                server.Bind("tcp://*:5555");


                while (true)
                {
                    var msg = server.ReceiveMultipartMessage();

                    var methodName = msg.Pop().ConvertToString(); //Method signature
                    var method = signatures[methodName]; //Get method from list
                    var paramList = new List<object>();

                    var parameters = method.GetParameters();

                    if (parameters.Count() != msg.FrameCount) throw new Exception("Invalid parameters");

                    for (var i = 0; i < parameters.Count(); i++)
                    {
                        var obj = serializer.Deserialize(parameters[i].ParameterType, msg.Pop().ToByteArray());
                        paramList.Add(obj);
                    }

                    var response = method.Invoke(service, paramList.ToArray());

                    var respMessage = serializer.Serialize(response);

                    server.SendFrame(respMessage);
                }

            }

        }
    }
}
