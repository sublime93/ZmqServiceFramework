using System.Threading;
using Common;
using DataContracts;

namespace Worker
{
    class Service : IService
    {
        public void AddUser(User user)
        {
            //Adding user
            Thread.Sleep(100);
        }

        public User GetUser(string username)
        {
            return new User()
            {
                Username = username,
                FirstName =  "First",
                LastName = "Last",
                Password = "Super secrete password"
            };
        }
    }
}
