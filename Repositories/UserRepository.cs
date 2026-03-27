using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        public User GetUserByEmail(string email) => UserDAO.Instance.GetUserByEmail(email);
        public void SaveUser(User u) => UserDAO.Instance.SaveUser(u);
        public List<User> GetStudents() => UserDAO.Instance.GetStudents();
    }
}
