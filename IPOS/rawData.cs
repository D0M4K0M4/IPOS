using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace IPOS
{
    internal class rawData
    {
        //Header for dataGrid
        public string modelName { get; set; }
        public bool interior { get; set; }
        public int drawDis { get; set; }
        public int flags { get; set; }
        public string textureName { get; set; }
        public string lodName { get; set; }
        //Coordinate position
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
        //Rotation 
        public double qX { get; set; }
        public double qY { get; set; }
        public double qZ { get; set; }
        public double qW { get; set; }
        //IPL
        public int LOD { get; set; }
        public LOD lodData { get; set; }
        public int isLOD { get; set; }

        public rawData()
        {

        }

        public rawData(string modelName, string textureName, string lodName, double x, double y, double z, double qX, double qY, double qZ, double qW, bool interior, int lOD, LOD LODdata, int drawDis, int flags)
        {
            this.modelName = modelName;
            this.textureName = textureName;
            this.lodName = lodName;
            this.x = x;
            this.y = y;
            this.z = z;
            this.qX = qX;
            this.qY = qY;
            this.qZ = qZ;
            this.qW = qW;
            this.interior = interior;
            this.LOD = lOD;
            this.lodData = LODdata;
            this.drawDis = drawDis;
            this.flags = flags;
        }
    }
}
