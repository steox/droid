using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DroidLib;

namespace Droid
{
    public static class Title
    {
        public static readonly RegisterParams CVolume = new RegisterParams
            ("Droid Volume (Cartesian)", "DVolC", "Defines the Volume of the printable area of machine", "Droid", "Droid");

    }

    public static class Info
    {
        public static readonly RegisterParams width = new RegisterParams("Width", "X", "X Axis of Print Area");
        public static readonly RegisterParams depth = new RegisterParams("Depth", "Y", "Y Axis of Print Area");
        public static readonly RegisterParams height = new RegisterParams("Height", "Z", "Z Axis of Print Area");
        public static readonly RegisterParams droidVolume = new RegisterParams("Droid Volume", "DV ->", "Droid Volume for use for Droid Components");
        public static readonly RegisterParams preview = new RegisterParams("Preview", "P", "Preview of Printable area");
    }
}
