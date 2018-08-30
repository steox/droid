using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Grasshopper.Kernel;
using Rhino.Geometry;
using DroidLib;



namespace Droid.Components
{
    using Polylines = List<Polyline>;

    public class Slicer : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Slicer()
          : base(Title.slicer[0], Title.slicer[1], Title.slicer[2], Title.slicer[3], Title.slicer[4])
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter(Info.mesh[0], Info.mesh[1], Info.mesh[2], GH_ParamAccess.item);
            pManager.AddGenericParameter(Info.parameters[0], Info.parameters[1], Info.parameters[2], GH_ParamAccess.item);
            pManager.AddGenericParameter(Info.droidVolume[0], Info.droidVolume[1], Info.droidVolume[2], GH_ParamAccess.item);
            pManager.AddNumberParameter(Info.xPos[0], Info.xPos[1], Info.xPos[2], GH_ParamAccess.item, 0);
            pManager.AddNumberParameter(Info.yPos[0], Info.yPos[1], Info.yPos[2], GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Info.paths[0], Info.paths[1], Info.paths[2], GH_ParamAccess.item);
            pManager.AddCurveParameter(Info.previewContour[0], Info.previewContour[1], Info.previewContour[2], GH_ParamAccess.list);
            pManager.AddCurveParameter(Info.previewShell[0], Info.previewShell[1], Info.previewShell[2], GH_ParamAccess.list);
            pManager.AddCurveParameter(Info.previewInfill[0], Info.previewInfill[1], Info.previewInfill[2], GH_ParamAccess.list);
            pManager.AddCurveParameter(Info.previewSkirt[0], Info.previewSkirt[1], Info.previewSkirt[2], GH_ParamAccess.list);
            pManager.AddCurveParameter(Info.previewCap[0], Info.previewCap[1], Info.previewCap[2], GH_ParamAccess.list);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            // Inputs
            Rhino.Geometry.Mesh dSlice = null;
            DroidVolume vol = null;
            double x = new double();
            double y = new double();
            DroidParameters para = null;

            if (!DA.GetData(0, ref dSlice)) return;
            if (!DA.GetData(1, ref para)) return;
            if (!DA.GetData(2, ref vol)) return;
            if (!DA.GetData(3, ref x)) return;
            if (!DA.GetData(4, ref y)) return;

            // Initialise
            Polylines[] brimSkirtPaths;
            Polylines[] contourPaths;
            Polylines[] shellPaths;
            List<Polylines[]> fillCapPaths;
            DroidMesh dMesh;

            // Core
            Vector3d normal = new Vector3d(0, 0, 1);
            Plane worldXY = new Plane(Point3d.Origin, normal);

            Vector3d trans = new Vector3d(x, y, 0);
            Rhino.Geometry.Mesh _inputMesh = new Rhino.Geometry.Mesh();

            if (vol.volumeOutline.Length == 2)
            {
                _inputMesh = dSlice;
                BoundingBox bbx = _inputMesh.GetBoundingBox(worldXY);
                Point3d cnr = bbx.Corner(true, true, true);
                Point3d center = bbx.Center;
                center.Z = cnr.Z;
                Vector3d toMiddle = new Vector3d((Point3d.Origin - center + trans));
                _inputMesh.Transform(Transform.Translation(toMiddle));
            }
            if (vol.volumeOutline.Length == 6)
            {
                _inputMesh = dSlice;
                BoundingBox bbx = _inputMesh.GetBoundingBox(worldXY);
                Point3d cnr = bbx.Corner(true, true, true);
                Point3d center = bbx.Center;
                center.Z = cnr.Z;
                Point3d middle = new Point3d((vol.size[0] / 2), (vol.size[1] / 2), 0);
                Vector3d toMiddle = new Vector3d((middle - center + trans));
                _inputMesh.Transform(Transform.Translation(toMiddle));
            }

            dMesh = new DroidMesh();
            dMesh.AssignParameters(para.layerHeight, para.scale, para.nozzle);

            contourPaths = dMesh.Contour(_inputMesh);
            shellPaths = dMesh.Offset(para.shellNumber);
            brimSkirtPaths = dMesh.BrimSkirt(para.brimSkirtInt, para.brimSkirt);
            fillCapPaths = dMesh.DroidBoolPaths(para.infillPercent, para.capThickness, para.shellNumber);

            // Initialising Wrapper
            DroidPaths myDroid = new DroidPaths(contourPaths, shellPaths, fillCapPaths[0], brimSkirtPaths, fillCapPaths[1]);
            
            // Output
            DA.SetData(0, myDroid);
            DA.SetDataList(1, myDroid.wrapperList[1]);
            DA.SetDataList(2, myDroid.wrapperList[2]);
            DA.SetDataList(3, myDroid.wrapperList[3]);
            DA.SetDataList(4, myDroid.wrapperList[0]);
            DA.SetDataList(5, myDroid.wrapperList[4]);
        }
        
        
        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.slice;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f5b38d6e-502a-473d-86a6-5e7985f9c1d6"); }
        }
    }
}
