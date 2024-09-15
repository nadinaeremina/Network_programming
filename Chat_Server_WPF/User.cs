using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Server_WPF
{
    public class User
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {
            return "Name: {} Message: {}";
        }
    }
}