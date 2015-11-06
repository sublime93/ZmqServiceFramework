using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
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

            using (var context = NetMQContext.Create())
            {


                Console.WriteLine("Sending user");

                var p = new Proxy<IService>(context, "tcp://localhost:5555");

                p.Service.AddUser(user);

                for (var i = 0; i < 10; i++)
                {
                    var u = p.Service.GetUser("sdf");
                    Console.WriteLine("Received {0}", u.Password);
                }


            }

        }
    }


    
}
