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
    #region Internal Definitions 
    
    using Polylines = List<Polyline>;

    #endregion

    #region Droid Paths class

    public class DroidPaths
    {
        public List<Polylines> wrapperList = new List<Polylines>();
        public Polylines printList = new Polylines();

        public DroidPaths()
        { }

        public DroidPaths(Polylines[] contours, Polylines[] shell, Polylines[] fill, Polylines[] skirt, Polylines[] cap)
        {
            List<Polylines> ordered = new List<Polylines>();
            Point3d pos = new Point3d(0, 0, 0);
            
            List<Polylines[]> wrapper = new List<Polylines[]>(5);
            wrapper.Add(skirt);
            wrapper.Add(contours);
            wrapper.Add(shell);
            wrapper.Add(fill);
            wrapper.Add(cap);
            
            for (int i = 0; i < wrapper.Count; i++)
            {
                Polylines group = new Polylines();
                foreach (Polylines x in wrapper[i])
                {
                    if (x != null)
                    {
                        foreach (Polyline y in x)
                        {
                            group.Add(y);
                        }
                    }
                }
                wrapperList.Add(group);
            }

            for (int i = 0; i < wrapper[1].Length; i++)
            {
                for (int j = 0; j < wrapper.Count; j++)
                {
                    if (wrapper[j][i] != null)
                    {
                        ordered.Add(wrapper[j][i]);
                    }
                }
            }
            foreach (Polylines x in ordered)
            {
                Polylines forTest = new Polylines();

                foreach (Polyline y in x)
                {
                    Polyline pl = new Polyline(y);
                    Polyline plr = new Polyline(y);
                    plr.Reverse();
                    forTest.Add(pl);
                    forTest.Add(plr);
                }
                for (int j = 0; j < x.Count; j++)
                {
                    List<double> dis = new List<double>();
                    foreach (Polyline z in forTest)
                    {
                        dis.Add(pos.DistanceTo(z.First));
                    }
                    Polyline buf = forTest[dis.IndexOf(dis.Min())];
                    Polyline bufr = new Polyline(forTest[dis.IndexOf(dis.Min())]);
                    bufr.Reverse();
                    printList.Add(buf);
                    pos = buf.Last();
                    forTest.Remove(buf);

                    Polyline bufrl = forTest.Find(p => p.First == bufr.First);
                    forTest.Remove(bufrl);
                    forTest = forTest.ToList();
                }
            }
        }
        
        public DroidPaths(List<Curve> Curves, bool Sort, DroidVolume DV)
        {
            List<Curve> allCurves = Curves;
            
            Polylines allPolyline = new Polylines();

            Polylines plines = new Polylines();
            foreach (Curve x in allCurves) 
            {
                Polyline getPL = new Polyline();
                if (x.TryGetPolyline(out getPL))
                {
                    Polyline pline = new Polyline();
                    foreach (Point3d p in getPL)
                    {
                        pline.Add(p);
                    }
                    plines.Add(pline);
                }
                /*else
                {
                    Polyline pline = new Polyline();
                    x.DivideByLength((para.nozzle * 2), true, out ptarr);
                    for (int i = 0; i < ptarr.Count(); i++)
                    {
                        //WIP
                        if (i == 0) pline.Add(ptarr[i]);
                        if (i == (ptarr.Count()-1)) pline.Add(ptarr[i]);
                    }
                }*/
            }

            if (Sort)
            {
                allPolyline = plines.OrderBy(pl => pl[0].Z).ToList(); 
            }
            else
            {
                allPolyline = plines;
            }
            printList = allPolyline;
        }
    }
    #endregion

}
