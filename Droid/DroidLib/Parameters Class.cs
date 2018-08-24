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

    #region Droid Parameters class


    public class DroidParameters
    {
        public double layerHeight = new double();
        public int scale = new int();
        public double nozzle = new double();
        public int infillPercent = new int();
        public int shellNumber = new int();
        public bool brimSkirt = new bool();
        public int brimSkirtInt = new int();
        public int capThickness = new int();
        public int printSpeed = new int();
        public int travelSpeed = new int();
        public bool retraction = new bool();
        public double rectractionDistance = new double();
        public double firstLayerHeight = new double();
        public double filamentDiameter = new double();
        public int retractionSpeed = new int();
        public double flowRate = new double();

        public DroidParameters()
        { }
    }
    #endregion

}
