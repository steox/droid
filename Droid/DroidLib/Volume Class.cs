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
        public List<Curve> volumeOutline = new List<Curve>();
        public List<double> size = new List<double>();
        //public Brep boundingVolume = new Brep();

        public DroidVolume()
        { }

        public DroidVolume(double x, double y, double z)
        {
            size.Add(x);
            size.Add(y);
            size.Add(z);

            //BoundingBox bbx = new BoundingBox(0, 0, 0, x, y, z);
            //boundingVolume = bbx.ToBrep();

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
            volumeOutline.Add(bt);
            top.Add(0, 0, z);
            top.Add(0, y, z);
            top.Add(x, y, z);
            top.Add(x, 0, z);
            top.Add(0, 0, z);
            PolylineCurve tp = new PolylineCurve(top);
            volumeOutline.Add(tp);
            pillar1.Add(0, 0, 0);
            pillar1.Add(0, 0, z);
            PolylineCurve p1 = new PolylineCurve(pillar1);
            volumeOutline.Add(p1);
            pillar2.Add(0, y, 0);
            pillar2.Add(0, y, z);
            PolylineCurve p2 = new PolylineCurve(pillar2);
            volumeOutline.Add(p2);
            pillar3.Add(x, y, 0);
            pillar3.Add(x, y, z);
            PolylineCurve p3 = new PolylineCurve(pillar3);
            volumeOutline.Add(p3);
            pillar4.Add(x, 0, 0);
            pillar4.Add(x, 0, z);
            PolylineCurve p4 = new PolylineCurve(pillar4);
            volumeOutline.Add(p4);
        }
        public DroidVolume(double diameter, double z)
        {
            size.Add(diameter);
            size.Add(z);

            Circle bot = new Circle(new Point3d(0, 0, 0), (diameter / 2));
            Circle top = new Circle(new Point3d(0, 0, z), (diameter / 2));

            //Cylinder cl = new Cylinder(bot, z);
            //boundingVolume = cl.ToBrep(true, true);

            volumeOutline.Add(bot.ToNurbsCurve());
            volumeOutline.Add(top.ToNurbsCurve());
        }
    }

    #endregion
    
}
