using System;
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

            var ser = MessagePackSerializer.Get<User>();


            using (var context = NetMQContext.Create())
            using (var server = context.CreateResponseSocket())
            {
                server.Bind("tcp://*:5555");


                while (true)
                {
                    var msg = server.ReceiveMultipartMessage();

                    if (msg.FrameCount == 2)
                    {
                        var method = msg[0].ConvertToString();
                        var user = ser.UnpackSingleObject(msg[1].ToByteArray());

                        typeof (Service).GetMethod(method).Invoke(service, new object[] {user});
                    }


                }


                //while (true)
                //{
                //    var message = server.ReceiveFrameBytes();
                //    var user = ser.UnpackSingleObject(message);

                //    Console.WriteLine("Received {0}", user.Username);

                //    user.Password = "asdfas";

                //    // processing the request
                //    Thread.Sleep(100);

                //    Console.WriteLine("Sending password");
                //    server.SendFrame(ser.PackSingleObject(user));
                //}
            }

        }
    }
}
