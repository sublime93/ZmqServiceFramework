using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using DataContracts;
using MsgPack.Serialization;
using NetMQ;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {

            
            var user = new User();
            user.Username = "FBar";
            user.FirstName = "Foo";
            user.LastName = "Bar";

            var ser = MessagePackSerializer.Get<User>();
            var bytes = ser.PackSingleObject(user);


            using (var context = NetMQContext.Create())
            using (var client = context.CreateRequestSocket())
            {
                client.Connect("tcp://localhost:5555");

                Console.WriteLine("Sending user");


                var msg = new NetMQMessage();
                msg.Append("AddUser");
                msg.Append(bytes);

                client.SendMultipartMessage(msg);

                //client.SendFrame(bytes);

                var message = client.ReceiveFrameBytes();

                var newUser = ser.UnpackSingleObject(message);

                Console.WriteLine("Received {0}", newUser.Password);
            }

        }
    }
}
