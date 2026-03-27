using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IUserService
    {
        User GetUserByEmail(string email);
        void SaveUser(User u);
        List<User> GetStudents();
    }
}