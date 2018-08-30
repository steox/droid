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

        public RegisterParams(in string name, in string nickname, in string description)
        {
            text = new string [3] {name, nickname, description};
        }

        public RegisterParams(in string name, in string nickname, in string description, in string catagory, in string subcatagory)
        {
            text = new string[5] { name, nickname, description, catagory, subcatagory };
        }

        public string this[in int index]
        {
            get
            {
                return text[index];
            }            
        }
    }

}
