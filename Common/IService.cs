using DataContracts;

namespace Common
{
    public interface IService
    {

        void AddUser(User user);
        User GetUser(string username);

    }
}
