using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository iUserRepository;
        public UserService()
        {
            iUserRepository = new UserRepository();
        }
        public User GetUserByEmail(string email) => iUserRepository.GetUserByEmail(email);

        public void SaveUser(User u) => iUserRepository.SaveUser(u);

        public List<User> GetStudents() => iUserRepository.GetStudents();
    }
}
