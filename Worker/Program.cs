using System;
using System.Threading;
using DataContracts;
using MsgPack.Serialization;
using NetMQ;

namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            var ser = MessagePackSerializer.Get<User>();

            using (var context = NetMQContext.Create())
            using (var server = context.CreateResponseSocket())
            {
                server.Bind("tcp://*:5555");

                while (true)
                {
                    var message = server.ReceiveFrameBytes();
                    var user = ser.UnpackSingleObject(message);

                    Console.WriteLine("Received {0}", user.Username);

                    user.Password = "asdfas";

                    // processing the request
                    Thread.Sleep(100);

                    Console.WriteLine("Sending password");
                    server.SendFrame(ser.PackSingleObject(user));
                }
            }

        }
    }
}
