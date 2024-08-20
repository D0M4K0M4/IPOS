using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPOS
{
    public class Cartesian
    {
        public string name { get; set; }
        public double x { get; set; }
        public double y { get; set; }

        public Cartesian(string Name, double X, double Y)
        {
            this.name = Name;
            this.x = X;
            this.y = Y;
        }
    }
}
