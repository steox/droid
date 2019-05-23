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

    #region Droid GCode class


    public class DroidGCode
    {
        Polylines polylineList = new Polylines();

        double nozzle;
        double layerHeight;
        double firstLayerHeight;
        double extrusionRate;
        double feedRate;
        double travelRate;
        double filamentDiameter;
        bool retraction;
        double retractionDistance;
        int retractionSpeed;

        public DroidGCode(Polylines printList)
        {
            polylineList = printList;
        }

        public List<string> Info(in DroidParameters parameters)
        {
            List<string> info = new List<string>();
            double printDistance = 0;
            double travelDistance = 0;
            Polylines travels = new Polylines();

            for(int i = 0; i < polylineList.Count; i++)
            {
                printDistance += polylineList[i].Length;
                Polyline buffer = new Polyline();
                if (i != 0)
                {
                    buffer.Add(polylineList[(i - 1)].Last);
                    buffer.Add(polylineList[(i)].First);
                }
                travels.Add(buffer);
            }

            foreach(Polyline x in travels)
            {
                travelDistance += x.Length;
            }

            double time = (printDistance / parameters.printSpeed) + (travelDistance / parameters.travelSpeed);
            var timeSpan = TimeSpan.FromSeconds(time);
            double mm3Used = (((parameters.nozzle * parameters.layerHeight) + (Math.PI * Math.Pow(parameters.layerHeight / 2, 2))) / (Math.PI * Math.Pow(parameters.filamentDiameter / 2, 2)) * (parameters.flowRate / 100) * printDistance);
            double cm3Used = Round((mm3Used / 1000), 2);
            double metersUsed = Round((mm3Used / (Math.Pow((parameters.filamentDiameter / 2), 2) * Math.PI) /1000),2);
            info.Add("Print Infomation");
            info.Add("Estimate time (hours,min,sec): " + timeSpan.Hours + ", " + timeSpan.Minutes + ", " + timeSpan.Seconds);
            info.Add("Estimate meters of filament used: " + metersUsed + "m");
            info.Add("Estimate cm^3 (cubic centermeters) used: " + cm3Used + "cm^3");

            return info;
        }

        public List<string> Execute(in DroidParameters parameters, in List<string> header, in List<string> footer)
        {
            nozzle = parameters.nozzle;
            layerHeight = parameters.layerHeight;
            firstLayerHeight = parameters.firstLayerHeight;
            filamentDiameter = parameters.filamentDiameter;
            extrusionRate =  ((nozzle * layerHeight) + (Math.PI * Math.Pow(layerHeight / 2, 2))) / (Math.PI * Math.Pow(filamentDiameter / 2, 2)) * (parameters.flowRate/100);
            feedRate = (parameters.printSpeed * 60);
            travelRate = (parameters.travelSpeed * 60);
            retraction = parameters.retraction;
            retractionDistance = parameters.rectractionDistance;
            retractionSpeed = (parameters.retractionSpeed * 60);

            Vector3d firstLayerVec = new Vector3d(0, 0, firstLayerHeight);
            foreach (Polyline x in polylineList)
            {
                x.Transform(Transform.Translation(firstLayerVec));
            }

            List <string> allGCode = new List<string>();
            Point3d lastPos = new Point3d(0, 0, double.MinValue);

            allGCode.Add(string.Join("\r\n", header));

            for (int i = 0; i < polylineList.Count; i++)
            {
                List<string> code = new List<string>();
                Polyline pl = polylineList[i];
                Point3d travelPos = pl.First();

                if (retraction)
                {
                    if (lastPos.DistanceTo(travelPos) > (nozzle * 1.2 * 10))
                    {
                        code.Add("G92 E0");
                        code.Add("G1 E-" + retractionDistance + " F" + retractionSpeed);
                        code.AddRange(TravelTo(travelPos, travelRate));
                        code.Add("G1 E0.1200" + " F" + retractionSpeed);
                    }
                    else
                    {
                        code.AddRange(TravelTo(travelPos, travelRate));
                    }
                }
                else
                {
                    code.AddRange(TravelTo(travelPos, travelRate));
                }

                code.AddRange(PrintPolyline(pl, feedRate));
                lastPos = pl.Last();
                allGCode.Add(string.Join("\r\n", code));
            }

            allGCode.Add(string.Join("\r\n", footer));
            return allGCode;
        }

        public List<string> PrintPolyline(in Polyline pl, in double printSpeed)
        {
            List<string> partGCode = new List<string>();
            partGCode.Add("G92 E0");
            double extrusionDistance = 0;
            double prevZ = pl[0].Z;
            for (int i = 1; i < pl.Count; i++)
            {
                Point3d currentPt = pl[i];
                Point3d prevPt = pl[i - 1];
                string line = "";
                extrusionDistance += currentPt.DistanceTo(prevPt) * extrusionRate;
                line = "G1 X" + Round(currentPt.X) + " Y" + Round(currentPt.Y);
                if (currentPt.Z != prevZ)
                {
                    prevZ = currentPt.Z;
                    line += " Z" + Round(currentPt.Z);
                }
                line += " E" + Round(extrusionDistance, 5);
                if (i == 1) line += " F" + (int)(printSpeed);

                partGCode.Add(line);
            }
            return partGCode;
        }

        public List<string> TravelTo(in Point3d pt, in double travelSpeed)
        {
            List<string> partGCode = new List<string>();
            partGCode.Add("G1 X" + Round(pt.X) + " Y" + Round(pt.Y) + " Z" + Round(pt.Z) + " F" + (int)travelSpeed);
            return partGCode;
        }

        public double Round(double number)
        {
            return ((long)(number * Math.Pow(10, 3))) / Math.Pow(10, 3);
        }
        public double Round(double number, int place)
        {
            return ((long)(number * Math.Pow(10, place))) / Math.Pow(10, place);
        }

    }

    public class DroidHeader
    {
        public List<string> header = new List<string>();

        public DroidHeader(in int bedtemp, in int nozzletemp, in bool haveHeatedBed, in int fanOn)
        {
            header.Add("; Gcode produced with DROID for Grasshopper / Rhino");
            if (haveHeatedBed) header.Add("M140 S" + bedtemp);
            header.Add("M104 S" + nozzletemp);
            header.Add("G28 ; HOME");
            header.Add("G21 ; UNITS: MM");
            header.Add("G90 ; ABSOLUTE COOD");
            header.Add("M82 ; ABSOLUTE EXTRUDE");
            header.Add("M109 S" + nozzletemp);
            if (haveHeatedBed) header.Add("M190 S" + bedtemp);
            header.Add("M106 S" + Math.Ceiling(fanOn * (255.0 / 100.0)));
            header.Add("; END OF HEADER");
        }
    }
    
    public class DroidFooter
    {
        public List<string> footer = new List<string>();

        public DroidFooter(in double MZ)
        {
            footer.Add("; START OF FOOTER");
            footer.Add("M107");
            footer.Add("M104 S0");
            footer.Add("M140 S0");
            footer.Add("G92 E0");
            footer.Add("G1 E-4 F3600");
            footer.Add("G1 Z" + MZ + " E0");
            footer.Add("M84");
        }
    }

    #endregion

}
