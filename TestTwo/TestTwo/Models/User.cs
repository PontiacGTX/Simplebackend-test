using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestTwo.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

    }
}
