using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTwo.Models;

namespace TestTwo.Services
{
    public interface IUserService
    {
        // Creates a new user and returns its identifier.
        // Throws an exception if a user is null. 
        // Throws an exception if a login is null or empty. 
        Guid Create(User user);
        // Returns a found user or null. 
        User Get(Guid id);
        // Returns true if a user was updated (firstname and lastname) or false if it was not possible to find it. 
        // Throws an exception if an userToUpdate is null. 
        bool Update(Guid id,User userToUpdate);

        User GetBy(Func<User, bool> userSelector);

        public IList<User> GetAll();

    }
}
