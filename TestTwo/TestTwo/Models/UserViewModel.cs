using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTwo.Models
{
    public class UserViewModel
    {
        public Guid? Id { get; set; }
        public string Login { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Title { get; set; }
        public string ErrorMessage { get; set; }
        public bool CreationMode { get; set; }
    }
}
