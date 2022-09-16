using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestTwo.Models;
using TestTwo.Services;

namespace TestTwo.Controllers
{
    [Route("[controller]")]
    public class UserController:Controller
    {
        protected IUserService _userService { get; }
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("[action]/{userId?}")]
        public async Task<IActionResult> Index([FromRoute]string userId = null)
        {
            var model = new UserViewModel
            {
                Title = string.IsNullOrEmpty(userId) ? "Create a new user" : "Edit a User",
                CreationMode =true
            };

            if(string.IsNullOrEmpty(userId))
                return View(model);



            try
            {
                Guid id = Guid.Parse(userId);
                var user = _userService.Get(id);
                if(user is null)
                {
                    return RedirectToAction("");
                }
                model.Firstname = user.Firstname;
                model.Lastname = user.Lastname;
                model.Id = user.Id;
                model.Login = user.Login;
                model.ErrorMessage = "";
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"User '{userId}' was not found!";
            }

            return View(model);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> UserList()
         => View(_userService.GetAll());

        [HttpPost]
        [Route("[action]/{userId?}")]
        public IActionResult Index([FromRoute]string userId,[FromForm] UserViewModel model)
        {
            Guid? guid = null;
            User user = null;
            if (!string.IsNullOrEmpty(userId))
            {
                guid = Guid.Parse(userId);
                user =_userService.GetBy(x =>x.Id==guid );
            }
            
            

            if (!model.CreationMode && guid != null ||
                _userService.GetBy(x => x.Login == model.Login.ToUpperInvariant()) != null)
            {
                if (guid is null)
                    RedirectToAction("UserList");

                _userService.Update(Guid.Parse(userId),new Models.User { Firstname = model.Firstname, Lastname = model.Lastname, Login = model.Login, Id =  (Guid)guid});
                return RedirectToAction("UserList");
            }

            _userService.Create(new Models.User { Firstname = model.Firstname.ToUpperInvariant(), Lastname = model.Lastname?.ToUpperInvariant(), Login= model.Login.ToUpperInvariant() });

            return RedirectToAction("UserList");
        }
    }
}
