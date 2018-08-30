using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Windows.Forms;

using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using ClipperLib;

namespace DroidLib
{
    
    #region Droid Volume class


    public class DroidVolume
    {
        public Curve[] volumeOutline;
        public double[] size;

        public DroidVolume()
        { }

        public DroidVolume(in double x, in double y, in double z)
        {
            size = new double[2] { x, y };

            Polyline bottom = new Polyline();
            Polyline top = new Polyline();
            Polyline pillar1 = new Polyline();
            Polyline pillar2 = new Polyline();
            Polyline pillar3 = new Polyline();
            Polyline pillar4 = new Polyline();

            bottom.Add(0, 0, 0);
            bottom.Add(0, y, 0);
            bottom.Add(x, y, 0);
            bottom.Add(x, 0, 0);
            bottom.Add(0, 0, 0);
            PolylineCurve bt = new PolylineCurve(bottom);

            top.Add(0, 0, z);
            top.Add(0, y, z);
            top.Add(x, y, z);
            top.Add(x, 0, z);
            top.Add(0, 0, z);
            PolylineCurve tp = new PolylineCurve(top);
            
            pillar1.Add(0, 0, 0);
            pillar1.Add(0, 0, z);
            PolylineCurve p1 = new PolylineCurve(pillar1);
            
            pillar2.Add(0, y, 0);
            pillar2.Add(0, y, z);
            PolylineCurve p2 = new PolylineCurve(pillar2);
            
            pillar3.Add(x, y, 0);
            pillar3.Add(x, y, z);
            PolylineCurve p3 = new PolylineCurve(pillar3);
            
            pillar4.Add(x, 0, 0);
            pillar4.Add(x, 0, z);
            PolylineCurve p4 = new PolylineCurve(pillar4);

            volumeOutline = new Curve[6] { bt, tp, p1, p2, p3, p4 };

        }
        public DroidVolume(in double diameter, in double z)
        {

            Circle bot = new Circle(new Point3d(0, 0, 0), (diameter / 2));
            Circle top = new Circle(new Point3d(0, 0, z), (diameter / 2));

            volumeOutline = new Curve[2] { bot.ToNurbsCurve(), top.ToNurbsCurve() };
        }
    }

    #endregion
    
}
