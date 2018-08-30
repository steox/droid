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

    using cInt = Int64;

    #endregion

    #region Conversion Struct

    public class SClipConvTo
    {

        public IntPoint Execute(in Point3d buffPt, in int convertScale)
        {

            IntPoint pt = new IntPoint();
            pt.X = (cInt)((buffPt.X * convertScale) + 0.5);
            pt.Y = (cInt)((buffPt.Y * convertScale) + 0.5);

            return pt;
        }
    }

    public class SClipConvFrom
    {

        public Point3d Execute(in IntPoint clipPt, in double zHeight, in int scale)
        {

            Point3d pt = new Point3d();
            pt.X = (clipPt.X);
            pt.Y = (clipPt.Y);
            pt.Z = zHeight;
            pt.X /= scale;
            pt.Y /= scale;

            return pt;
        }
    }
    #endregion

}
