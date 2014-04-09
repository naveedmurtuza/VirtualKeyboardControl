using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfKeyBoard.Model
{
    public class Keyboard
    {
       
        public Language Language { get; set; }

        public List<Switcheroo> Switcheroos { get; set; }

        
        public string DefaultSwitcheroo { get; set; }
        public int ColumnCount { get; set; }
        public int RowCount { get; set; }
    }
}
