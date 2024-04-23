using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo_des
{
    public class thanhvien
    {
        private string username;
        private string password;
        private string phone;
        private string gender;
        private string address;
        private string clb;

        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public string Phone { get => phone; set => phone = value; }
        public string Gender { get => gender; set => gender = value; }
        public string Address { get => address; set => address = value; }
        public string Clb { get => clb; set => clb = value; }
    }
}
