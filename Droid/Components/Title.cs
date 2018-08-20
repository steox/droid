using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DroidLib;

namespace Droid.Components
{
    public static class Title
    {
        public static readonly RegisterParams cVolume = new RegisterParams
            ("Droid Volume (Cartesian)", "DVolC", "Defines the Volume of the printable area of machine", "Droid", "Droid");

        public static readonly RegisterParams dVolume = new RegisterParams
            ("Droid Volume (Delta)", "DVolD", "Defines the Volume of the printable area of machine", "Droid", "Droid");

        public static readonly RegisterParams footer = new RegisterParams
            ("Droid Gcode Footer", "DGFoot", "Gcode Footer creation", "Droid", "Gcode");

        public static readonly RegisterParams gcode = new RegisterParams
            ("Droid Gcode Creator", "GCode", "Creates Gcode infomation from Droid components", "Droid", "Gcode");

        public static readonly RegisterParams header = new RegisterParams
            ("Droid Gcode Header", "DGHead", "Gcode Header creation", "Droid", "Gcode");

        public static readonly RegisterParams save = new RegisterParams
            ("Droid Save Gcode", "SaveG", "Save Gcode File", "Droid", "Gcode");

        public static readonly RegisterParams paths = new RegisterParams
            ("Droid Paths", "DPath", "Set custom user defined curves (Polylines) for 3d Printing. Input order is important (Unless Sort Z is true)", "Droid", "Droid");

        public static readonly RegisterParams parameters = new RegisterParams
            ("Droid Parameters", "DPmters", "Custom Parameter creation for Droid", "Droid", "Droid");

        public static readonly RegisterParams mesh = new RegisterParams
            ("Droid Mesh", "DMesh", "Converts and Centers mesh for Droid Slicer", "Droid", "Droid");

        public static readonly RegisterParams slicer = new RegisterParams
            ("Droid Slicer", "Dslice", "Slicer of Droid Mesh in preparation for 3D Printing", "Droid", "Droid");
    }
    
}
