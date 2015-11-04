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
            var signatures = SignatureGenerator.GetSignatureList(service.GetType());


            var ser = MessagePackSerializer.Get<User>();


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
                        var deSer = MessagePackSerializer.Get(parameters[i].ParameterType);
                        var obj = deSer.UnpackSingleObject(msg.Pop().ToByteArray());
                        paramList.Add(obj);
                    }

                    var response = method.Invoke(service, paramList.ToArray());

                    var respMessage = ser.PackSingleObject((User)response);

                    server.SendFrame(respMessage);
                }

            }

        }
    }
}
