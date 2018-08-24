using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DroidLib;

namespace Droid.Components
{   
    public static class Info
    {
        public static readonly RegisterParams width = new RegisterParams("Width", "X", "X Axis measurement of Print Area");
        public static readonly RegisterParams depth = new RegisterParams("Depth", "Y", "Y Axis measurement of Print Area");
        public static readonly RegisterParams height = new RegisterParams("Height", "Z", "Z Axis measurement of Print Area");
        public static readonly RegisterParams diameter = new RegisterParams("Diameter", "Dmtr", "Diameter measurement of Print Area");
        public static readonly RegisterParams droidVolume = new RegisterParams("Droid Volume", "DVol", "Volume for use with Droid");
        public static readonly RegisterParams preview = new RegisterParams("Preview", "P", "Preview of Position");
        public static readonly RegisterParams footer = new RegisterParams("Footer", "Foot", "Footer Gcode text");
        public static readonly RegisterParams paths = new RegisterParams("Droid Paths", "DP", "Droid Paths data");
        public static readonly RegisterParams parameters = new RegisterParams("Droid Parameters", "DPr", "Input Droid Parameters");
        public static readonly RegisterParams header = new RegisterParams("Droid Header", "Head", "Header Gcode Text");
        public static readonly RegisterParams gcode = new RegisterParams("Gcode", "GCd", "Gcode text");
        public static readonly RegisterParams heatedBed = new RegisterParams("Has Heated Bed", "HB", "Option to use Heated Bed");
        public static readonly RegisterParams heatedBedTemp = new RegisterParams("Heated Bed Temp", "HBT", "Set Heated Bed Temperature");
        public static readonly RegisterParams extruderTemp = new RegisterParams("Extruder Temp", "ET", "Set Extruder Hotend Temperature");
        public static readonly RegisterParams fan = new RegisterParams("Fan", "Fan", "Use Fan (True = On)");
        public static readonly RegisterParams mesh = new RegisterParams("Mesh", "M", "Input (closed) Mesh");
        public static readonly RegisterParams xPos = new RegisterParams("X Position", "XPos", "Re-Position of Mesh on X-Axis");
        public static readonly RegisterParams yPos = new RegisterParams("Y Position", "YPos", "Re-Position of Mesh on Y-Axis");
        public static readonly RegisterParams droidMesh = new RegisterParams("Droid Mesh", "DMesh", "Droid Mesh for use with Droid");
        public static readonly RegisterParams layerHeight = new RegisterParams("Layer Height", "LH", "Layer Height Resolution");
        public static readonly RegisterParams firstLayerHeight = new RegisterParams("First Layer Height", "FLH", "First Layer Height Resolution");
        public static readonly RegisterParams precision = new RegisterParams("Precision", "P", "Precision of sliced geometry. Default value is 128");
        public static readonly RegisterParams nozzle = new RegisterParams("Nozzle", "N", "Nozzle diameter");
        public static readonly RegisterParams infill = new RegisterParams("Infill", "I", "Infill percentage");
        public static readonly RegisterParams shell = new RegisterParams("Shell Number", "S", "Number of Outer Shells, in Nozzle thickness multiples");
        public static readonly RegisterParams skirt = new RegisterParams("Brim / Skirt", "BS", "For Brim = True, for Skirt = False");
        public static readonly RegisterParams brimDist = new RegisterParams("Brim / Skirt distance", "BSD", "Number of Brims or Distance of Skirt. 0 = none");
        public static readonly RegisterParams cap = new RegisterParams("Top / Bottom", "TB", "Number of layers as Top and Bottom Caps");
        public static readonly RegisterParams printSpeed = new RegisterParams("Print Speed", "PS", "Speed of Printing in units per/s");
        public static readonly RegisterParams travelSpeed = new RegisterParams("Travel Speed", "TS", "Speed of travelling when not printing in units per/s");
        public static readonly RegisterParams retraction = new RegisterParams("Retraction", "R", "Enable Retraction = true");
        public static readonly RegisterParams retractionDist = new RegisterParams("Rectraction Distance", "RD", "Retraction Distance");
        public static readonly RegisterParams retractionSpeed = new RegisterParams("Retraction Speed", "RS", "Feedrate of Retraction in units per/s");
        public static readonly RegisterParams filament = new RegisterParams("Filament Diameter", "F", "Filament Diameter (Extrusion volume (mm^2) per mm of extrusion)");
        public static readonly RegisterParams polylines = new RegisterParams("Polylines", "P", "List of Polyline Curves (Convert line types to Polyline)");
        public static readonly RegisterParams sortZ = new RegisterParams("Sort Z", "SortZ", "Sort List of Curves by ascending Z Value at start of Curve");
        public static readonly RegisterParams save = new RegisterParams("Save", "Save", "Save Gcode. Add Grasshopper Boolean Button");
        public static readonly RegisterParams previewContour = new RegisterParams("Preview Contour", "PContour", "Contour Out for preview");
        public static readonly RegisterParams previewShell = new RegisterParams("Preview Shell", "PShell", "Shell Out for preview");
        public static readonly RegisterParams previewInfill = new RegisterParams("Preview Infill", "PInfill", "Infill Out for preview");
        public static readonly RegisterParams previewSkirt = new RegisterParams("Preview Skirt", "PSkirt", "Skirt Out for preview");
        public static readonly RegisterParams previewCap = new RegisterParams("Preview Cap", "Pcap", "Cap Out for preview");
        public static readonly RegisterParams flowRate = new RegisterParams("Flow Rate", "FR", "Flow rate / Extrusion multiplier");
    }
}
