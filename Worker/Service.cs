using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using DataContracts;

namespace Worker
{
    class Service : IService
    {
        public void AddUser(User user)
        {
            //Adding user
            System.Threading.Thread.Sleep(100);
        }

        public User GetUser(string username)
        {
            return new User()
            {
                Username = username,
                FirstName =  "First",
                LastName = "Last"
            };
        }
    }
}
