using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfKeyBoard.Model
{
public class Switcheroo
    {
    public Switcheroo()
    {
        
        KeyCollection = new List<VKey>();
    }
    public bool HasPages { get { return PageCount != 0; } }

    public int PageCount { get; set; }

    public string Name { get; set; }

    public string FontFamily { get; set; }

    public string FontFamilyUri { get; set; }

    public List<VKey> KeyCollection { get; internal set; }

    public bool Virtual { get; internal set; }

    public bool IsVolatile { get; internal set; }
    public bool IsDefault { get; set; }
    }
}
