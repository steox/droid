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

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    using Polylines = List<Polyline>;
    using cInt = Int64;

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

        public List<string> Execute(DroidParameters parameters, List<string> header, List<string> footer)
        {
            nozzle = parameters.nozzle;
            layerHeight = parameters.layerHeight;
            firstLayerHeight = parameters.firstLayerHeight;
            filamentDiameter = parameters.filamentDiameter;
            extrusionRate = (((nozzle * layerHeight) + (Math.PI * Math.Pow(layerHeight / 2, 2))) / (Math.PI * Math.Pow(filamentDiameter / 2, 2)));
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
                    code.Add("G92 E0");
                    code.Add("G1 E-" + retractionDistance + " F" + retractionSpeed);
                    code.AddRange(TravelTo(travelPos, travelRate));
                    code.Add("G92 E0");
                    code.Add("G1 E" + retractionDistance + " F" + retractionSpeed);
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

        public List<string> PrintPolyline(Polyline pl, double printSpeed)
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

        public List<string> TravelTo(Point3d pt, double travelSpeed)
        {
            List<string> partGCode = new List<string>();
            partGCode.Add("G1 X" + Round(pt.X) + " Y" + Round(pt.Y) + " Z" + Round(pt.Z) + " F" + (int)travelSpeed);
            return partGCode;
        }

        public double Round(double number)
        {
            return ((int)(number * Math.Pow(10, 3))) / Math.Pow(10, 3);
        }
        public double Round(double number, int place)
        {
            return ((int)(number * Math.Pow(10, place))) / Math.Pow(10, place);
        }

    }

    public class DroidHeader
    {
        public List<string> header = new List<string>();

        public DroidHeader(int bedtemp, int nozzletemp, bool haveHeatedBed, bool fanOn)
        {
            header.Add("; Gcode produced by DROID for Grasshopper / Rhino");
            if (haveHeatedBed) header.Add("M140 S" + bedtemp);
            header.Add("M104 S" + nozzletemp);
            header.Add("G28 ; HOME");
            header.Add("G21 ; UNITS: MM");
            header.Add("G90 ; ABSOLUTE COOD");
            header.Add("M82 ; ABSOLUTE EXTRUDE");
            header.Add("M109 S" + nozzletemp);
            if (haveHeatedBed) header.Add("M190 S" + bedtemp);
            if (fanOn) header.Add("M106 S25");
            header.Add("; END OF HEADER");
        }
    }
    
    public class DroidFooter
    {
        public List<string> footer = new List<string>();

        public DroidFooter(double MZ)
        {
            footer.Add("; START OF FOOTER");
            footer.Add("M107");
            footer.Add("M104 S0");
            footer.Add("M140 S0");
            footer.Add("G92 E0");
            footer.Add("G1 E-2 F1800");
            footer.Add("G1 Z" + MZ + " E0");
            footer.Add("G28 X0 Y0");
            footer.Add("M84");
        }
    }

    #endregion

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

        public DroidParameters()
        { }
    }
    #endregion

    #region Save GCode

    public class SaveGCode
    {
        private List<string> _gcode = new List<string>();

        public SaveGCode(List<string> gcode)
        {
            _gcode = gcode;
        }

        public void Save()
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "GCode (*.gcode)|*.gcode";
            saveFileDialog1.Title = "Save GCode File";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    StreamWriter writer = new StreamWriter(myStream);
                    foreach (string x in _gcode)
                    {
                        writer.WriteLine(x);
                    }
                    writer.Close();
                    myStream.Close();
                }
            }
        }
    }
    #endregion

    #region Droid Paths class

    public class DroidPaths
    {
        public List<Polylines> wrapperList = new List<Polylines>();
        public Dictionary<int, List<Polylines>> wrapperPaths = new Dictionary<int, List<Polylines>>();
        public Polylines printList = new Polylines();

        public DroidPaths()
        { }

        public DroidPaths(ConcurrentDictionary<int, Polylines> contours, ConcurrentDictionary<int, Polylines> shell, ConcurrentDictionary<int, Polylines> fill, ConcurrentDictionary<int, Polylines> skirt, ConcurrentDictionary<int, Polylines> cap)
        {
            List<Polylines> ordered = new List<Polylines>();
            Point3d pos = new Point3d(0, 0, 0);
            
            List<ConcurrentDictionary<int, Polylines>> wrapper = new List<ConcurrentDictionary<int, Polylines>>();
            wrapper.Add(skirt);
            wrapper.Add(contours);
            wrapper.Add(shell);
            wrapper.Add(fill);
            wrapper.Add(cap);

            for (int i = 0; i < wrapper.Count; i++)
            {
                Polylines group = new Polylines();
                foreach (Polylines x in wrapper[i].Values)
                {
                    foreach (Polyline y in x)
                    {
                        group.Add(y);
                    }
                }
                wrapperList.Add(group);
            }
               
            for (int i = 0; i < wrapper[1].Count; i++)
            {
                for (int j = 0; j < wrapper.Count; j++)
                {
                    if (wrapper[j].ContainsKey(i))
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

    #region Droid Mesh Class

    public class DroidMesh
    {
        public readonly Mesh inputMesh = new Mesh();
        public double layerHeight = new double();
        public int scale = new int();
        public double nozzle = new double();
        public ConcurrentDictionary<int, Paths> startCD = new ConcurrentDictionary<int, Paths>();
        public ConcurrentDictionary<int, Paths> offsetCD = new ConcurrentDictionary<int, Paths>();

        public DroidMesh()
        { }

        public DroidMesh(Mesh _mesh)
        {
            inputMesh = _mesh;
        }

        public void AssignParameters(double _layerHeight, int _scale, double _nozzle)
        {
            layerHeight = _layerHeight;
            scale = _scale;
            nozzle = _nozzle;
            return;
        }

        // Making of contours by layers - multi-threaded
        //-------------------------------------------------------------------------------------------//

        public ConcurrentDictionary<int, Polylines> Contour()
        {
            Point3d origin = new Point3d(0, 0, 0);
            Vector3d normal = new Vector3d(0, 0, 1);
            Plane worldXY = new Plane(origin, normal);
            Box boundingBx = new Box(inputMesh.GetBoundingBox(worldXY));
            int layerNumber = (Convert.ToInt32((boundingBx.Z.Length - (boundingBx.Z.Length % layerHeight)) / layerHeight) + 1);
            ConcurrentDictionary<int, Polylines> slicedMeshDic = new ConcurrentDictionary<int, Polylines>(Environment.ProcessorCount, layerNumber);

            Parallel.For(0, (layerNumber), i =>
            {
                Paths contourByLayer = new Paths();
                Paths off_contourByLayer = new Paths();
                ClipperOffset offset = new ClipperOffset(2, 0.25);
                Plane cutPlane = new Plane(new Point3d(0, 0, (i * layerHeight)), normal);
                Curve[] arrayOfPoly = Mesh.CreateContourCurves(inputMesh, cutPlane);

                foreach (Curve x in arrayOfPoly)
                {

                    Polyline y = new Polyline();
                    Path polygons = new Path();

                    if (x.TryGetPolyline(out y))
                    {
                        foreach (Point3d r in y)
                        {
                            IntPoint buf = new SClipConvTo().Execute(r, scale);
                            polygons.Add(buf);
                        }
                    }

                    contourByLayer.Add(polygons);
                }

                offset.AddPaths(contourByLayer, JoinType.jtRound, EndType.etClosedPolygon);
                offset.Execute(ref off_contourByLayer, (scale * nozzle * -0.5));

                Polylines pLines = new Polylines();

                foreach (Path x in off_contourByLayer)
                {
                    x.Add(x[0]);

                    Polyline pLine = new Polyline();
                    foreach(IntPoint y in x)
                    {
                        Point3d bufPt = new SClipConvFrom().Execute(y, (i * layerHeight), scale);
                        pLine.Add(bufPt);
                    }

                    pLines.Add(pLine);
                }
                startCD[i] = off_contourByLayer;
                slicedMeshDic[i] = pLines;                
            }
            );

            return slicedMeshDic;
        }

        // Infill - interection per Layer is multi-threaded
        //-------------------------------------------------------------------------------------------//

        public List<ConcurrentDictionary<int, Polylines>> DroidBoolPaths(int infillPercent, int thickness, int shell)
        {
            ConcurrentDictionary<int, Polylines> capPaths = new ConcurrentDictionary<int, Polylines>(Environment.ProcessorCount, startCD.Count);
            ConcurrentDictionary<int, Polylines> fillPaths = new ConcurrentDictionary<int, Polylines>(Environment.ProcessorCount, startCD.Count);
            List<ConcurrentDictionary<int, Polylines>> output = new List<ConcurrentDictionary<int, Polylines>>();
            Point3d origin = new Point3d(0, 0, 0);
            Vector3d normal = new Vector3d(0, 0, 1);
            Plane worldXY = new Plane(origin, normal);

            Polylines cInfill = new Polylines();
            Polylines cCap1 = new Polylines();
            Polylines cCap0 = new Polylines();

            Path boundPath = new Path();

            Mesh rotateMesh = inputMesh;
            rotateMesh.Rotate((Math.PI * 0.25), normal, origin);

            BoundingBox BB = rotateMesh.GetBoundingBox(worldXY);
            Point3d BBPt = BB.Center;
            BBPt.Z = 0;
            BB.Transform(Transform.Scale(BBPt, 1.5));

            Point3d[] corners = BB.GetCorners();

            corners[0].Transform(Transform.Rotation(Math.PI * 1.75, origin));
            corners[1].Transform(Transform.Rotation(Math.PI * 1.75, origin));
            corners[2].Transform(Transform.Rotation(Math.PI * 1.75, origin));
            corners[3].Transform(Transform.Rotation(Math.PI * 1.75, origin));
            rotateMesh.Rotate((Math.PI * 1.75), normal, origin);
            for (int w = 0; w <= 3; w++)
            {
                IntPoint cnr = new SClipConvTo().Execute(corners[w], scale);
                boundPath.Add(cnr);
            }

            // cap creation
            //-------------------------------------------------------------------------------------------

            if (thickness != 0)
            {
                Line c1 = new Line(corners[0], corners[1]);
                int noOfStrokesc1 = Convert.ToInt32(Math.Floor(c1.Length / nozzle));

                Vector3d bufVecc1 = new Vector3d(1, -1, 0);
                bufVecc1.Unitize();
                bufVecc1 *= nozzle;

                for (int i = 0; i <= noOfStrokesc1; i++)
                {
                    Polyline line = new Polyline();

                    Point3d startPt = corners[0];
                    Point3d endPt = corners[3];

                    startPt += (bufVecc1 * i);
                    endPt += (bufVecc1 * i);

                    line.Add(startPt);
                    line.Add(endPt);
                    
                    cCap1.Add(line);
                }

                Line c0 = new Line(corners[0], corners[3]);
                int noOfStrokesc0 = Convert.ToInt32(Math.Floor(c0.Length / nozzle));

                Vector3d bufVecc0 = new Vector3d(1, 1, 0);
                bufVecc0.Unitize();
                bufVecc0 *= nozzle;

                for (int i = 0; i <= noOfStrokesc0; i++)
                {
                    Polyline line = new Polyline();

                    Point3d startPt = corners[0];
                    Point3d endPt = corners[1];

                    startPt += (bufVecc0 * i);
                    endPt += (bufVecc0 * i);

                    line.Add(startPt);
                    line.Add(endPt);

                    cCap0.Add(line);
                }
            }
            // Infill creation
            //-------------------------------------------------------------------------------------------

            if (infillPercent != 0)
            {
                
                Line a1 = new Line(corners[0], corners[1]);
                Line b1 = new Line(corners[0], corners[3]);

                int noOfStrokesa1 = Convert.ToInt32(Math.Floor((a1.Length * ((0.5 * infillPercent) / 100)) / nozzle));
                double spacinga1 = a1.Length / noOfStrokesa1;

                int noOfStrokesb1 = Convert.ToInt32(Math.Floor((b1.Length * ((0.5 * infillPercent) / 100)) / nozzle));
                double spacingb1 = b1.Length / noOfStrokesb1;

                Vector3d bufVec = new Vector3d(1, -1, 0);
                bufVec.Unitize();
                bufVec *= spacinga1;
                Vector3d bufVec2 = new Vector3d(1, 1, 0);
                bufVec2.Unitize();
                bufVec2 *= spacingb1;

                for (int i = 0; i <= noOfStrokesa1; i++)
                {
                    Polyline line = new Polyline();

                    Point3d startPt = corners[0];
                    Point3d endPt = corners[3];

                    startPt += (bufVec * i);
                    endPt += (bufVec * i);

                    line.Add(startPt);
                    line.Add(endPt);

                    cInfill.Add(line);
                }

                for (int i = 0; i <= noOfStrokesb1; i++)
                {
                    Polyline line = new Polyline();

                    Point3d startPt = corners[0];
                    Point3d endPt = corners[1];

                    startPt += (bufVec2 * i);
                    endPt += (bufVec2 * i);

                    line.Add(startPt);
                    line.Add(endPt);

                    cInfill.Add(line);
                }
            }

            // Gets the fills and caps
            //-------------------------------------------------------------------------------------------

            Parallel.For(0, startCD.Count, _i =>
            {
                int i = _i;

                Clipper booleandifTop = new Clipper();
                Clipper booleandifBot = new Clipper();
                Clipper booleandifTopInvert = new Clipper();
                Clipper booleandifBotInvert = new Clipper();
                Clipper booleancapunionTop = new Clipper();
                Clipper booleancapunionBot = new Clipper();
                Clipper booleancapunionAll = new Clipper();
                Clipper booleanfill = new Clipper();

                Paths capTop = new Paths();
                Paths capBot = new Paths();
                Paths capAll = new Paths();
                Paths fill = new Paths();

                Paths thisOffset = offsetCD[i];
                Paths thisOGT = new Paths();
                Paths thisOGB = new Paths();
                ClipperOffset getOGT = new ClipperOffset(2, 0.25);
                ClipperOffset getOGB = new ClipperOffset(2, 0.25);
                
                if (thickness != 0)
                {
                    if ((i + thickness) >= (startCD.Count))
                    {
                        capTop = offsetCD[i];
                    }
                    else
                    {
                        if (shell != 1)
                        {
                            getOGT.AddPaths(offsetCD[(i + thickness)], JoinType.jtRound, EndType.etClosedPolygon);
                            getOGT.Execute(ref thisOGT, (scale * (shell - 1) * nozzle));
                        }
                        else
                        {
                            thisOGT = offsetCD[(i + thickness)];
                        }
                        foreach (Path adding in thisOGT)
                        {
                            adding.Add(adding[0]);
                        }
                        Paths temp = new Paths();
                        Paths temp2 = new Paths();
                        booleandifTop.AddPath(boundPath, PolyType.ptSubject, true);
                        booleandifTop.AddPaths(thisOffset, PolyType.ptClip, true);
                        booleandifTop.Execute(ClipType.ctDifference, temp, PolyFillType.pftEvenOdd);
                        foreach (Path adding in temp)
                        {
                            adding.Add(adding[0]);
                        }
                        booleancapunionTop.AddPaths(temp, PolyType.ptSubject, true);
                        booleancapunionTop.AddPaths(thisOGT, PolyType.ptClip, true);
                        booleancapunionTop.Execute(ClipType.ctUnion, temp2, PolyFillType.pftEvenOdd);
                        foreach (Path adding in temp2)
                        {
                            adding.Add(adding[0]);
                        }
                        booleandifTopInvert.AddPath(boundPath, PolyType.ptSubject, true);
                        booleandifTopInvert.AddPaths(temp2, PolyType.ptClip, true);
                        booleandifTopInvert.Execute(ClipType.ctDifference, capTop, PolyFillType.pftEvenOdd);
                        foreach (Path adding in capTop)
                        {
                            adding.Add(adding[0]);
                        }
                    }
                    if ((i - thickness) < 0)
                    {
                        capBot = offsetCD[i];
                    }
                    else
                    {
                        if (shell != 1)
                        {
                            getOGB.AddPaths(offsetCD[(i - thickness)], JoinType.jtRound, EndType.etClosedPolygon);
                            getOGB.Execute(ref thisOGB, (scale * (shell - 1) * nozzle));
                        }
                        else
                        {
                            thisOGB = offsetCD[(i - thickness)];
                        }
                        foreach (Path adding in thisOGB)
                        {
                            adding.Add(adding[0]);
                        }

                        Paths temp = new Paths();
                        Paths temp2 = new Paths();
                        booleandifBot.AddPath(boundPath, PolyType.ptSubject, true);
                        booleandifBot.AddPaths(thisOffset, PolyType.ptClip, true);
                        booleandifBot.Execute(ClipType.ctDifference, temp, PolyFillType.pftEvenOdd);
                        foreach (Path adding in temp)
                        {
                            adding.Add(adding[0]);
                        }
                        booleancapunionBot.AddPaths(temp, PolyType.ptSubject, true);
                        booleancapunionBot.AddPaths(thisOGB, PolyType.ptClip, true);
                        booleancapunionBot.Execute(ClipType.ctUnion, temp2, PolyFillType.pftEvenOdd);
                        foreach (Path adding in temp2)
                        {
                            adding.Add(adding[0]);
                        }
                        booleandifBotInvert.AddPath(boundPath, PolyType.ptSubject, true);
                        booleandifBotInvert.AddPaths(temp2, PolyType.ptClip, true);
                        booleandifBotInvert.Execute(ClipType.ctDifference, capBot, PolyFillType.pftEvenOdd);
                        foreach (Path adding in capBot)
                        {
                            adding.Add(adding[0]);
                        }
                    }
                    booleancapunionAll.AddPaths(capTop, PolyType.ptSubject, true);
                    booleancapunionAll.AddPaths(capBot, PolyType.ptClip, true);
                    booleancapunionAll.Execute(ClipType.ctUnion, capAll, PolyFillType.pftEvenOdd);
                    foreach (Path adding in capAll)
                    {
                        adding.Add(adding[0]);
                    }

                    //------------ intersect

                    if (i % 2 == 0)
                    {
                        Polylines gotcap = new Polylines();

                        foreach (Polyline p in cCap0)
                        {
                            List<Point3d> onLine = new List<Point3d>();
                            PolylineCurve _p = new PolylineCurve(p);

                            foreach (Path x in capAll)
                            {
                                x.Add(x[0]);
                                Polyline pLine = new Polyline();

                                foreach (IntPoint y in x)
                                {
                                    Point3d bufPt = new SClipConvFrom().Execute(y, (0), scale);
                                    pLine.Add(bufPt);
                                }
                                PolylineCurve _pLine = new PolylineCurve(pLine);
                                _pLine.MakeClosed((0));
                                CurveIntersections interS = Intersection.CurveCurve(_p, _pLine, double.Epsilon, 0.0);

                                for (int beta = 0; beta < interS.Count; beta++)
                                {
                                    onLine.Add(interS[beta].PointA);
                                }
                            }

                            List<Point3d> _onLine = onLine.OrderBy(tPt => tPt.X).ToList();
                            for (int omega = 0; omega < (_onLine.Count - 1); omega++)
                            {
                                if ((omega % 2 == 0))
                                {
                                    Polyline inside = new Polyline();
                                    Point3d startPt = _onLine[omega];
                                    Point3d endPt = _onLine[(omega + 1)];

                                    startPt.Z = (layerHeight * i);
                                    endPt.Z = (layerHeight * i);

                                    inside.Add(startPt);
                                    inside.Add(endPt);

                                    gotcap.Add(inside);
                                }
                            }
                        }
                        capPaths[i] = gotcap;
                    }

                    else
                    {
                        Polylines gotcap = new Polylines();

                        foreach (Polyline p in cCap1)
                        {
                            List<Point3d> onLine = new List<Point3d>();
                            PolylineCurve _p = new PolylineCurve(p);

                            foreach (Path x in capAll)
                            {
                                x.Add(x[0]);
                                Polyline pLine = new Polyline();

                                foreach (IntPoint y in x)
                                {
                                    Point3d bufPt = new SClipConvFrom().Execute(y, (0), scale);
                                    pLine.Add(bufPt);
                                }
                                PolylineCurve _pLine = new PolylineCurve(pLine);
                                _pLine.MakeClosed((0));
                                CurveIntersections interS = Intersection.CurveCurve(_p, _pLine, double.Epsilon, 0.0);

                                for (int beta = 0; beta < interS.Count; beta++)
                                {
                                    onLine.Add(interS[beta].PointA);
                                }
                            }

                            List<Point3d> _onLine = onLine.OrderBy(tPt => tPt.X).ToList();
                            for (int omega = 0; omega < (_onLine.Count - 1); omega++)
                            {
                                if ((omega % 2 == 0))
                                {
                                    Polyline inside = new Polyline();
                                    Point3d startPt = _onLine[omega];
                                    Point3d endPt = _onLine[(omega + 1)];

                                    startPt.Z = (layerHeight * i);
                                    endPt.Z = (layerHeight * i);

                                    inside.Add(startPt);
                                    inside.Add(endPt);

                                    gotcap.Add(inside);
                                }
                            }
                        }
                        capPaths[i] = gotcap;
                    }
                }

                if (infillPercent != 0)
                {
                    booleanfill.AddPaths(offsetCD[i], PolyType.ptSubject, true);
                    booleanfill.AddPaths(capAll, PolyType.ptClip, true);
                    booleanfill.Execute(ClipType.ctDifference, fill, PolyFillType.pftEvenOdd);
                    foreach (Path adding in fill)
                    {
                        adding.Add(adding[0]);
                    }

                    //----------- intersect
                    Polylines gotfill = new Polylines();

                    foreach (Polyline p in cInfill)
                    {
                        List<Point3d> onLine = new List<Point3d>();
                        PolylineCurve _p = new PolylineCurve(p);

                        foreach (Path x in fill)
                        {
                            x.Add(x[0]);
                            Polyline pLine = new Polyline();

                            foreach (IntPoint y in x)
                            {
                                Point3d bufPt = new SClipConvFrom().Execute(y, (0), scale);
                                pLine.Add(bufPt);
                            }
                            PolylineCurve _pLine = new PolylineCurve(pLine);
                            CurveIntersections interS = Intersection.CurveCurve(_p, _pLine, 0, 0);

                            for (int beta = 0; beta < interS.Count; beta++)
                            {
                                onLine.Add(interS[beta].PointA);
                            }
                        }

                        List<Point3d> _onLine = onLine.OrderBy(tPt => tPt.X).ToList();
                        for (int omega = 0; omega < (_onLine.Count - 1); omega++)
                        {
                            if ((omega % 2 == 0))
                            {
                                Polyline inside = new Polyline();
                                Point3d startPt = _onLine[omega];
                                Point3d endPt = _onLine[(omega + 1)];

                                startPt.Z = (layerHeight * i);
                                endPt.Z = (layerHeight * i);

                                inside.Add(startPt);
                                inside.Add(endPt);

                                gotfill.Add(inside);
                            }
                        }
                    }
                    fillPaths[i] = gotfill;
                }
            }
            );
            
            output.Add(fillPaths);
            output.Add(capPaths);

            return output;
        }

        // Offset Creation - multi-threaded
        //-------------------------------------------------------------------------------------------//

        public ConcurrentDictionary<int, Polylines> Offset(int shell)
        {
            ConcurrentDictionary<int, Polylines> offsetMeshDic = new ConcurrentDictionary<int, Polylines>(Environment.ProcessorCount, startCD.Count);
            
            Parallel.For(0, startCD.Count, i =>
            {
                ClipperOffset offset = new ClipperOffset(2, 0.25);
                Polylines pLines = new Polylines();
                Paths pLinesLast = new Paths();
                Paths buffer = startCD[i];

                for (int j = 1; j < shell; j++)
                {
                    Paths solution = new Paths();

                    offset.Clear();
                    offset.AddPaths(buffer, JoinType.jtRound, EndType.etClosedPolygon);
                    offset.Execute(ref solution, ((j * (-1 * nozzle)) * scale));
                    buffer.Clear();

                    foreach (Path x in solution)
                    {
                        x.Add(x[0]);
                        buffer.Add(x);

                        Polyline pLine = new Polyline();
                        foreach (IntPoint y in x)
                        {
                            Point3d bufPt = new SClipConvFrom().Execute(y, (i * layerHeight), scale);
                            pLine.Add(bufPt);

                        }
                        if (j == (shell - 1))
                        {
                            pLinesLast.Add(x);
                        }
                        pLines.Add(pLine);
                    }
                }
                if (shell == 1)
                {
                    pLinesLast = startCD[i];
                }
                
                offsetCD[i] = pLinesLast;
                offsetMeshDic[i] = pLines;
            }
            );

            return offsetMeshDic;
        }

        // Brim / Skirt Creation
        //-------------------------------------------------------------------------------------------//

        public ConcurrentDictionary<int, Polylines> BrimSkirt(int number, bool theboolean)
        {
            ConcurrentDictionary<int, Polylines> BrimSkirtDic = new ConcurrentDictionary<int, Polylines>(Environment.ProcessorCount, startCD.Count);
            Polylines pLines = new Polylines();
            ClipperOffset offset = new ClipperOffset(2, 0.25);

            if (theboolean)
            {

                Paths buffer = startCD[0];

                for (int i = 0; i < number; i++)
                {
                    Paths during = new Paths();

                    offset.Clear();
                    offset.AddPaths(buffer, JoinType.jtRound, EndType.etClosedPolygon);
                    if (i == 0)
                    {
                        offset.Execute(ref during, (2 * nozzle * scale));
                    }
                    else
                    {
                        offset.Execute(ref during, (nozzle * scale));
                    }
                    buffer.Clear();

                    foreach (Path x in during)
                    {
                        x.Add(x[0]);
                        buffer.Add(x);

                        Polyline pLine = new Polyline();
                        foreach (IntPoint y in x)
                        {
                            Point3d bufPt = new SClipConvFrom().Execute(y, (0), scale);
                            pLine.Add(bufPt);
                        }
                        pLines.Add(pLine);
                    }
                }
            }

            else
            {
                if (number > 0)
                {
                    Paths during = new Paths();
                    offset.AddPaths(startCD[0], JoinType.jtRound, EndType.etClosedPolygon);
                    offset.Execute(ref during, ((1 + number) * nozzle * scale));

                    foreach (Path x in during)
                    {
                        x.Add(x[0]);
                        Polyline pLine = new Polyline();
                        foreach (IntPoint y in x)
                        {
                            Point3d bufPt = new SClipConvFrom().Execute(y, (0), scale);
                            pLine.Add(bufPt);
                        }
                        pLines.Add(pLine);
                    }
                }
            }
            BrimSkirtDic[0] = pLines;
            return BrimSkirtDic;
        }
    }
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
