using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace wpfKeyBoard.Model
{
    public class Language
    {
        public string ISO2Code { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public string Description { get; set; }

        public bool IsDefault { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var l = (Language) obj;
            return this.ISO2Code.Equals(l.ISO2Code) && this.Name.Equals(l.Name)
                   && this.NativeName.Equals(l.NativeName) && this.Description.Equals(l.Description);
        }


        public override int GetHashCode()
        {
            int hashcode = 87;
            hashcode += ISO2Code.GetHashCode() + Name.GetHashCode() + NativeName.GetHashCode() +
                        Description.GetHashCode();
            return hashcode;
        }
    }
}
