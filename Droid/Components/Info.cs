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
        public static readonly RegisterParams diameter = new RegisterParams("Diameter", "D", "Diameter measurement of Print Area");
        public static readonly RegisterParams droidVolume = new RegisterParams("Droid Volume", "DVol", "Volume for use with Droid");
        public static readonly RegisterParams preview = new RegisterParams("Preview", "P", "Preview of Position");
        public static readonly RegisterParams footer = new RegisterParams("Footer", "F ->", "Footer Gcode");
        public static readonly RegisterParams paths = new RegisterParams("Droid Paths", "-> DP", "Input Droid Paths");
        public static readonly RegisterParams parameters = new RegisterParams("Droid Parameters", "-> DPr", "Input Droid Parameters");
        public static readonly RegisterParams header = new RegisterParams("Droid Header", "-> H", "Input Header Text (from Droid component, or user custom text)");
        public static readonly RegisterParams gcode = new RegisterParams("Gcode", "GC ->", "Gcode as text");
        public static readonly RegisterParams heatedBed = new RegisterParams("Has Heated Bed", "HB", "Option to use Heated Bed");
        public static readonly RegisterParams heatedBedTemp = new RegisterParams("Heated Bed Temp", "HBT", "Set Heated Bed Temperature");
        public static readonly RegisterParams extruderTemp = new RegisterParams("Extruder Temp", "ET", "Set Extruder Hotend Temperature");
        public static readonly RegisterParams fan = new RegisterParams("Fan", "F", "Use Fan (Default = true)");
        public static readonly RegisterParams mesh = new RegisterParams("Mesh", "M", "Input (closed) Mesh");
        public static readonly RegisterParams xPos = new RegisterParams("X Position", "X", "Re-Position of Mesh on X-Axis");
        public static readonly RegisterParams yPos = new RegisterParams("Y Position", "Y", "Re-Position of Mesh on Y-Axis");
        public static readonly RegisterParams droidMesh = new RegisterParams("Droid Mesh", "DM ->", "Droid Mesh for use with Droid Components");
        public static readonly RegisterParams layerHeight = new RegisterParams("Layer Height", "LH", "Layer Height Resolution");
        public static readonly RegisterParams firstLayerHeight = new RegisterParams("First Layer Height", "FLH", "First Layer Height Resolution");
        public static readonly RegisterParams precision = new RegisterParams("Precision", "P", "Precision of sliced geometry (Whole Number [int]). Default value is 128");
        public static readonly RegisterParams nozzle = new RegisterParams("Nozzle", "N", "Nozzle diameter, in millimeters");
        public static readonly RegisterParams infill = new RegisterParams("Infill", "I", "Infill percentage (0% - 99%) (Whole Number [int])");
        public static readonly RegisterParams shell = new RegisterParams("Shell Number", "S", "Outer Shell thickness, in Nozzle thickness (Shell Thickness = Nozzle diameter * Shell Number). Default = 1");
        public static readonly RegisterParams skirt = new RegisterParams("Brim / Skirt", "BS", "For Brim = True, for Skirt = False. Default is False");
        public static readonly RegisterParams brimDist = new RegisterParams("Brim / Skirt distance", "BSD", "For Brim : Number of Brim Offsets, For Skirt : Distance (in multiples of Nozzle Diameter) of Skirt Offset, For None : Parameter = 0 (default)");
        public static readonly RegisterParams cap = new RegisterParams("Top / Bottom", "TB", "Number of layers as Top and Bottom Caps");
        public static readonly RegisterParams printSpeed = new RegisterParams("Print Speed", "PS", "Speed of Printing in units per/s");
        public static readonly RegisterParams travelSpeed = new RegisterParams("Travel Speed", "TS", "Speed of travelling when not printing in units per/s");
        public static readonly RegisterParams retraction = new RegisterParams("Retraction", "R", "Enable Retraction = true");
        public static readonly RegisterParams retractionDist = new RegisterParams("Rectraction Distance", "RD", "Retraction Distance in millimeters");
        public static readonly RegisterParams retractionSpeed = new RegisterParams("Retraction Speed", "RS", "Feedrate of Retraction in units per/s");
        public static readonly RegisterParams filament = new RegisterParams("Filament Diameter", "F", "Filament Diameter, in millimeters. Also Extrusion volume (mm^2) per mm of extrusion for large scale extruders");
        public static readonly RegisterParams polylines = new RegisterParams("Polylines", "P", "List of Polyline Curves (Convert line types to Polyline). 3D Curves accepted");
        public static readonly RegisterParams sortZ = new RegisterParams("Sort Z", "Z", "Sort List of Curves by ascending Z Value at start of Curve. Default is False");
        public static readonly RegisterParams save = new RegisterParams("Save", "SAVE", "Save. Add Grasshopper Boolean Button");
        public static readonly RegisterParams previewContour = new RegisterParams("Contour", "C", "Contour Out for preview");
        public static readonly RegisterParams previewShell = new RegisterParams("Shell", "S", "Shell Out for preview");
        public static readonly RegisterParams previewInfill = new RegisterParams("Infill", "I", "Infill Out for preview");
        public static readonly RegisterParams previewSkirt = new RegisterParams("Skirt", "S", "Skirt Out for preview");
        public static readonly RegisterParams previewCap = new RegisterParams("Cap", "TB", "Cap Out for preview");
    }
}
