using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Grasshopper.Kernel;
using Rhino.Geometry;
using DroidLib;



namespace Droid
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
          : base("Droid Slicer", "Dslice",
              "Slicer of Droid Mesh in preparation for 3D Printing",
              "Droid", "Droid")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Droid Mesh", "-> DM", "Input Droid Mesh for Slicing", GH_ParamAccess.item);
            pManager.AddGenericParameter("Droid Parameters", "-> DPr", "Droid Parameters for Slicing", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Droid Paths", "DP ->", "Droid Paths for use with Droid Components", GH_ParamAccess.item);
            pManager.AddCurveParameter("Contour", "C", "Contour Out for preview", GH_ParamAccess.list);
            pManager.AddCurveParameter("Shell", "S", "Shell Out for preview", GH_ParamAccess.list);
            pManager.AddCurveParameter("Infill", "I", "Infill Out for preview", GH_ParamAccess.list);
            pManager.AddCurveParameter("Skirt", "S", "Skirt Out for preview", GH_ParamAccess.list);
            pManager.AddCurveParameter("Cap", "TB", "Cap Out for preview", GH_ParamAccess.list);
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            // Inputs
            DroidMesh dSlice = new DroidMesh();
            DroidParameters para = new DroidParameters();

            if (!DA.GetData(0, ref dSlice)) return;
            if (!DA.GetData(1, ref para)) return;

            // Initialise
            ConcurrentDictionary<int, Polylines> brimSkirtPaths = new ConcurrentDictionary<int, Polylines>();
            ConcurrentDictionary<int, Polylines> contourPaths = new ConcurrentDictionary<int, Polylines>();
            ConcurrentDictionary<int, Polylines> shellPaths = new ConcurrentDictionary<int, Polylines>();
            List<ConcurrentDictionary<int, Polylines>> fillCapPaths = new List<ConcurrentDictionary<int, Polylines>>();

            // Core
            dSlice.AssignParameters(para.layerHeight, para.scale, para.nozzle);

            contourPaths = dSlice.Contour();
            shellPaths = dSlice.Offset(para.shellNumber);
            brimSkirtPaths = dSlice.BrimSkirt(para.brimSkirtInt, para.brimSkirt);
            fillCapPaths = dSlice.DroidBoolPaths(para.infillPercent, para.capThickness, para.shellNumber);

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
