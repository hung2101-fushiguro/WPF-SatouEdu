using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace Repositories
{
    public interface IUserRepository
    {
        User GetUserByEmail(string email);
        void SaveUser(User u);
        List<User> GetStudents();
    }
}
