using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPOS
{
    internal class LOD
    {
        public int id {  get; set; }
        public string modelName { get; set; }
        public bool interior { get; set; }
        public int drawDis { get; set; }
        public int flags { get; set; }

        public int isLOD { get; set; }

        public LOD(int ID, string modelName, bool interior, int DrawDis, int flags)
        {
            this.id = ID;
            this.modelName = modelName;
            this.interior = interior;
            this.drawDis = DrawDis;
            this.flags = flags;
        }
    }
}
