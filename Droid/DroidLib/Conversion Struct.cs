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

    public struct SClipConvTo
    {
        public IntPoint pt;

        public IntPoint Execute(Point3d buffPt, int convertScale)
        {
            double xd = (buffPt.X * convertScale);
            double yd = (buffPt.Y * convertScale);

            cInt cx = (cInt)(xd + 0.5);
            cInt cy = (cInt)(yd + 0.5);

            pt.X = cx;
            pt.Y = cy;

            return pt;
        }
    }

    public struct SClipConvFrom
    {
        public Point3d pt;

        public Point3d Execute(IntPoint clipPt, double zHeight, int scale)
        {
            double x = clipPt.X;
            double y = clipPt.Y;
            double z = zHeight;

            x /= scale;
            y /= scale;

            pt.X = x;
            pt.Y = y;
            pt.Z = z;

            return pt;
        }
    }
    #endregion

}
