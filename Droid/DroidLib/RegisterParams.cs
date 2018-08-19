using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroidLib
{
    public class RegisterParams
    {

        private string[] text;

        public RegisterParams(string name, string nickname, string description)
        {
            text = new string [3] {name, nickname, description};
        }

        public RegisterParams(string name, string nickname, string description, string catagory, string subcatagory)
        {
            text = new string[5] { name, nickname, description, catagory, subcatagory };
        }

        public string this[int index]
        {
            get
            {
                return text[index];
            }            
        }
    }

}
