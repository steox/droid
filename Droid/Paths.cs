using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using DroidLib;

namespace Droid
{
    public class Paths : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Paths()
          : base("Droid Paths", "DPath",
              "Set custom user defined curves (Polylines) for 3d Printing. IMPORTANT: Curves will print in the order the user has defined in input (Unless 'Sort Z' is set to 'true'). Will not check for collisions. For experimental projects",
              "Droid", "Droid")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Polylines", "P", "List of Polyline Curves (Convert line types to Polyline). 3D Curves accepted", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Sort Z", "Z", "Sort List of Curves by ascending Z Value at start of Curve. Default is False", GH_ParamAccess.item, false);
            pManager.AddGenericParameter("Droid Volume", "-> DV", "Input Droid Volume", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Droid Paths", "DP ->", "Droid Paths for use with Droid Components", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> contour = new List<Curve>();
            bool sort = new bool();
            DroidVolume dv = new DroidVolume();

            if (!DA.GetDataList(0, contour)) return;
            if (!DA.GetData(1, ref sort)) return;
            if (!DA.GetData(2, ref dv)) return;

            DroidPaths myDroid = new DroidPaths(contour, sort, dv);

            DA.SetData(0, myDroid);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.path;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("44ef37c7-bf02-460c-a4c2-20fd8b071965"); }
        }
    }
}