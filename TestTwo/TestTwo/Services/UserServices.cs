using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTwo.Helpers;
using TestTwo.Models;

namespace TestTwo.Services
{
    public class UserServices : IUserService
    {
        static Dictionary<Guid,User> Users { get; set; } = new Dictionary<Guid, User>();
        public Guid Create(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException();
            }

            if (string.IsNullOrEmpty(user.Login))
            {
                throw new Exception("Login cannot be null or empty.");
            }

            if(Users.ContainsKey(user.Id))
            {
                return user.Id;
            }

            return CreateUser(user).Id;
        }
        public IList<User> GetAll()
            => Users.Values.ToList();
        public User GetBy(Func<User, bool> userSelector)
            => Users.Values.FirstOrDefault(userSelector);
        public User Get(Guid id)
        {
            if (Users.ContainsKey(id))
                return Users[id];

            var user =Generate(id);

            if(user!=null)
                Users.Add(id, user);

            return user;
        }
        public User CreateUser(User u)
        {
            u.Id = Guid.NewGuid();

            while (Users.ContainsKey(u.Id))
                u.Id = Guid.NewGuid();

            if (Users.Values.Any(x => x.Login == u.Login))
                return Users.Values.FirstOrDefault(x => x.Login == u.Login);

            Users.Add(u.Id, u);

            return u;
        }
        public User Generate(Guid id)
        {
            Random randN = new Random();
            int number = randN.Next(0, 5);
            if (number == 5)
            {
                return null;
            }

            Random rand = new Random(DateTime.Now.Second); // we need a random variable to select names randomly
            RandomName nameGen = new RandomName(rand); // create a new instance of the RandomName class
            Sex sexSelect;
            if (number % 2 == 0)
            {
                sexSelect = Sex.Male;
            }
            else
            {
                sexSelect = Sex.Female;
            }
            var userRet = new User();
            var name = nameGen.Generate(sexSelect);
            var namesplit = name.Split(" ");
            userRet.Firstname = namesplit[0];
            if (namesplit.Count() > 1)
            {
                userRet.Lastname = namesplit[1];
            }

            userRet.Id = id;
            userRet.Login = nameGen.Generate(Sex.Male).Split(" ")[0];

            return userRet;
        }

        public bool Update(Guid id,User userToUpdate)
        {
            if (userToUpdate == null)
            {
                throw new ArgumentNullException();
            }

            if(!Users.ContainsKey(userToUpdate.Id))
            {
                throw new ArgumentNullException();
            }

            var user = Users[userToUpdate.Id];
            user.Firstname = userToUpdate.Firstname;
            user.Lastname = userToUpdate.Lastname;
            user.Login = userToUpdate.Login;

            Users[userToUpdate.Id] = user;

            return user == userToUpdate;
        }
    }
}
