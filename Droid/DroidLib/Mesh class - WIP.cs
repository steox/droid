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
using DroidLib;

namespace _DroidLib
{
    #region Internal Definitions 

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    using Polylines = List<Polyline>;
    using cInt = Int64;

    #endregion

    #region Droid Mesh Class

    public class DroidMesh
    {
        private Box rotatedBoundingBx;
        private double layerHeight;
        private int scale;
        private double extrusionWidth;
        private static readonly Point3d origin = new Point3d(0, 0, 0);
        private static readonly Vector3d normal = new Vector3d(0, 0, 1);
        private static readonly Plane worldXY = new Plane(origin, normal);
        private Plane rotateXY = new Plane(origin, normal);
        private Paths[] startContour;
        public ConcurrentDictionary<int, Paths> startCD = new ConcurrentDictionary<int, Paths>();
        public ConcurrentDictionary<int, Paths> offsetCD = new ConcurrentDictionary<int, Paths>();

        public DroidMesh()
        {
        }
        
        public void AssignParameters(in double _layerHeight ,in int _scale, in double _nozzle)
        {
            layerHeight = _layerHeight;
            scale = _scale;
            extrusionWidth = (_nozzle * 1.20);
        }
        
        /// <summary>
        /// Making of contours by layers - multi-threaded
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<int, Polylines> Contour(Mesh inputMesh)
        {

            rotateXY.Transform(Transform.Rotation(Math.PI * 0.25, origin));
            Box boundingBx = new Box(inputMesh.GetBoundingBox(worldXY));
            rotatedBoundingBx = new Box(inputMesh.GetBoundingBox(rotateXY));
            rotatedBoundingBx.Transform(Transform.Rotation(Math.PI * -0.25, origin));

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
                offset.Execute(ref off_contourByLayer, (scale * extrusionWidth * -0.5));

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
        
        /// <summary>
        /// Infill - intersection per Layer is multi-threaded
        /// </summary>
        /// <param name="infillPercent"></param>
        /// <param name="thickness"></param>
        /// <param name="shell"></param>
        /// <returns></returns>
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
                        
            Point3d BBPt = rotatedBoundingBx.Center;
            BBPt.Z = 0;
            rotatedBoundingBx.Transform(Transform.Scale(BBPt, 1.5));

            Point3d[] corners = rotatedBoundingBx.GetCorners();

            corners[0].Transform(Transform.Rotation(Math.PI * -0.25, origin));
            corners[1].Transform(Transform.Rotation(Math.PI * -0.25, origin));
            corners[2].Transform(Transform.Rotation(Math.PI * -0.25, origin));
            corners[3].Transform(Transform.Rotation(Math.PI * -0.25, origin));

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
                int noOfStrokesc1 = Convert.ToInt32(Math.Floor(c1.Length / extrusionWidth));

                Vector3d bufVecc1 = new Vector3d(1, -1, 0);
                bufVecc1.Unitize();
                bufVecc1 *= extrusionWidth;

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
                int noOfStrokesc0 = Convert.ToInt32(Math.Floor(c0.Length / extrusionWidth));

                Vector3d bufVecc0 = new Vector3d(1, 1, 0);
                bufVecc0.Unitize();
                bufVecc0 *= extrusionWidth;

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

                int noOfStrokesa1 = Convert.ToInt32(Math.Floor((a1.Length * ((0.5 * infillPercent) / 100)) / extrusionWidth));
                double spacinga1 = a1.Length / noOfStrokesa1;

                int noOfStrokesb1 = Convert.ToInt32(Math.Floor((b1.Length * ((0.5 * infillPercent) / 100)) / extrusionWidth));
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
                            getOGT.Execute(ref thisOGT, (scale * (shell - 1) * extrusionWidth));
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
                            getOGB.Execute(ref thisOGB, (scale * (shell - 1) * extrusionWidth));
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
                    offset.Execute(ref solution, ((j * (-1 * extrusionWidth)) * scale));
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
                        offset.Execute(ref during, (2 * extrusionWidth * scale));
                    }
                    else
                    {
                        offset.Execute(ref during, (extrusionWidth * scale));
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
                    offset.Execute(ref during, ((1 + number) * extrusionWidth * scale));

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
    
}
